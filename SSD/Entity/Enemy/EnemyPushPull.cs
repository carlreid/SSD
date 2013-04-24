using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class EnemyPushPull : EnemyEntity
    {
        public EnemyPushPull(Vector3 position, ModelContainer model, Entity planetSphere, PlayerEntity player, Random ran, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0)
            : base(position, model, false, scale, yaw, pitch, roll)
        {
            _planetTarget = planetSphere;
            _player = player;
            _forwardSpeed = (((float)ran.NextDouble() - 0.5f) * ran.Next(4, 10)) * 0.001f;
            _sideSpeed = (((float)ran.NextDouble() - 0.5f) * ran.Next(4, 10)) * 0.001f;
            _speed = 1.0f;
            _myScore = 2500;
            base.setHealth(2000);
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
                float distanceBetween = Vector3.Distance(_player.getMatrix().Translation, getMatrix().Translation);
                if (distanceBetween < 200)
                {
                    Vector3 direction = _player.getMatrix().Translation - getMatrix().Translation;
                    direction.Normalize();

                    float angle = MathHelper.ToDegrees((float)-Math.Atan2(-direction.X, direction.Z)) - 90;

                    _player.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, (float)(Math.Cos(angle) * 0.002f)));
                    _player.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, (float)(Math.Sin(angle) * 0.002f)));

                    this.addRoll(_forwardSpeed * 1000 * -1 * deltaTime.Milliseconds);
                    this.addPitch(_sideSpeed * 1000 * -1 * deltaTime.Milliseconds);
                }
                else
                {
                    this.addRoll(_forwardSpeed * 100 * -1 * deltaTime.Milliseconds);
                    this.addPitch(_sideSpeed * 100 * -1 * deltaTime.Milliseconds);
                }
            }

            base.update(deltaTime);
        }

        public override void draw(Matrix view, Matrix proj, Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice)
        {
            foreach (ModelMesh mesh in getModelContainer().getModel().Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = getModelContainer().getBoneTransform(mesh.ParentBone.Index) * getMatrix();
                        effect.View = view;
                        effect.Projection = proj;

                        effect.SpecularColor = Color.Red.ToVector3();
                        effect.SpecularPower = 200f;

                        effect.LightingEnabled = true; // turn on the lighting subsystem.
                        effect.DirectionalLight0.Enabled = true;
                        effect.DirectionalLight1.Enabled = true;
                        //effect.DirectionalLight2.Enabled = true;

                        effect.DirectionalLight0.Direction = new Vector3(1, 1, 1);
                        effect.DirectionalLight0.SpecularColor = Color.White.ToVector3();
                        effect.DirectionalLight1.Direction = new Vector3(-1, -1, -1);
                        effect.DirectionalLight1.SpecularColor = Color.White.ToVector3();
                    }
                    //BoundingSphereRenderer.Render(entity.getBoundingSphere(), _graphicsDevice, view, proj, Color.Red);
                    mesh.Draw();
                }
            }
        }

        Entity _planetTarget;
        PlayerEntity _player;
        float _forwardSpeed;
        float _sideSpeed;
        float _speed;

    }
}
