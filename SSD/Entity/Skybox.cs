using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class Skybox : Entity
    {
        public Skybox(Vector3 position, ModelContainer model, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0) : 
            base(position, model, scale, yaw, pitch, roll)
        {

        }

        //virtual public void update()
        //{
        //    //Do nothing
        //}

        public void draw(Matrix view, Matrix proj)
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
                    }
                    //BoundingSphereRenderer.Render(entity.getBoundingSphere(), _graphicsDevice, view, proj, Color.Red);
                    mesh.Draw();
                }
            }
        }
    }
}
