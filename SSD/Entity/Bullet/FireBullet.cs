using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

using DPSF;
using DPSF.ParticleSystems;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class FireBullet : Bullet
    {
        public FireBullet(Vector3 position, Quaternion rotation, float yaw, ModelContainer bulletModel)
            : base(position, rotation, yaw, bulletModel)
        {
            base.setDamage(100);
        }

        public override void draw(Matrix view, Matrix proj, GraphicsDevice graphicsDevice)
        {
            foreach (ModelMesh mesh in getModelContainer().getModel().Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EmissiveColor = new Vector3(1, 0, 0);
                        //effect.LightingEnabled = true;
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
