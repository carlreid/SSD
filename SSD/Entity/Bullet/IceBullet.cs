using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

using DPSF;
using DPSF.ParticleSystems;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class IceBullet : Bullet
    {
        public IceBullet(Vector3 position, Quaternion rotation, float yaw, ModelContainer bulletModel, float lifespan, float scale)
            : base(position, rotation, yaw, bulletModel)
        {
            base.setDamage(25);
            base.setLifespan(lifespan);
            //base.setScale(scale);
        }

        public override void draw(Matrix view, Matrix proj, GraphicsDevice graphicsDevice)
        {
            foreach (ModelMesh mesh in getModelContainer().getModel().Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EmissiveColor = new Vector3(0, 0, 1);
                        effect.World = getModelContainer().getBoneTransform(mesh.ParentBone.Index) * getMatrix();
                        effect.View = view;
                        effect.Projection = proj;
                    }
                    //BoundingSphereRenderer.Render(getBoundingSphere(), graphicsDevice, view, proj, Color.Red);
                    mesh.Draw();
                }
            }
        }

    }
}
