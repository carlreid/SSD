using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class Planet : Entity
    {
        public Planet(Vector3 position, ModelContainer model, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0) : 
            base(position, model, scale, yaw, pitch, roll)
        {

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
                        effect.SpecularPower = 100f;
                        effect.Alpha = 0.9f;

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

        //virtual public void update()
        //{
        //    //Do nothing
        //}
    }
}
