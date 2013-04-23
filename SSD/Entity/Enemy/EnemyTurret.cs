using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class EnemyTurret : EnemyEntity
    {
        public EnemyTurret(Vector3 position, ModelContainer model, Entity planetSphere, Random ran, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0)
            : base(position, model, true, scale, yaw, pitch, roll)
        {
            _planetTarget = planetSphere;
            

            //delta = object2 - object1;
            //delta.noramlise;

            //then find out the right vector
            //right = vector(-delta.z, 0, delta.x);
            //right.normalise;

            //then to find the up vector just find the angle between delta and right using the crossproduct.
            //up = delta.crossproject(right);
            //up.normalise;

            Vector3 directionVector = this.getMatrix().Translation - planetSphere.getMatrix().Translation;
            directionVector.Normalize();

            Vector3 right = new Vector3(-directionVector.X, 0, directionVector.X);
            right.Normalize();

            Vector3 up = Vector3.Cross(directionVector, right);
            up.Normalize();



            _bulletShootSpeed = 2000;
            _lastBulletShot = _bulletShootSpeed;
            _rotation = 0;

            _isIceEnemy = false;
            base.setHealth(600);
        }

        override public Matrix getMatrix()
        {
            Matrix transform = Matrix.Identity;

            Quaternion accumulateRotation = base.getRotation();

            transform *= Matrix.CreateScale(base.getScale());
            transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(this.getYaw(), this.getPitch(), this.getRoll()));
            transform *= Matrix.CreateTranslation(base.getPosition());
            //transform *= Matrix.CreateFromQuaternion(this.getRawRotation());
            //transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(base.getMatrix().Forward, MathHelper.ToRadians(0.1f)));

            //setPosition(transform.Translation);
            //addRotation(Quaternion.CreateFromAxisAngle(base.getMatrix().Forward, MathHelper.ToRadians(0.1f)));

            return transform;
        }

        override public void update(TimeSpan deltaTime)
        {
            float dt = deltaTime.Milliseconds / 10.0f;

            if (_isSpawning)
            {
                this.setPosition(this.getPosition() + (_planetTarget.getMatrix().Translation - this.getMatrix().Translation) * 0.001f * dt);
            }

            _lastBulletShot -= deltaTime.Milliseconds;

            //base.setPosition(base.getPosition() + base.getMatrix().Forward);

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

                        effect.SpecularColor = Color.White.ToVector3();
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

        public bool shouldShoot()
        {
            return _lastBulletShot <= 0;
        }

        public void didShoot()
        {
            _lastBulletShot = _bulletShootSpeed;
        }

        public Quaternion getOrientationToPlanet()
        {
            Matrix rotationMat = Matrix.Identity;
            Matrix currentPosition = getMatrix();
            rotationMat.Down = currentPosition.Translation - _planetTarget.getMatrix().Translation;
            rotationMat.Down.Normalize();
            //rotationMat.Up = -rotationMat.Down;
            rotationMat.Right = Vector3.Cross(currentPosition.Forward, rotationMat.Up);
            rotationMat.Right.Normalize();
            //rotationMat.Left = -rotationMat.Right;
            rotationMat.Forward = Vector3.Cross(rotationMat.Left, rotationMat.Up);
            //rotationMat.Backward = -rotationMat.Forward;

            return Quaternion.CreateFromRotationMatrix(rotationMat);

           // return Quaternion.CreateFromRotationMatrix//
        }

        Entity _planetTarget;
        Draw _renderer;
        float _lastBulletShot;
        float _bulletShootSpeed;
        float _rotation;

    }
}
