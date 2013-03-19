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
        SpriteFont _hudFont;

        SoundManager _soundManager;
        BloomComponent bloom;

        float aspectRatio;
        float _lastBulletShot = 0;

        Draw _renderer;
        Vector3 camUp = Vector3.Left;
        //Entity playerEntity;

        PlayerEntity _playerOne;
        Entity _universe;
        Entity _planet;
        Entity _playSphere;

        List<Entity> _worldEntities = new List<Entity>();

        //Dictionary<String, Entity> _entities = new Dictionary<string,Entity>();
        //List<Bullet> _bullets = new List<Bullet>();

        // Declare our Particle System variable
        TrailParticleSystem shipExaustParticles = null;
        SmokeParticleSystem bulletSmokeParticles = null;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }

        private bool removeEntity(Entity e){
            return !e.getAlive();
        }

        //private Entity getEntity(String entityName)
        //{
        //    try
        //    {
        //        Entity entity = null;
        //        _entities.TryGetValue(entityName, out entity);
        //        return entity;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        throw;
        //    }
        //}

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

            //graphics = new GraphicsDeviceManager(this);

            //Setup the game window
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();

            bloom = new BloomComponent(this);
            bloom.Settings = BloomSettings.PresetSettings[0]; //3
            bloom.Visible = true;

            Components.Add(bloom);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _hudFont = Content.Load<SpriteFont>("hudFont");

            BoundingSphereRenderer.InitializeGraphics(GraphicsDevice, 30);

            // Declare a new Particle System instance and Initialize it
            shipExaustParticles = new TrailParticleSystem(this);
            shipExaustParticles.AutoInitialize(this.GraphicsDevice, this.Content, null);

            bulletSmokeParticles = new SmokeParticleSystem(this);
            bulletSmokeParticles.AutoInitialize(this.GraphicsDevice, this.Content, null);
            bulletSmokeParticles.Emitter.EmitParticlesAutomatically = false;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);



            //_missileSound = Content.Load<SoundEffect>("missile_sound");
            //_missileSoundEffectInstance = _missileSound.CreateInstance();
            //_missileSoundEffectInstance.Apply3D(listener, emitter);
            //_missileSoundEffectInstance.IsLooped = true; //For testing
            //_missileSoundEffectInstance.Play();

            _renderer = new Draw(graphics.GraphicsDevice, Content);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            //Load models
            _renderer.addModel("playerShip", "Models\\player_ship");
            _renderer.addModel("worldSphere", "Models\\burning_planet");
            _renderer.addModel("spaceSphere", "Models\\space_sphere");
            _renderer.addModel("e_one", "Models\\e_one");
            _renderer.addModel("nebula", "Models\\nebula_bg");
            _renderer.addModel("bullet", "Models\\bullet");
            _renderer.addModel("playSphere", "Models\\play_sphere");

            //Create entities
            _worldEntities.Add(new PlayerEntity(new Vector3(0, 400f, 0), _renderer.getModel("playerShip"), 1f, 90));
            _playerOne = (PlayerEntity)_worldEntities[_worldEntities.Count - 1];

            _worldEntities.Add(new Planet(Vector3.Zero, _renderer.getModel("worldSphere"), 8f, 0, 90));
            _planet = _worldEntities[_worldEntities.Count - 1];

            _worldEntities.Add(new Skybox(Vector3.Zero, _renderer.getModel("spaceSphere"), 100f));
            _universe = _worldEntities[_worldEntities.Count - 1];

            _worldEntities.Add(new PlaySphere(Vector3.Zero, _renderer.getModel("playSphere"), 15f));
            _playSphere = _worldEntities[_worldEntities.Count - 1];

            _worldEntities.Add(new EnemyEntity((Matrix.CreateTranslation(0, 400f, 0) * Matrix.CreateRotationZ(MathHelper.ToRadians(90))).Translation, _renderer.getModel("e_one"), 0.1f));

            //Setup audio
            _soundManager = new SoundManager(Content, _playerOne);

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

            if (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
            {
                _playerOne.useBoost();
            }

            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X != 0 || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y != 0)
            {
                float angle = MathHelper.ToDegrees((float)Math.Atan2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X));
                _playerOne.setYaw(0);
                camUp = _playerOne.getMatrix().Left;
                _playerOne.setYaw(angle);

                //Debug.WriteLine(angle);

                _playerOne.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * _playerOne.getSpeed()));
                _playerOne.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y * _playerOne.getSpeed()));
            }

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.W) ||
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S) ||
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A) ||
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D))
            {
                Entity playerShip = _playerOne;
                _playerOne.setYaw(0);
                camUp = _playerOne.getMatrix().Left;

                if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.W) && Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, 0.0075f));
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, -0.0075f));
                    _playerOne.setYaw(135);
                }
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.W) && Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, 0.0075f));
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, 0.0075f));
                    _playerOne.setYaw(45);
                } 
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S) && Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, -0.0075f));
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, -0.0075f));
                    _playerOne.setYaw(-135);
                }
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S) && Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, -0.0075f));
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, 0.0075f));
                    _playerOne.setYaw(-45);
                } 
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.W))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, 0.01f));
                    _playerOne.setYaw(90);
                }
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, -0.01f));
                    _playerOne.setYaw(-90);
                }
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.A))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, -0.01f));
                    _playerOne.setYaw(180);
                }
                else if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.D))
                {
                    playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, 0.01f));
                    _playerOne.setYaw(0);
                }
            }


            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X != 0 || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y != 0)
            {
                if (_lastBulletShot <= 0)
                {
                    float angle = MathHelper.ToDegrees((float)Math.Atan2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y, GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X));

                    float oldYaw = MathHelper.ToDegrees(_playerOne.getYaw());
                    _playerOne.setYaw(0);
                    Quaternion playerRotation = _playerOne.getRotation();
                    _playerOne.setYaw(oldYaw);
                    //playerRotation *= Quaternion.CreateFromAxisAngle(getEntity("player").getMatrix().Up, MathHelper.ToRadians(angle + 90));

                    float calcYaw = /*-getEntity("player").getYaw() +*/ angle + 90;

                    Bullet newBullet = new Bullet(_playerOne.getMatrix().Translation, playerRotation, calcYaw, _renderer.getModel("bullet"));
                    _worldEntities.Add(newBullet);
                    _soundManager.addAttatchment(LoadedSounds.ROCKET_SOUND, newBullet);
                    _lastBulletShot = 150;
                }
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                float angle = MathHelper.ToDegrees((float)Math.Atan2(Mouse.GetState().Y - graphics.PreferredBackBufferHeight / 2, Mouse.GetState().X - graphics.PreferredBackBufferWidth / 2));

                //Debug.WriteLine(Mouse.GetState().X);

                //_bullets.Add(new Bullet(getEntity("player").getMatrix().Translation, angle + 90, _renderer.getModel("bullet")));
            }

            #endregion

            foreach (Entity entity in _worldEntities){
                entity.update(gameTime.ElapsedGameTime);
                if (entity is Bullet)
                {
                    bulletSmokeParticles.Emitter.PositionData.Position = entity.getMatrix().Translation;
                    bulletSmokeParticles.AddParticle();
                }
            }

            //Check for collision, maybe move elsewhere
            for (int entity = 0; entity < _worldEntities.Count; ++entity)
            {
                //Check entity is a ModelEntity (Only these have bounding spheres)
                if (_worldEntities[entity] is ModelEntity)
                {
                    //As we know it is a ModelEntity, cast it to get access to functions.
                    ModelEntity currentEntity = (ModelEntity)_worldEntities[entity];

                    //Loop over the rest of the entities to check against
                    for (int nextEntity = entity + 1; nextEntity < _worldEntities.Count; ++nextEntity)
                    {
                        //Check to see if this entity is also a ModelEntity
                        if (_worldEntities[nextEntity] is ModelEntity)
                        {
                            //Cast to get functionality
                            ModelEntity checkEntity = (ModelEntity)_worldEntities[nextEntity];

                            //Do a check to see if either have been flagged dead as they may have collided so no point re-checking.
                            if (currentEntity.getAlive() == false)
                            {
                                break; //Quit this inner for loop, no point checking the rest.
                            }
                            if (checkEntity.getAlive() == false)
                            {
                                continue; //Continue to next entity in this loop.
                            }

                            //Check to see if the entity we're chacking is hostile (is an enemy..)
                            if (currentEntity.getFriendly() == false || checkEntity.getFriendly() == false)
                            {
                                //Check for sphere intersects.
                                if (currentEntity.getBoundingSphere().Intersects(checkEntity.getBoundingSphere()))
                                {
                                    //Debug.WriteLine("Collision");

                                    //Check for ship collision. If there is one, mark both dead.
                                    if ((currentEntity is PlayerEntity && checkEntity is EnemyEntity) ||
                                        (checkEntity is PlayerEntity && currentEntity is EnemyEntity))
                                    {
                                        currentEntity.doDamage(currentEntity.getHealth());
                                        currentEntity.setAlive(false);
                                        checkEntity.doDamage(checkEntity.getHealth());
                                        checkEntity.setAlive(false);
                                        continue; //Do not check if bullets, this will be game over/remove life.
                                    }

                                    //Check to see if either entity is a bullet. If so, apply bullet damage.
                                    if (currentEntity is Bullet)
                                    {
                                        checkEntity.doDamage(((Bullet)currentEntity).getDamage());
                                        currentEntity.setAlive(false);
                                    }
                                    if (checkEntity is Bullet)
                                    {
                                        currentEntity.doDamage(((Bullet)checkEntity).getDamage());
                                        checkEntity.setAlive(false);
                                    }

                                    //Check to see if the entity health is below 0, if so, mark dead.
                                    if (currentEntity.getHealth() < 0)
                                    {
                                        currentEntity.setAlive(false);
                                    }
                                    if (checkEntity.getHealth() < 0)
                                    {
                                        checkEntity.setAlive(false);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }

            _worldEntities.RemoveAll(removeEntity);

            shipExaustParticles.Emitter.PositionData.Position = _playerOne.getMatrix().Translation + _playerOne.getMatrix().Backward * 10; ;

            _universe.addRoll(-0.001f);
            _universe.addYaw(-0.005f);
            _planet.addRoll(0.02f);
            _planet.addPitch(0.05f);

            _soundManager.update();

            if (_lastBulletShot > 0)
            {
                _lastBulletShot -= gameTime.ElapsedGameTime.Milliseconds;
            }

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

            bloom.BeginDraw();

            GraphicsDevice.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //graphics.GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
            //graphics.GraphicsDevice.DepthStencilState.DepthBufferWriteEnable = true;

            Viewport viewport = graphics.GraphicsDevice.Viewport;
            float aspectRatio = viewport.AspectRatio;

            //Vector3 cameraPosition = _playerOne.getMatrix().Translation + _playerOne.getMatrix().Left * 500;
            Vector3 cameraPosition = _playerOne.getMatrix().Translation + _playerOne.getMatrix().Up * 500;
            Vector3 cameraTarget = _playerOne.getMatrix().Translation;
            camUp.Normalize();

            Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, camUp);
            Matrix proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 50000.0f);

            //Render all models that are loaded in.


            foreach(Entity entity in _worldEntities)
            {
                _renderer.renderEntity(view, proj, entity);
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
            
            base.Draw(gameTime);

            //Draw 2D things - AFTER the bloom
            spriteBatch.Begin();
            spriteBatch.DrawString(_hudFont, "Player Health: " + ((ModelEntity)_playerOne).getHealth(), new Vector2(0, 0), Color.Green);
            spriteBatch.End();


            
        }
    }
}
