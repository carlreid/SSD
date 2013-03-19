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
        }

        public void renderEntity(Matrix view, Matrix proj, Entity entity)
        {
            entity.draw(view, proj, _graphicsDevice);
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
        Dictionary<string, ModelContainer> _models;
    }
}
