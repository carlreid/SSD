using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

using DPSF;
using DPSF.ParticleSystems;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class Bullet : ModelEntity
    {
        public Bullet(Vector3 position, Quaternion rotation, float yaw, ModelContainer bulletModel)
            : base(position, bulletModel, 1f, yaw)
        {
            base.addRotation(rotation);

            _curAngle = 0;
            _angleModifier = 1.5f;
            _timeTillDeath = 1000;
            _damage = 100;
        }

        override public Matrix getMatrix()
        {
            Matrix transform = Matrix.Identity;

            Quaternion accumulateRotation = base.getRotation();

            transform *= Matrix.CreateScale(base.getScale());
            transform *= Matrix.CreateFromQuaternion(accumulateRotation);
            transform *= Matrix.CreateTranslation(base.getPosition());
            transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(base.getMatrix().Forward, MathHelper.ToRadians(_curAngle)));

            return transform;
        }

        override public void update(TimeSpan deltaTime)
        {

            _curAngle += _angleModifier;

            if (_angleModifier > 0.5f)
            {
                _angleModifier -= 0.018f;
                if (_angleModifier < 0.5f)
                {
                    _angleModifier = 0.5f;
                }
            }
            

            _timeTillDeath -= deltaTime.Milliseconds;
            if (_timeTillDeath <= 0)
            {
                base.setAlive(false);
            }

            base.update(deltaTime);
        }

        public int getDamage()
        {
            return _damage;
        }

        public void setDamage(int amountOfDamage)
        {
            _damage = amountOfDamage;
        }

        //public bool getAlive()
        //{
        //    return _isAlive;
        //}

        //float _velocity;
        Vector3 _direction;
        float _directionYaw;
        float _timeTillDeath;

        float _curAngle;
        float _angleModifier;
        //Vector3 _position;
        int _damage;
    }
}
