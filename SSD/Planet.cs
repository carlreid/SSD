using Microsoft.Xna.Framework;

namespace SSD
{
    class Planet : Entity
    {
        public Planet(Vector3 position, ModelContainer model, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0) : 
            base(position, model, scale, yaw, pitch, roll)
        {

        }

        //public override void draw(Matrix view, Matrix proj, Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice)
        //{
        //    base.draw(view, proj, graphicsDevice);
        //}

        //virtual public void update()
        //{
        //    //Do nothing
        //}
    }
}
