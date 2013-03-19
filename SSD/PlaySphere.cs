using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class PlaySphere : Entity
    {
        public PlaySphere(Vector3 position, ModelContainer model, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0) : 
            base(position, model, scale, yaw, pitch, roll)
        {

        }

        public override void draw(Matrix view, Matrix proj, GraphicsDevice graphicsDevice)
        {
            RasterizerState originalState = graphicsDevice.RasterizerState;
            RasterizerState wireFrame = new RasterizerState();
            wireFrame.FillMode = FillMode.WireFrame;

            DepthStencilState originalDepth = graphicsDevice.DepthStencilState;
            DepthStencilState noDepth = new DepthStencilState();
            noDepth.DepthBufferEnable = false;
            

            //wireFrame.GraphicsDevice.DepthStencilState.DepthBufferEnable = false;
            graphicsDevice.DepthStencilState = noDepth;

            graphicsDevice.RasterizerState =  wireFrame;
            foreach (ModelMesh mesh in getModelContainer().getModel().Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.DiffuseColor = new Vector3(0.5f,1f,1f);
                        effect.Alpha = 0.05f;
                        effect.World = getModelContainer().getBoneTransform(mesh.ParentBone.Index) * getMatrix();
                        effect.View = view;
                        effect.Projection = proj;
                    }
                    //BoundingSphereRenderer.Render(entity.getBoundingSphere(), _graphicsDevice, view, proj, Color.Red);
                    mesh.Draw();
                }
            }
            graphicsDevice.RasterizerState = originalState;
            graphicsDevice.DepthStencilState = originalDepth;
        }

        //virtual public void update()
        //{
        //    //Do nothing
        //}
    }
}
