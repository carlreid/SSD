using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System;

namespace SSD
{
    class ModelEntity : Entity
    {
        public ModelEntity(Vector3 position, ModelContainer model, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0) : 
            base(position, model, scale, yaw, pitch, roll)
        {
            _boundingSphere = new BoundingSphere(position, scale);
            _isFriendly = true;
            _health = 1000;
        }

        public override void draw(Matrix view, Matrix proj, GraphicsDevice graphicsDevice)
        {
            //BoundingSphereRenderer.Render(getBoundingSphere(), graphicsDevice, view, proj, Color.Red);
            base.draw(view, proj, graphicsDevice);

        }

        override public void update(TimeSpan deltaTime)
        {
            foreach (ModelMesh mesh in getModelContainer().getModel().Meshes)
            {
                updateBoundingSphere(getMatrix().Translation, mesh.BoundingSphere.Radius * base.getScale());
            }
        }

        public void updateBoundingSphere(Vector3 position, float radius)
        {
            _boundingSphere.Radius = radius;
            _boundingSphere.Center = position;
        }

        public BoundingSphere getBoundingSphere()
        {
            return _boundingSphere;
        }

        public void setFriendly(bool isFriendly){
            _isFriendly = isFriendly;
        }

        public bool getFriendly()
        {
            return _isFriendly;
        }

        public void doDamage(int amountOfDamage)
        {
            _health -= amountOfDamage;
        }

        public int getHealth()
        {
            return _health;
        }

        public void setHealth(int newHealth)
        {
            _health = newHealth;
        }

        BoundingSphere _boundingSphere;
        bool _isFriendly;
        int _health;
    }
}
