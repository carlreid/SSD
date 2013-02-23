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

        //ModelContainer playerShip;
        //ModelContainer worldModel;
        //ModelContainer spaceSphere;

        float aspectRatio;
        //float playerSpeed;

        Draw _renderer;
        

        //ModelContainer myModel;

        // Set the position of the model in world space, and set the rotation.
        //Matrix shipMatrix = Matrix.Identity;
        //Matrix worldMatrix = Matrix.Identity;

        // Set the position of the camera in world space, for our view matrix.
        Matrix cameraPosition = Matrix.Identity;

        //Camera position
        Vector3 cameraThirdPerson = new Vector3(0, 200, -200);

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
            //_renderer.addModel("worldSphere", "Models\\burning_planet");
            _renderer.addModel("spaceSphere", "Models\\space_sphere");
            _renderer.addModel("e_one", "Models\\e_one");
            _renderer.addModel("testPlane", "Models\\testPlane");

            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            _renderer.getModel("e_one").setScale(1f);
            _renderer.getModel("e_one").setPosition(0, 3500f, 0);

            _renderer.getModel("playerShip").setScale(10f);
            //_renderer.getModel("playerShip").setPosition(0, 6000f, 0);

            _renderer.getModel("spaceSphere").setScale(100f);

            _renderer.getModel("testPlane").setScale(4f);
            _renderer.getModel("testPlane").setPosition(0, -1000f, 0);


            //_renderer.getModel("worldSphere").setScale(40f); //40
            //_renderer.getModel("worldSphere").addPitch(90);
            

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


                //spaceSphere.applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(playerShip.getMatrix().Right), (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * -0.1f) / 50));
                //spaceSphere.applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(playerShip.getMatrix().Forward), (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y * -0.1f) / 50));

                playerShip.applyMatrix(Matrix.CreateTranslation(new Vector3(10 * GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y,
                                                                            0,
                                                                            10 * GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X)
                                                                ));

            }

            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X != 0 || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y != 0)
            {
                //double angle = Math.Atan2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * -1, GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * -1);
                //Console.WriteLine(angle);

                //_renderer.getModel("playerShip").setYaw((MathHelper.ToDegrees((float)angle) - 90));

                _renderer.getModel("playerShip").addYaw(5 * GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X);
                _renderer.getModel("playerShip").addRoll(5 * GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y);

            }

            if (GamePad.GetState(PlayerIndex.One).Triggers.Left != 0 || GamePad.GetState(PlayerIndex.One).Triggers.Right != 0)
            {
                _renderer.getModel("playerShip").applyMatrix(Matrix.CreateTranslation(0,
                                                                                      -10 * GamePad.GetState(PlayerIndex.One).Triggers.Left + 10 * GamePad.GetState(PlayerIndex.One).Triggers.Right, 
                                                                                      0));
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder != 0 || GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder != 0)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    _renderer.getModel("playerShip").addPitch(5f);
                }
                else
                {
                    _renderer.getModel("playerShip").addPitch(-5f);
                }
            }


            #endregion

            //modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.1f);

            //Rotate the planet
            //worldMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(0.1f));
            //worldMatrix *= Matrix.CreateRotationZ(MathHelper.ToRadians(0.05f));

            //_renderer.getModel("worldSphere").addRoll(0.1f);
            //_renderer.getModel("worldSphere").addYaw(0.05f);

            _renderer.getModel("spaceSphere").addRoll(-0.1f);
            _renderer.getModel("spaceSphere").addYaw(-0.05f);

            _renderer.getModel("e_one").addYaw(1);
            _renderer.getModel("e_one").applyMatrix(Matrix.CreateFromAxisAngle(Vector3.Normalize(_renderer.getModel("e_one").getMatrix().Backward), 0.01f));


            //playerShip.addYaw(1);

            Quaternion cameraOriantaion = Quaternion.CreateFromRotationMatrix(_renderer.getModel("playerShip").getMatrix());
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

            Viewport viewport = graphics.GraphicsDevice.Viewport;
            float aspectRatio = viewport.AspectRatio;

            //Matrix view = Matrix.CreateLookAt(playerShip.getMatrix().Translation + (playerShip.getMatrix().Up * 500), playerShip.getMatrix().Translation, playerShip.getMatrix().Left);
            Matrix view = Matrix.CreateLookAt(_renderer.getModel("playerShip").getMatrix().Translation + (_renderer.getModel("playerShip").getMatrix().Up * 500),
                                                _renderer.getModel("playerShip").getMatrix().Translation, new Vector3(1, 0, 0));
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
