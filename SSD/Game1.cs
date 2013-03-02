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
using System.Diagnostics;

namespace SSD
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        float aspectRatio;

        Draw _renderer;
        Vector3 camUp = Vector3.Left;
        //Entity playerEntity;
        Dictionary<String, Entity> _entities = new Dictionary<string,Entity>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }

        private Entity getEntity(String entityName)
        {
            try
            {
                Entity entity;
                _entities.TryGetValue(entityName, out entity);
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.Title = "Super Stardust Clone";
            base.Initialize();

            //_renderer = new Draw(graphics.GraphicsDevice, Content);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Setup the game window
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();

            _renderer = new Draw(graphics.GraphicsDevice, Content);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            //Load models
            _renderer.addModel("playerShip", "Models\\space_frigate");
            _renderer.addModel("worldSphere", "Models\\burning_planet");
            _renderer.addModel("spaceSphere", "Models\\space_sphere");
            _renderer.addModel("e_one", "Models\\e_one");
            _renderer.addModel("nebula", "Models\\nebula_bg");
            _renderer.addModel("bullet", "Models\\bullet");
            //_renderer.addModel("testPlane", "Models\\testPlane");

            //Create entities
            _entities.Add("player", new Entity(new Vector3(0, 4500f, 0), _renderer.getModel("playerShip"), 5f, 90));
            _entities.Add("player2", new Entity(new Vector3(0, 4500f, 0), _renderer.getModel("playerShip"), 5f, 80, 10));
            _entities.Add("player3", new Entity(new Vector3(0, 4500f, 0), _renderer.getModel("playerShip"), 5f, 70, 20));
            _entities.Add("player4", new Entity(new Vector3(0, 4500f, 0), _renderer.getModel("playerShip"), 5f, 60, 30));
            _entities.Add("player5", new Entity(new Vector3(0, 4500f, 0), _renderer.getModel("playerShip"), 5f, 50, 40));
            _entities.Add("player6", new Entity(new Vector3(0, 4500f, 0), _renderer.getModel("playerShip"), 5f, 40, 50));
            _entities.Add("e_one",    new Entity((Matrix.CreateTranslation(0, 3500f, 0) * Matrix.CreateRotationZ(MathHelper.ToRadians(90))).Translation, _renderer.getModel("e_one"), 1f));
            _entities.Add("universe", new Entity(Vector3.Zero, _renderer.getModel("spaceSphere"), 100f));
            _entities.Add("world",    new Entity(Vector3.Zero, _renderer.getModel("worldSphere"), 40f, 0, 90));
            _entities.Add("nebula",   new Entity(new Vector3(10000f, 4500f, 0), _renderer.getModel("nebula"), 10f));

            ////_renderer.getModel("e_one").setScale(1f);
            ////Matrix shipPos = Matrix.CreateTranslation(0, 3500f, 0) * Matrix.CreateRotationZ(MathHelper.ToRadians(90));
            ////_renderer.getModel("e_one").setPosition(shipPos.Translation);

            ////_renderer.getModel("playerShip").setScale(5f);
            ////_renderer.getModel("playerShip").setPosition(0, 4500f, 0);
            //////_renderer.getModel("playerShip").setYaw(90);

            ////_renderer.getModel("spaceSphere").setScale(100f);

            //////_renderer.getModel("testPlane").setScale(4f);
            //////_renderer.getModel("testPlane").setPosition(0, -1000f, 0);

            ////_renderer.getModel("worldSphere").setScale(40f); //40
            ////_renderer.getModel("worldSphere").addPitch(90);

            ////_renderer.getModel("nebula").setScale(10f); //40
            ////_renderer.getModel("nebula").setPosition(10000f, 4500f, 0);

            ////_renderer.getModel("bullet").setScale(1f); //40
            //////_renderer.getModel("bullet").setPosition(0, 4500f, 0);




            //myCamera = new Camera(playerEntity.getMatrix().Translation + playerEntity.getMatrix().Up * 1000,
            //                      playerEntity.getMatrix().Translation,
            //                      Vector3.Left/* _renderer.getModel("playerShip").getMatrix().Forward */);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            #region Controls
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X != 0 || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y != 0)
            {

                Entity worldModel = getEntity("world");
                Entity spaceSphere = getEntity("universe");
                Entity playerShip = getEntity("player");

                double angle = Math.Atan2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X);
                getEntity("player").setYaw(MathHelper.ToDegrees(0));
                camUp = getEntity("player").getMatrix().Left;
                getEntity("player").setYaw((MathHelper.ToDegrees((float)angle)));


                playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * 0.01f));
                playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y * 0.01f));


            }

            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X != 0 || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y != 0)
            {
                double angle = Math.Atan2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * -1, GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * -1);
                //Console.WriteLine(angle);

                //_renderer.getModel("playerShip").setYaw((MathHelper.ToDegrees((float)angle) - 90));

            }

            #endregion

            //Rotate the planet
            getEntity("world").addRoll(0.1f);
            getEntity("world").addYaw(0.05f);

            getEntity("universe").addRoll(-0.1f);
            getEntity("universe").addYaw(-0.05f);

            //_renderer.getModel("e_one").addYaw(1);
            getEntity("e_one").applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(getEntity("e_one").getMatrix().Backward), 0.01f));
            getEntity("e_one").applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(getEntity("e_one").getMatrix().Left), 0.01f));

            getEntity("player2").applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(getEntity("player2").getMatrix().Left), 0.01f));
            getEntity("player3").applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(getEntity("player3").getMatrix().Left), 0.01f));
            getEntity("player4").applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(getEntity("player4").getMatrix().Left), 0.01f));
            getEntity("player5").applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(getEntity("player5").getMatrix().Left), 0.01f));
            getEntity("player6").applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(getEntity("player6").getMatrix().Left), 0.01f));

            //_renderer.getModel("e_one").applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(_renderer.getModel("e_one").getMatrix().Down), 0.01f));

            //Quaternion cameraOriantaion = Quaternion.CreateFromRotationMatrix(getEntity("player").getMatrix());
            //cameraPosition = Matrix.CreateTranslation(new Vector3(0, 1000, -1000) + _renderer.getModel("playerShip").getPosition());
            //cameraPosition = Matrix.CreateTranslation(new Vector3(0, 1000, -1000) + getEntity("player").getPosition());

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            /* DISABLE CULLING - Useful to see backwards triangles. 
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            //rs.FillMode = FillMode.WireFrame;
            graphics.GraphicsDevice.RasterizerState = rs;*/

            //Console.WriteLine(GraphicsDevice.Adapter.DeviceName.ToString());

            GraphicsDevice.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);

            // Copy any parent transforms. (From bones)

            //Matrix[] transforms = new Matrix[myModel.getModel().Bones.Count];
            //myModel.getModel().CopyAbsoluteBoneTransformsTo(transforms);            

            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //GraphicsDevice.BlendState = BlendState.AlphaBlend;

            //GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //GraphicsDevice.DepthStencilState = DepthStencilState.None;
            //GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            //GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            //GraphicsDevice.BlendState = BlendState.Opaque;
            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            Viewport viewport = graphics.GraphicsDevice.Viewport;
            float aspectRatio = viewport.AspectRatio;


            //Matrix view = Matrix.CreateLookAt(_renderer.getModel("playerShip").getMatrix().Translation + (_renderer.getModel("playerShip").getMatrix().Up * 500),
            //                      _renderer.getModel("playerShip").getMatrix().Translation, _renderer.getModel("playerShip").getMatrix().Left);

            Vector3 cameraPosition = getEntity("player").getMatrix().Translation + getEntity("player").getMatrix().Up * 1000;
            Vector3 cameraTarget = getEntity("player").getMatrix().Translation;

            //Vector3 cameraUp = _renderer.getModel("playerShip").getMatrix().Forward;

            //Vector3 cameraUp = new Vector3(-1, 0, 0);
            //Vector3 cameraUp = Vector3.Transform(cameraPosition - cameraTarget, Matrix.CreateRotationX(90));

            //Vector3 cameraUp = _renderer.getModel("playerShip").getMatrix().Forward;
            camUp.Normalize();

            Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, camUp);

            //Vector3 campos = cameraPosition.Translation;

            //Matrix view = Matrix.CreateLookAt(cameraPosition.Translation, _renderer.getModel("playerShip").getMatrix().Translation, new Vector3(1, 0, 0));

            //myCamera.setLookAt(_renderer.getModel("playerShip").getMatrix().Translation);
            //myCamera.setPosition(_renderer.getModel("playerShip").getMatrix().Translation + _renderer.getModel("playerShip").getMatrix().Up * 1000);

            //myCamera.Roll(0.0001f);
            //Matrix view = myCamera.getLookAt();

            Matrix proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 50000.0f);

            //Render all models that are loaded in.
            //_renderer.renderAllModels(view, proj);

            foreach(KeyValuePair<string, Entity> entity in _entities)
            {
                _renderer.renderEntity(view, proj, entity.Value);
            }

            //Draw 2D things
            spriteBatch.Begin();
            //Draw nothing
            spriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
