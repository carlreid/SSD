using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SSD
{
    class Draw
    {

        public Draw(GraphicsDevice graphicsDevice, ContentManager content)
        {
            _graphicsDevice = graphicsDevice;
            _content = content;
            _models = new Dictionary<string,ModelContainer>();
            //Matrix view = Matrix.CreateLookAt(playerShip.getMatrix().Translation + (playerShip.getMatrix().Up * 100), playerShip.getMatrix().Translation, playerShip.getMatrix().Left);
            //Matrix proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 10000.0f);

        }

        public void render(ModelContainer model, Matrix view, Matrix proj)
        {
            foreach (ModelMesh mesh in model.getModel().Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.World = model.getBoneTransform(mesh.ParentBone.Index) * model.getMatrix();
                    effect.View = view;
                    effect.Projection = proj;
                }
                mesh.Draw();
            }
        }

        public void renderAllModels(Matrix view, Matrix proj)
        {
            foreach (KeyValuePair<string, ModelContainer> modelEntry in _models)
            {
                foreach (ModelMesh mesh in modelEntry.Value.getModel().Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;

                        effect.World = modelEntry.Value.getBoneTransform(mesh.ParentBone.Index) * modelEntry.Value.getMatrix();
                        effect.View = view;
                        effect.Projection = proj;
                    }
                    mesh.Draw();
                }
            }
        }

        public void addModel(String modelName, String filePath)
        {
            _models.Add(modelName, new ModelContainer(filePath, _content));
        }

        public ModelContainer getModel(String modelName)
        {
            try
            {
                ModelContainer model;
                _models.TryGetValue(modelName, out model);
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        GraphicsDevice _graphicsDevice;
        ContentManager _content;
        //List<ModelContainer> _models;
        Dictionary<string, ModelContainer> _models;
    }
}
