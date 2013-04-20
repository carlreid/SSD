using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class EnemyEntity : ModelEntity
    {
        public EnemyEntity(Vector3 position, ModelContainer model, bool isIceEnemy, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0)
            : base(position, model, scale, yaw, pitch, roll)
        {
            base.setFriendly(false);
            _isSpawning = true;
            base.setHealth(300); //Move to level down?
            _isIceEnemy = isIceEnemy;
            _myScore = 1000;
        }

        override public void draw(Matrix view, Matrix proj, GraphicsDevice graphicsDevice)
        {
            foreach (ModelMesh mesh in getModelContainer().getModel().Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        if (_isIceEnemy)
                        {
                            effect.DiffuseColor = Color.Cyan.ToVector3();
                            effect.EmissiveColor = Color.Cyan.ToVector3();
                        }
                        else
                        {
                            effect.DiffuseColor = Color.Red.ToVector3();
                            effect.EmissiveColor = Color.Red.ToVector3();
                        }
                        effect.World = getModelContainer().getBoneTransform(mesh.ParentBone.Index) * getMatrix();
                        effect.View = view;
                        effect.Projection = proj;
                    }
                    //BoundingSphereRenderer.Render(getBoundingSphere(), graphicsDevice, view, proj, Color.Red);
                    mesh.Draw();
                }
            }
        }

        //override public Matrix getMatrix()
        //{
        //    Matrix transform = Matrix.Identity;

        //    Quaternion accumulateRotation = base.getRotation();

        //    transform *= Matrix.CreateScale(base.getScale());
        //    transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(this.getYaw(), this.getPitch(), this.getRoll()));
        //    transform *= Matrix.CreateTranslation(base.getPosition());
        //    transform *= Matrix.CreateFromQuaternion(this.getRawRotation());
        //    //transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(base.getMatrix().Forward, MathHelper.ToRadians(0.1f)));

        //    //setPosition(transform.Translation);
        //    //addRotation(Quaternion.CreateFromAxisAngle(base.getMatrix().Forward, MathHelper.ToRadians(0.1f)));

        //    return transform;
        //}

        //override public void update(TimeSpan deltaTime)
        //{
        //    base.update(deltaTime);
        //}

        public void setIsSpawning(bool isSpawning)
        {
            _isSpawning = isSpawning;
        }

        public bool isIceEnemy()
        {
            return _isIceEnemy;
        }

        public int getScore()
        {
            return _myScore;
        }

        protected bool _isSpawning;
        protected bool _isIceEnemy;
        protected int  _myScore;

    }
}
