﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SSD
{
    class EnemyMine : EnemyEntity
    {
        public EnemyMine(Vector3 position, ModelContainer model, Entity planetSphere, Random ran, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0)
            : base(position, model, scale, yaw, pitch, roll)
        {
            _planetTarget = planetSphere;

            //Random ran = new Random();
            _forwardSpeed = (((float)ran.NextDouble() - 0.5f) * 4) * 0.001f;
            _sideSpeed = (((float)ran.NextDouble() - 0.5f) * 4) * 0.001f;
            _speed = 1.0f;
        }

        override public Matrix getMatrix()
        {
            Matrix transform = Matrix.Identity;

            Quaternion accumulateRotation = base.getRotation();

            transform *= Matrix.CreateScale(base.getScale());
            transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(this.getYaw(), this.getPitch(), this.getRoll()));
            transform *= Matrix.CreateTranslation(base.getPosition());
            transform *= Matrix.CreateFromQuaternion(this.getRawRotation());

            return transform;
        }

        override public void update(TimeSpan deltaTime)
        {
            float dt = deltaTime.Milliseconds / 10.0f;


            if (_isSpawning)
            {
                this.setPosition(this.getPosition() + (_planetTarget.getMatrix().Translation - this.getMatrix().Translation) * 0.001f * dt);
                this.addRoll(_forwardSpeed * 10 * -1 * deltaTime.Milliseconds);
                this.addPitch(_sideSpeed * 10 * -1 * deltaTime.Milliseconds);
            }
            else
            {
                this.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Forward, _forwardSpeed * _speed * dt));
                this.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, _sideSpeed * _speed * dt));

                this.addRoll(_forwardSpeed * 1000 * dt);
                this.addPitch(_sideSpeed * 1000 * dt);
            }

            base.update(deltaTime);
        }

        Entity _planetTarget;
        float _forwardSpeed;
        float _sideSpeed;
        float _speed;

    }
}
