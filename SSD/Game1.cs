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

        // Set the position of the model in world space, and set the rotation.
        Matrix shipMatrix = Matrix.Identity;
        Matrix worldMatrix = Matrix.Identity;

        // Set the position of the camera in world space, for our view matrix.
        Matrix cameraPosition = Matrix.Identity;

        //Camera position
        Vector3 cameraThirdPerson = new Vector3(0, 200, -200);

        Vector3 camUp = Vector3.Left;

        Camera myCamera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
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

            //Load model
            _renderer.addModel("playerShip", "Models\\space_frigate");
            _renderer.addModel("worldSphere", "Models\\burning_planet");
            _renderer.addModel("spaceSphere", "Models\\space_sphere");
            _renderer.addModel("e_one", "Models\\e_one");
            _renderer.addModel("nebula", "Models\\nebula_bg");
            _renderer.addModel("bullet", "Models\\bullet");
            //_renderer.addModel("testPlane", "Models\\testPlane");

            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            _renderer.getModel("e_one").setScale(1f);
            Matrix shipPos = Matrix.CreateTranslation(0, 3500f, 0) * Matrix.CreateRotationZ(MathHelper.ToRadians(90));
            _renderer.getModel("e_one").setPosition(shipPos.Translation);

            _renderer.getModel("playerShip").setScale(5f);
            _renderer.getModel("playerShip").setPosition(0, 4500f, 0);
            //_renderer.getModel("playerShip").setYaw(90);

            _renderer.getModel("spaceSphere").setScale(100f);

            //_renderer.getModel("testPlane").setScale(4f);
            //_renderer.getModel("testPlane").setPosition(0, -1000f, 0);

            _renderer.getModel("worldSphere").setScale(40f); //40
            _renderer.getModel("worldSphere").addPitch(90);

            _renderer.getModel("nebula").setScale(10f); //40
            _renderer.getModel("nebula").setPosition(10000f, 4500f, 0);

            _renderer.getModel("bullet").setScale(1f); //40
            //_renderer.getModel("bullet").setPosition(0, 4500f, 0);


            myCamera = new Camera(_renderer.getModel("playerShip").getMatrix().Translation + _renderer.getModel("playerShip").getMatrix().Up * 1000,
                                  _renderer.getModel("playerShip").getMatrix().Translation,
                                  Vector3.Left/* _renderer.getModel("playerShip").getMatrix().Forward */);

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

                ModelContainer worldModel = _renderer.getModel("worldSphere");
                ModelContainer spaceSphere = _renderer.getModel("spaceSphere");
                ModelContainer playerShip = _renderer.getModel("playerShip");

                double angle = Math.Atan2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X);
                _renderer.getModel("playerShip").setYaw(MathHelper.ToDegrees(0));
                camUp = _renderer.getModel("playerShip").getMatrix().Left;
                _renderer.getModel("playerShip").setYaw((MathHelper.ToDegrees((float)angle)));


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
            _renderer.getModel("worldSphere").addRoll(0.1f);
            _renderer.getModel("worldSphere").addYaw(0.05f);

            _renderer.getModel("spaceSphere").addRoll(-0.1f);
            _renderer.getModel("spaceSphere").addYaw(-0.05f);

            //_renderer.getModel("e_one").addYaw(1);
            _renderer.getModel("e_one").applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(_renderer.getModel("e_one").getMatrix().Backward), 0.01f));
            _renderer.getModel("e_one").applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(_renderer.getModel("e_one").getMatrix().Left), 0.01f));
            //_renderer.getModel("e_one").applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(_renderer.getModel("e_one").getMatrix().Down), 0.01f));

            Quaternion cameraOriantaion = Quaternion.CreateFromRotationMatrix(_renderer.getModel("playerShip").getMatrix());
            //cameraPosition = Matrix.CreateTranslation(new Vector3(0, 1000, -1000) + _renderer.getModel("playerShip").getPosition());
            cameraPosition = Matrix.CreateTranslation(new Vector3(0, 1000, -1000) + _renderer.getModel("playerShip").getPosition());

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


            Vector3 cameraPosition = _renderer.getModel("playerShip").getMatrix().Translation + _renderer.getModel("playerShip").getMatrix().Up * 1000;
            Vector3 cameraTarget = _renderer.getModel("playerShip").getMatrix().Translation;

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
            _renderer.renderAllModels(view, proj);

            //Draw 2D things
            spriteBatch.Begin();
            //Draw nothing
            spriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
