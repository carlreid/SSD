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
            _speed = 1.0f;
            _scale = 1.0f;
        }

        override public Matrix getMatrix()
        {
            Matrix transform = Matrix.Identity;

            Quaternion accumulateRotation = base.getRotation();

            transform *= Matrix.CreateScale(base.getScale() * _scale);
            transform *= Matrix.CreateFromQuaternion(accumulateRotation);
            transform *= Matrix.CreateTranslation(base.getPosition());
            transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(base.getMatrix().Forward, MathHelper.ToRadians(_curAngle)));

            return transform;
        }

        override public void update(TimeSpan deltaTime)
        {

            _curAngle += _angleModifier * _speed;

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

        //public override void draw(Matrix view, Matrix proj, GraphicsDevice graphicsDevice)
        //{
        //    foreach (ModelMesh mesh in getModelContainer().getModel().Meshes)
        //    {
        //        foreach (ModelMeshPart meshPart in mesh.MeshParts)
        //        {
        //            foreach (BasicEffect effect in mesh.Effects)
        //            {
        //                effect.EmissiveColor = new Vector3(1, 0, 0);
        //                //effect.LightingEnabled = true;
        //                effect.World = getModelContainer().getBoneTransform(mesh.ParentBone.Index) * getMatrix();
        //                effect.View = view;
        //                effect.Projection = proj;
        //            }
        //            //BoundingSphereRenderer.Render(entity.getBoundingSphere(), _graphicsDevice, view, proj, Color.Red);
        //            mesh.Draw();
        //        }
        //    }
        //}

        public int getDamage()
        {
            return _damage;
        }

        public void setDamage(int amountOfDamage)
        {
            _damage = amountOfDamage;
        }

        public void setSpeed(float speed){
            _speed = speed;
        }

        protected void setLifespan(float lifespan)
        {
            _timeTillDeath = lifespan;
        }

        protected void setScale(float scale)
        {
            _scale = scale;
        }

        float _timeTillDeath;
        float _curAngle;
        float _angleModifier;
        float _speed;
        float _scale;
        int _damage;
    }
}
