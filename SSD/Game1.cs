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

using DPSF;
using DPSF.ParticleSystems;

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

        List<Bullet> _bullets = new List<Bullet>();

        // Declare our Particle System variable
        TrailParticleSystem shipExaustParticles = null;
        SmokeParticleSystem bulletSmokeParticles = null;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }

        private bool removeBullets(Bullet b){
            return !b.getAlive();
        }

        private Entity getEntity(String entityName)
        {
            try
            {
                Entity entity = null;
                _entities.TryGetValue(entityName, out entity);
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        //public static Vector3 MapToSphere(Vector3 coords, WorldMap MAP)
        //{
        //    float pi = 3.14159265f;
        //    float thetaDelta = ((2 * pi) / (MAP.XSize - 1));
        //    float phiDelta = ((pi) / (MAP.ZSize - .5f));
        //    float RadiusBase = ((MAP.XSize) / pi / 2f);
        //    float theta = (coords.Z * thetaDelta);
        //    float phi = (coords.X * phiDelta);

        //    //Limit the map to half a sphere
        //    if (theta > pi) { theta = theta - (pi); }

        //    if (theta < 0.0) { theta = theta + (pi); }

        //    if (phi > 2 * pi) { phi = phi - (2 * pi); }

        //    if (phi < 0.0) { phi = phi + (2 * pi); }

        //    Vector3 coords2 = new Vector3();
        //    coords2.X = (float)(((RadiusBase) * Math.Sin(theta) * Math.Cos(phi)) + MAP.XSize / 2f);
        //    coords2.Y = (float)((RadiusBase) * Math.Sin(theta) * Math.Sin(phi));
        //    coords2.Z = (float)(((RadiusBase) * Math.Cos(theta)) + MAP.ZSize / 2f);
        //    return coords2;
        //}

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

            BoundingSphereRenderer.InitializeGraphics(GraphicsDevice, 30);

            // Declare a new Particle System instance and Initialize it
            shipExaustParticles = new TrailParticleSystem(this);
            shipExaustParticles.AutoInitialize(this.GraphicsDevice, this.Content, null);

            bulletSmokeParticles = new SmokeParticleSystem(this);
            bulletSmokeParticles.AutoInitialize(this.GraphicsDevice, this.Content, null);
            bulletSmokeParticles.Emitter.EmitParticlesAutomatically = false;

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
            _renderer.addModel("playerShip", "Models\\player_ship");
            _renderer.addModel("worldSphere", "Models\\burning_planet");
            _renderer.addModel("spaceSphere", "Models\\space_sphere");
            _renderer.addModel("e_one", "Models\\e_one");
            _renderer.addModel("nebula", "Models\\nebula_bg");
            _renderer.addModel("bullet", "Models\\bullet");

            //Create entities
            _entities.Add("player", new Entity(new Vector3(0, 400f, 0), _renderer.getModel("playerShip"), 1f, 90));

            _entities.Add("e_one",    new Entity((Matrix.CreateTranslation(0, 1500f, 0) * Matrix.CreateRotationZ(MathHelper.ToRadians(90))).Translation, _renderer.getModel("e_one"), 1f));
            _entities.Add("universe", new Entity(Vector3.Zero, _renderer.getModel("spaceSphere"), 100f));
            _entities.Add("world",    new Entity(Vector3.Zero, _renderer.getModel("worldSphere"), 10f, 0, 90));

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            shipExaustParticles.Destroy();
            bulletSmokeParticles.Destroy();
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

                float angle = MathHelper.ToDegrees((float)Math.Atan2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X));
                getEntity("player").setYaw(0);
                camUp = getEntity("player").getMatrix().Left;
                getEntity("player").setYaw(angle);

                Debug.WriteLine(angle);

                playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * 0.01f));
                playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y * 0.01f));
            }

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.W) ||
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S) ||
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A) ||
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D))
            {
                Entity playerShip = getEntity("player");
                getEntity("player").setYaw(0);
                camUp = getEntity("player").getMatrix().Left;

                if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.W) && Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, 0.0075f));
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, -0.0075f));
                    getEntity("player").setYaw(135);
                }
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.W) && Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, 0.0075f));
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, 0.0075f));
                    getEntity("player").setYaw(45);
                } 
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S) && Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, -0.0075f));
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, -0.0075f));
                    getEntity("player").setYaw(-135);
                }
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S) && Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, -0.0075f));
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, 0.0075f));
                    getEntity("player").setYaw(-45);
                } 
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.W))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, 0.01f));
                    getEntity("player").setYaw(90);
                }
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, -0.01f));
                    getEntity("player").setYaw(-90);
                }
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, -0.01f));
                    getEntity("player").setYaw(180);
                }
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, 0.01f));
                    getEntity("player").setYaw(0);
                }
            }


            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X != 0 || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y != 0)
            {
                float angle = MathHelper.ToDegrees((float)Math.Atan2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y, GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X));

                float oldYaw = MathHelper.ToDegrees(getEntity("player").getYaw());
                getEntity("player").setYaw(0);
                Quaternion playerRotation = getEntity("player").getRotation();
                getEntity("player").setYaw(oldYaw);
                //playerRotation *= Quaternion.CreateFromAxisAngle(getEntity("player").getMatrix().Up, MathHelper.ToRadians(angle + 90));

                float calcYaw = /*-getEntity("player").getYaw() +*/ angle + 90;

                _bullets.Add(new Bullet(getEntity("player").getMatrix().Translation, playerRotation, calcYaw, _renderer.getModel("bullet")));
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                float angle = MathHelper.ToDegrees((float)Math.Atan2(Mouse.GetState().Y - graphics.PreferredBackBufferHeight / 2, Mouse.GetState().X - graphics.PreferredBackBufferWidth / 2));

                Debug.WriteLine(Mouse.GetState().X);

                //_bullets.Add(new Bullet(getEntity("player").getMatrix().Translation, angle + 90, _renderer.getModel("bullet")));
            }

            #endregion

            foreach (KeyValuePair<String, Entity> entity in _entities){
                entity.Value.update();
            }

            foreach (Bullet bul in _bullets)
            {
                bul.update(gameTime.ElapsedGameTime);
                bulletSmokeParticles.Emitter.PositionData.Position = bul.getMatrix().Translation;
                bulletSmokeParticles.AddParticle();
            }

            _bullets.RemoveAll(removeBullets);

            shipExaustParticles.Emitter.PositionData.Position = getEntity("player").getMatrix().Translation + getEntity("player").getMatrix().Backward * 10; ;

            getEntity("universe").addRoll(-0.001f);
            getEntity("universe").addYaw(-0.005f);
            getEntity("world").addRoll(0.02f);
            getEntity("world").addPitch(0.05f);

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
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Viewport viewport = graphics.GraphicsDevice.Viewport;
            float aspectRatio = viewport.AspectRatio;

            Vector3 cameraPosition = getEntity("player").getMatrix().Translation + getEntity("player").getMatrix().Up * 500;
            Vector3 cameraTarget = getEntity("player").getMatrix().Translation;
            camUp.Normalize();

            Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, camUp);
            Matrix proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 50000.0f);

            //Render all models that are loaded in.
            foreach(KeyValuePair<string, Entity> entity in _entities)
            {
                _renderer.renderEntity(view, proj, entity.Value);
            }

            foreach (Bullet b in _bullets)
            {
                _renderer.renderEntity(view, proj, b);
            }

            // Set the Particle System's World, View, and Projection matrices so that it knows how to draw the particles properly.
            shipExaustParticles.SetWorldViewProjectionMatrices(Matrix.Identity, view, proj);
            bulletSmokeParticles.SetWorldViewProjectionMatrices(Matrix.Identity, view, proj);

            // Update the Particle System
            shipExaustParticles.SetCameraPosition(cameraPosition);
            shipExaustParticles.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            bulletSmokeParticles.SetCameraPosition(cameraPosition);
            bulletSmokeParticles.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            shipExaustParticles.Draw();
            bulletSmokeParticles.Draw();
            

            //Draw 2D things
            spriteBatch.Begin();
            //Draw nothing
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
