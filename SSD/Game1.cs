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
        Random _randomGen = new Random();

        //Fonts and GUI
        FontFile _fontFile;
        Texture2D _fontTexture;
        FontRenderer _fontRenderer;
        Texture2D _menuBackground;
        Texture2D _healthShieldBackground;
        Texture2D _healthShieldBar;
        Texture2D _healthShieldFrame;
        Texture2D _xboxA;
        Texture2D _xboxB;
        Texture2D _gameControls;
        Texture2D _boostOverlay = null;
        Vector2 _boostOverlayPosition;
        Texture2D _boostFlame = null;
        Vector2 _boostFlamePosition;
        Texture2D _lifeIcon = null;
        Vector2 _lifeIconPosition;
        Texture2D _bombIcon = null;
        Vector2 _bombIconPosition;

        //Managers and Bloom
        SoundManager _soundManager;
        FPSManager _fpsManager = new FPSManager();
        SpawnManager _spawnManager;
        BloomComponent bloom;
        GameLevel _currentLevel;
        DifficultyManager _difficultyManager = new DifficultyManager();

        //Game related variables
        const float BLAST_RADIUS = 250.0f;
        float aspectRatio;
        float _lastBulletShot = 0;
        int _currentScore;

        //Drawing variables
        Draw _renderer;
        Vector3 camUp = Vector3.Left;

        //References to entities that exisit in all levels
        PlayerEntity _playerOne;
        Entity _universe;
        Entity _planet;
        Entity _playSphere;

        //Reserve a list of 150 entities
        List<Entity> _worldEntities = new List<Entity>(150);

        //Just to check draw and update count
        int drawCount = 0;
        int updateCount = 0;

        //Variables to keep track of menu status
        bool _isInMenus = true;
        Menu _currentMenu;

        //Create all the particle systems
        //Could have possibly moved all this into a particle manager.
        ParticleSystemManager _particleSystemManager = null;
        TrailParticleSystem _shipExaustParticles = null;
        BoostParticleSystem _shipBoostParticles = null;
        BoostGlowParticleSystem _shipBoostGlowParticles = null;
        //SmokeParticleSystem _bulletSmokeParticles = null;
        FireBulletParticleSystem _bulletFireParticles = null;
        IceBulletParticleSystem _bulletIceParticles = null;
        ExplosionRockParticleSystem _rockExplodeParticles = null;
        ExplosionMineParticleSystem _mineExplodeParticles = null;
        ShipExplodeParticleSystem _shipExplodeParticles = null;
        ShipBombExplodeParticleSystem _shipBombExplodeParticles = null;
        //PowerUpParticleSystem _powerUpParticles = null;
        //PushPullParticleSystem _pushPullParticles = null;

        //Input States to keep backup
        GamePadState _controllerState;
        KeyboardState _keyboardState;
        MouseState _mouseState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        //Call this when wanting to restart the game or load a new level
        public void restartGame(int levelToLoad)
        {
            //Currently only one level
            switch(levelToLoad){
                case 1:
                    _currentLevel = new LaveLevel(_spawnManager, _randomGen, _difficultyManager);
                    break;
                default:
                    break;
            }

            //Clear the world entities
            _worldEntities.Clear();

            //Add the reference entities
            _worldEntities.Add(new PlayerEntity(new Vector3(0, 400f, 0), _renderer.getModel("playerShip"), 1f, 90));
            _playerOne = (PlayerEntity)_worldEntities[_worldEntities.Count - 1];

            _worldEntities.Add(new Planet(Vector3.Zero, _renderer.getModel(_currentLevel.getPlanetModelString()), 8f, 0, 90));
            _planet = _worldEntities[_worldEntities.Count - 1];

            _worldEntities.Add(new Skybox(Vector3.Zero, _renderer.getModel("spaceSphere"), 100f));
            _universe = _worldEntities[_worldEntities.Count - 1];

            _worldEntities.Add(new PlaySphere(Vector3.Zero, _renderer.getModel("playSphere"), 15f));
            _playSphere = _worldEntities[_worldEntities.Count - 1];

            //Reset the sound manager
            _soundManager.stopMusic();
            _soundManager.reset(_playerOne);

            //Reload the particle systems
            loadParticleSystems();

            //Reset camera and score
            camUp = Vector3.Left;
            _currentScore = 0;
        }

        //Call this when checking to remove entities
        private bool removeEntity(Entity e)
        {
            //Special case when play dies
            if (e is PlayerEntity)
            {
                if (_playerOne.getHealth() <= 0)
                {
                    _playerOne.removeLife();
                    _playerOne.setHealth(1000);
                    _playerOne.setInDeathCooldown(true);
                    _soundManager.playDeathSound(_playerOne);

                    //Cause massive explosion.
                    _shipExplodeParticles.Emitter.PositionData.Position = _playerOne.getMatrix().Translation;
                    _shipExplodeParticles.ExplosionIntensity = 2000;
                    _shipExplodeParticles.Explode();

                    //If the player is out of lives show game over
                    if (_playerOne.getLives() < 0)
                    {
                        _playerOne.setAlive(false);
                        _currentMenu = new GameOverMenu(this, _currentMenu, graphics.GraphicsDevice.Viewport);
                        _isInMenus = true;
                        _soundManager.pauseMusic();
                        _soundManager.pauseEffects();
                    }
                    else
                    {
                        //Otherwise make them alive again
                        _playerOne.setAlive(true);
                    }

                    //Now check to see if entities are within BLAST_RADIUS
                    _worldEntities.ForEach(delegate(Entity entity)
                    {
                        if (entity is EnemyEntity)
                        {
                            //Debug.WriteLine((entity.getMatrix().Translation - _playerOne.getMatrix().Translation).Length());
                            if ((entity.getMatrix().Translation - _playerOne.getMatrix().Translation).Length() < BLAST_RADIUS)
                            {
                                if (!(entity is EnemyIceBoss))
                                {
                                    //Entity is within blast radius, kill it.
                                    entity.setAlive(false);
                                }
                            }
                        }
                    });

                    return !e.getAlive();
                }
            }

            if (!e.getAlive())
            {
                //Get the sound manager to play the entity death sound
                _soundManager.playDeathSound(e);

                if (e is EnemyEntity)
                {
                    //Add the score from the entity
                    int scoreToAdd = (int)(((EnemyEntity)e).getScore() * _playerOne.getScoreMultiplier());
                    _renderer.addScore(e.getMatrix(), scoreToAdd);
                    _currentScore += scoreToAdd;

                    //Spawn a power up if lucky!
                    //Calculate a nice sport for the power up to be at. If you use the middle of an entity, if it's too large the player can't pick up!
                    //Calculate direction vector
                    Vector3 directionToPlanet = _planet.getMatrix().Translation - e.getMatrix().Translation;
                    directionToPlanet.Normalize();

                    //Initial spawn vector
                    Vector3 spawnPoint = e.getMatrix().Translation + directionToPlanet;

                    //Get player's bounding sphere, we'll know that they can reach it then.
                    BoundingSphere playerBoundingSize = _playerOne.getBoundingSphere();
                    playerBoundingSize.Center = spawnPoint;

                    //Keep moving till intersecting with the play surface
                    while (!playerBoundingSize.Intersects(((PlaySphere)_playSphere).getBoundingSphere()))
                    {
                        playerBoundingSize.Center += directionToPlanet;
                    }

                    //Set the spawnPoint to the new value and use below.
                    spawnPoint = playerBoundingSize.Center;

                    switch (_randomGen.Next(0, (int)(100 * _difficultyManager._powerUpChance)))
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            _worldEntities.Add(new SpeedUpPU(spawnPoint, _renderer.getModel("speedPowerUp")));
                            break;
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                            _worldEntities.Add(new SlowDownPU(spawnPoint, _renderer.getModel("slowPowerUp")));
                            break;
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                            _worldEntities.Add(new BulletSpeedPU(spawnPoint, _renderer.getModel("ammoPowerUp")));
                            break;
                        case 15:
                        case 16:
                        case 17:
                        case 18:
                        case 19:
                            _worldEntities.Add(new BombPU(spawnPoint, _renderer.getModel("bombPowerUp")));
                            break;
                        case 20:
                            _worldEntities.Add(new LifePU(spawnPoint, _renderer.getModel("lifePowerUp")));
                            break;
                        default:
                            _worldEntities.Add(new MultiplierPU(spawnPoint, _renderer.getModel("multiplierPowerUp")));
                            break;
                    }

                    //Spawn particles for dead entity
                    if (e is EnemyRock)
                    {
                        _rockExplodeParticles.Emitter.PositionData.Position = e.getMatrix().Translation;
                        _rockExplodeParticles.AddParticles(30);
                        //_rockExplodeParticles.ExplosionIntensity = 60;
                        //_rockExplodeParticles.Explode();
                    }
                    else if (e is EnemyMine)
                    {
                        _mineExplodeParticles.Emitter.PositionData.Position = e.getMatrix().Translation;
                        _mineExplodeParticles.AddParticles(30);
                        //_mineExplodeParticles.ExplosionIntensity = 60;
                        //_mineExplodeParticles.Explode();
                    }
                }
            }
            return !e.getAlive();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Window.Title = "Cosmic Dust";
            this.IsMouseVisible = true;

            //Setup the game window
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();

            //Start up the bloom component
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
            //Load in all the GUI textures and fonts
            _hudFont = Content.Load<SpriteFont>("hudFont");
            _boostOverlay = Content.Load<Texture2D>("GUI/boost_overlay");
            _boostOverlayPosition = new Vector2(10, graphics.GraphicsDevice.Viewport.Height - (_boostOverlay.Height + 20));
            _boostFlame = Content.Load<Texture2D>("GUI/boost_flame");
            _boostFlamePosition = new Vector2(22, graphics.GraphicsDevice.Viewport.Height - (_boostFlame.Height + 30));
            _lifeIcon = Content.Load<Texture2D>("GUI/lifeIcon");
            _lifeIconPosition = new Vector2(10, 10);
            _bombIcon = Content.Load<Texture2D>("GUI/bombIcon");
            _bombIconPosition = new Vector2(10, 85);
            _menuBackground = Content.Load<Texture2D>("GUI/menuBackground");
            _gameControls = Content.Load<Texture2D>("GUI/controls");
            _fontFile = FontLoader.Load("Content/GUI/ui_font.fnt");
            _fontTexture = Content.Load<Texture2D>("GUI/ui_font_0");
            _fontRenderer = new FontRenderer(_fontFile, _fontTexture);
            _xboxA = Content.Load<Texture2D>("GUI/xboxControllerButtonA");
            _xboxB = Content.Load<Texture2D>("GUI/xboxControllerButtonB");
            _healthShieldBackground = Content.Load<Texture2D>("GUI/health_shield_background");
            _healthShieldBar = Content.Load<Texture2D>("GUI/health_shield_bar");
            _healthShieldFrame = Content.Load<Texture2D>("GUI/health_shield_frame");

            //Start up the bounding sphere draw
            BoundingSphereRenderer.InitializeGraphics(GraphicsDevice, 30);

            //Load particles
            loadParticleSystems();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _renderer = new Draw(graphics.GraphicsDevice, Content);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            //Load models
            _renderer.addModel("playerShip", "Models\\player_ship");
            _renderer.addModel("worldSphere", "Models\\burning_planet");
            _renderer.addModel("spaceSphere", "Models\\space_sphere");
            _renderer.addModel("e_rock", "Models\\asteroid1");
            _renderer.addModel("e_mine", "Models\\mine");
            _renderer.addModel("e_pushPull", "Models\\e_pushPull");
            _renderer.addModel("e_iceBossHead", "Models\\e_iceBossHead");
            _renderer.addModel("e_iceBossTail", "Models\\e_iceBossTail");
            _renderer.addModel("nebula", "Models\\nebula_bg");
            _renderer.addModel("iceBullet", "Models\\iceBullet");
            _renderer.addModel("fireBullet", "Models\\fireBullet");
            _renderer.addModel("playSphere", "Models\\play_sphere");
            _renderer.addModel("speedPowerUp", "Models\\pu_speed");
            _renderer.addModel("slowPowerUp", "Models\\pu_slow");
            _renderer.addModel("ammoPowerUp", "Models\\pu_ammo");
            _renderer.addModel("lifePowerUp", "Models\\pu_health");
            _renderer.addModel("shieldPowerUp", "Models\\pu_shield");
            _renderer.addModel("bombPowerUp", "Models\\pu_bomb");
            _renderer.addModel("multiplierPowerUp", "Models\\pu_multiplier");

            //Create entities
            _worldEntities.Add(new PlayerEntity(new Vector3(0, 400f, 0), _renderer.getModel("playerShip"), 1f, 90));
            _playerOne = (PlayerEntity)_worldEntities[_worldEntities.Count - 1];

            _worldEntities.Add(new Planet(Vector3.Zero, _renderer.getModel("worldSphere"), 8f, 0, 90));
            _planet = _worldEntities[_worldEntities.Count - 1];

            _worldEntities.Add(new Skybox(Vector3.Zero, _renderer.getModel("spaceSphere"), 100f));
            _universe = _worldEntities[_worldEntities.Count - 1];

            _worldEntities.Add(new PlaySphere(Vector3.Zero, _renderer.getModel("playSphere"), 15f));
            _playSphere = _worldEntities[_worldEntities.Count - 1];

            //Setup managers
            _soundManager = new SoundManager(Content, _playerOne);
            _spawnManager = new SpawnManager(ref _worldEntities, ref _renderer, ref _soundManager);

            //Setup Menu
            _currentMenu = new MainMenu(this, null, graphics.GraphicsDevice.Viewport);

            //Start level
            _currentLevel = new LaveLevel(_spawnManager, _randomGen, _difficultyManager);

            //Get the initial states
            _controllerState = GamePad.GetState(PlayerIndex.One);
            _keyboardState = Keyboard.GetState();
            _mouseState = Mouse.GetState();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //Call destroy on particle systems
            _particleSystemManager.DestroyAndRemoveAllParticleSystems();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            updateCount++;

            //Get the current control states
            GamePadState curGamepadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState curKeyboardState = Keyboard.GetState();
            MouseState curMouseState = Mouse.GetState();

            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || curKeyboardState.IsKeyDown(Keys.Escape))
            //{
            //    this.Exit();
            //}

            //If the game is running menus, run update and return.
            if (_isInMenus)
            {
                _currentMenu.update(curGamepadState, _controllerState, curKeyboardState, _keyboardState, ref _currentMenu, ref _isInMenus);
                _controllerState = curGamepadState;
                _keyboardState = curKeyboardState;
                return;
            }

            //Check for win
            if (_currentLevel.isEndGame())
            {
                bool gameOver = true;
                _worldEntities.ForEach(delegate(Entity e)
                {
                    if (e is EnemyEntity && gameOver)
                    {
                        gameOver = false;
                    }
                });

                if (gameOver)
                {
                    _currentMenu = new WinMenu(this, _currentMenu, graphics.GraphicsDevice.Viewport);
                    _isInMenus = true;
                    //restartGame(1);
                }
            }

            //Check sound manager to resume and music that may have paused due to menus
            if (_soundManager.isStopped())
            {
                _soundManager.playMusic();
            }
            else if (_soundManager.isPaused())
            {
                _soundManager.resumeMusic();
            }
            else if (_soundManager.areEffectsPaused())
            {
                _soundManager.resumeEffects();
            }

            #region Controls

            //Launch menu if START or ESCAPE is pressed
            if (curGamepadState.Buttons.Start == ButtonState.Pressed || curKeyboardState.IsKeyDown(Keys.Escape) && _keyboardState.IsKeyUp(Keys.Escape))
            {
                _isInMenus = true;
                if (!(_currentMenu is PauseMenu))
                {
                    _currentMenu = new PauseMenu(this, _currentMenu, graphics.GraphicsDevice.Viewport);
                }
                _soundManager.pauseMusic();
                _soundManager.pauseEffects();
                _keyboardState = curKeyboardState;
                return;
            }

            //Boost if LEFT TRIGGER or Q KEY is pressed
            if (curGamepadState.Triggers.Left > 0
                || curKeyboardState.IsKeyDown(Keys.Q) && _keyboardState.IsKeyUp(Keys.Q))
            {
                if (!_playerOne.getInDeathCooldown())
                {
                    _playerOne.useBoost(_soundManager);
                }
            }

            //Use bomb if LEFT BUMPER or SPACE KEY is pressed
            if (curGamepadState.Buttons.LeftShoulder == ButtonState.Pressed && _controllerState.Buttons.LeftShoulder != ButtonState.Pressed
                || curKeyboardState.IsKeyDown(Keys.Space) && _keyboardState.IsKeyUp(Keys.Space))
            {
                if (_playerOne.useBomb(_soundManager))
                {
                    if (!_playerOne.getInDeathCooldown())
                    {
                        _shipBombExplodeParticles.Emitter.PositionData.Position = _playerOne.getMatrix().Translation;
                        _shipBombExplodeParticles.Explode();
                        _worldEntities.ForEach(delegate(Entity entity)
                        {
                            if (entity is EnemyEntity)
                            {
                                //Debug.WriteLine((entity.getMatrix().Translation - _playerOne.getMatrix().Translation).Length());
                                if ((entity.getMatrix().Translation - _playerOne.getMatrix().Translation).Length() < BLAST_RADIUS)
                                {
                                    if (!(entity is EnemyIceBoss/* || entity is EnemyIceBit*/))
                                    {
                                        entity.setAlive(false);
                                    }
                                    else
                                    {
                                        ((EnemyEntity)entity).doDamage(1000); 
                                    }
                                }
                            }
                        });
                    }
                }
            }

            //Switch elements if RIGHT BUMPER or E KEY is pressed
            if (curGamepadState.Buttons.RightShoulder == ButtonState.Pressed && _controllerState.Buttons.RightShoulder != ButtonState.Pressed
                || curKeyboardState.IsKeyDown(Keys.E) && _keyboardState.IsKeyUp(Keys.E)
                || curMouseState.RightButton == ButtonState.Pressed && _mouseState.RightButton == ButtonState.Released)
            {
                if (!_playerOne.getInDeathCooldown())
                {
                    _playerOne.switchElement();
                    if (_playerOne.isIceElement())
                    {
                        _soundManager.addAttatchment(LoadedSounds.ICE_ACTIVATED, _playerOne);
                    }
                    else
                    {
                        _soundManager.addAttatchment(LoadedSounds.FIRE_ACTIVATED, _playerOne);
                    }
                }
            }

            //Move player if LEFT STICK is moved
            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X != 0 || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y != 0)
            {
                if (!_playerOne.getInDeathCooldown())
                {
                    float angle = MathHelper.ToDegrees((float)Math.Atan2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X));
                    _playerOne.setYaw(0);
                    camUp = _playerOne.getMatrix().Left;
                    _playerOne.setYaw(angle);
                    
                    _playerOne.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * _playerOne.getSpeed() /* * (gameTime.ElapsedGameTime.Milliseconds / 10) */));
                    _playerOne.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y * _playerOne.getSpeed() /*  * (gameTime.ElapsedGameTime.Milliseconds / 10) */));
                }
            }

            //Move player if WASD KEY or ARROW KEY is pressed
            if (curKeyboardState.IsKeyDown(Keys.W) || curKeyboardState.IsKeyDown(Keys.Up)   ||
                curKeyboardState.IsKeyDown(Keys.S) || curKeyboardState.IsKeyDown(Keys.Down) ||
                curKeyboardState.IsKeyDown(Keys.A) || curKeyboardState.IsKeyDown(Keys.Left) ||
                curKeyboardState.IsKeyDown(Keys.D) || curKeyboardState.IsKeyDown(Keys.Right)) 
            {
                if (!_playerOne.getInDeathCooldown())
                {
                    Entity playerShip = _playerOne;
                    _playerOne.setYaw(0);
                    camUp = _playerOne.getMatrix().Left;

                    if (curKeyboardState.IsKeyDown(Keys.W) && curKeyboardState.IsKeyDown(Keys.A) ||
                        curKeyboardState.IsKeyDown(Keys.Up) && curKeyboardState.IsKeyDown(Keys.Left))
                    {
                        playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, 0.75f * _playerOne.getSpeed()));
                        playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, -0.75f * _playerOne.getSpeed()));
                        _playerOne.setYaw(135);
                    }
                    else if (curKeyboardState.IsKeyDown(Keys.W) && curKeyboardState.IsKeyDown(Keys.D) ||
                        curKeyboardState.IsKeyDown(Keys.Up) && curKeyboardState.IsKeyDown(Keys.Right))
                    {
                        playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, 0.75f * _playerOne.getSpeed()));
                        playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, 0.75f * _playerOne.getSpeed()));
                        _playerOne.setYaw(45);
                    }
                    else if (curKeyboardState.IsKeyDown(Keys.S) && curKeyboardState.IsKeyDown(Keys.A) ||
                        curKeyboardState.IsKeyDown(Keys.Down) && curKeyboardState.IsKeyDown(Keys.Left))
                    {
                        playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, -0.75f * _playerOne.getSpeed()));
                        playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, -0.75f * _playerOne.getSpeed()));
                        _playerOne.setYaw(-135);
                    }
                    else if (curKeyboardState.IsKeyDown(Keys.S) && curKeyboardState.IsKeyDown(Keys.D) ||
                        curKeyboardState.IsKeyDown(Keys.Down) && curKeyboardState.IsKeyDown(Keys.Right))
                    {
                        playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, -0.75f * _playerOne.getSpeed()));
                        playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, 0.75f * _playerOne.getSpeed()));
                        _playerOne.setYaw(-45);
                    }
                    else if (curKeyboardState.IsKeyDown(Keys.W) || curKeyboardState.IsKeyDown(Keys.Up))
                    {
                        playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, 1f * _playerOne.getSpeed()));
                        _playerOne.setYaw(90);
                    }
                    else if (curKeyboardState.IsKeyDown(Keys.S) || curKeyboardState.IsKeyDown(Keys.Down))
                    {
                        playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, -1f * _playerOne.getSpeed()));
                        _playerOne.setYaw(-90);
                    }
                    else if (curKeyboardState.IsKeyDown(Keys.A) || curKeyboardState.IsKeyDown(Keys.Left))
                    {
                        playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, -1f * _playerOne.getSpeed()));
                        _playerOne.setYaw(180);
                    }
                    else if (curKeyboardState.IsKeyDown(Keys.D) || curKeyboardState.IsKeyDown(Keys.Right))
                    {
                        playerShip.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, 1f * _playerOne.getSpeed()));
                        _playerOne.setYaw(0);
                    }
                }
            }

            //Shoot a bullet if the RIGHT STICK is moved
            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X != 0 || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y != 0)
            {
                float angle = MathHelper.ToDegrees((float)Math.Atan2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y, GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X));
                shootBullet(angle);
            }

            //Shoot a bullet in the direction that LEFT CLICK was pressed
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Vector3 cameraPosition = _playerOne.getMatrix().Translation + _playerOne.getMatrix().Up * 500;
                Vector3 cameraTarget = _playerOne.getMatrix().Translation;
                Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, camUp);
                Matrix proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 50000.0f);
                Vector3 projectedPlayerPos = GraphicsDevice.Viewport.Project(Vector3.Zero, proj, view, _playerOne.getMatrix());

                float angle = MathHelper.ToDegrees((float)Math.Atan2(projectedPlayerPos.Y - Mouse.GetState().Y, Mouse.GetState().X - projectedPlayerPos.X));

                shootBullet(angle);
            }

            #endregion

            //Update all entities
            //Calculate the players affect on time (slow down power up)
            float calc = gameTime.ElapsedGameTime.Ticks * _playerOne.getAffectOnTime();
            TimeSpan playerAffectOnTime = new TimeSpan((long)calc);

            //Loop over all entities
            //Could have possibly been a .ForEach loop
            for (int entityID = 0; entityID < _worldEntities.Count; ++entityID)
            {
                //Check to see if the entitiy is a Player
                if (_worldEntities[entityID] is PlayerEntity)
                {
                    //Update as normal
                    _worldEntities[entityID].update(gameTime.ElapsedGameTime);
                }
                else
                {
                    //Update with time affected by player power up
                    _worldEntities[entityID].update(playerAffectOnTime);
                }

                //If entity is a bullet, spawn the particle for it.
                if (_worldEntities[entityID] is Bullet)
                {
                    if (_worldEntities[entityID] is IceBullet)
                    {
                        _bulletIceParticles.Emitter.PositionData.Position = _worldEntities[entityID].getMatrix().Translation;
                        _bulletIceParticles.AddParticle();
                    }
                    else
                    {
                        _bulletFireParticles.Emitter.PositionData.Position = _worldEntities[entityID].getMatrix().Translation;
                        _bulletFireParticles.AddParticle();
                    }
                }
                //If entitiy is a turret, try shoot
                //TODO: Make this work. Currently no turrets.
                else if (_worldEntities[entityID] is EnemyTurret)
                {
                    EnemyTurret turret = (EnemyTurret)_worldEntities[entityID];

                    //Check if the turret can shoot
                    if (!turret.shouldShoot())
                    {
                        continue;
                    }


                    /*
                     * To fix this, I think I'm going to have to fix up the orientation on models.
                     * That way I can use their up vector to correctly calculate the fire direction.
                     * Also, the bullet can inherit the entities rotation and shoot correctly.
                     */

                    Vector3 Va = _playerOne.getMatrix().Translation;
                    Vector3 Vb = turret.getMatrix().Translation;
                    Va.Normalize();
                    Vb.Normalize();

                    float sina = Vector3.Cross(Va, Vb).Length() / (Va.Length() * Vb.Length());
                    float cosa = Vector3.Dot(Va, Vb) / (Va.Length() * Vb.Length());
                    float sign = Vector3.Dot(_playerOne.getMatrix().Up, Vector3.Cross(Va, Vb));

                    Vector3 direction = _playerOne.getMatrix().Translation - turret.getMatrix().Translation;
                    direction.Normalize();
                    float angle = MathHelper.ToDegrees((float)-Math.Atan2(-direction.X, direction.Z)) - 90;

                    float calcYaw = angle;
                    if (sign < 0)
                    {
                        calcYaw = -angle;
                    }


                    Bullet newBullet = new FireBullet(turret.getMatrix().Translation, turret.getRotation(), calcYaw, _renderer.getModel("fireBullet"));
                    newBullet.setFriendly(false);
                    _worldEntities.Add(newBullet);
                    turret.didShoot();
                }
                //Add particles if it's a power up
                else if (_worldEntities[entityID] is PowerUp)
                {
                    //Disabled due to not looking so good and lag
                    //_powerUpParticles.Emitter.PositionData.Position = _worldEntities[entityID].getMatrix().Translation;
                    //_powerUpParticles.AddParticles(1);
                }
                //Add particles for the PushPull enemy
                else if (_worldEntities[entityID] is EnemyPushPull)
                {
                    //Disabled due to not looking so good and REALLY BAD lag
                    //_pushPullParticles.Emitter.PositionData.Position = _worldEntities[entityID].getMatrix().Translation;
                    //_pushPullParticles.AddParticles(5);
                }
            }

            #region Collision Checking
            //Check for collision, maybe would have been wise to move elsewhere
            for (int entity = 0; entity < _worldEntities.Count; ++entity)
            {
                //Check entity is a ModelEntity (These mainly have bounding spheres)
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
                                    if ((currentEntity is PlayerEntity && checkEntity is EnemyEntity && !(_worldEntities[entity] is EnemyIceBoss)) ||
                                        (checkEntity is PlayerEntity && currentEntity is EnemyEntity && !(_worldEntities[entity] is EnemyIceBoss)))
                                    {
                                        //If the player is boosting, they shouldn't die. So only kill enemy
                                        if (currentEntity is PlayerEntity)
                                        {
                                            if (((PlayerEntity)currentEntity).isBoosting())
                                            {
                                                if (!(checkEntity is EnemyIceBoss))
                                                {
                                                    checkEntity.setAlive(false);
                                                    checkEntity.doDamage(currentEntity.getHealth());
                                                    continue;
                                                }
                                            }
                                        }
                                        else if (checkEntity is PlayerEntity)
                                        {
                                            if (((PlayerEntity)checkEntity).isBoosting())
                                            {
                                                currentEntity.setAlive(false);
                                                currentEntity.doDamage(checkEntity.getHealth());
                                                continue;
                                            }
                                        }

                                        //Player isn't boosting do mass destruction
                                        currentEntity.doDamage(currentEntity.getHealth());
                                        currentEntity.setAlive(false);
                                        checkEntity.doDamage(checkEntity.getHealth());
                                        checkEntity.setAlive(false);
                                        

                                        continue; //Do not check if bullets, this will be game over/remove life.
                                    }

                                    //Check to see if either entity is a bullet. If so, apply bullet damage.
                                    //IF THINGS GO WRONG UNCOMMENT THIS (Bullets not colliding)
                                    //if (currentEntity is Bullet)
                                    //{
                                    //    Bullet curBullet = (Bullet)currentEntity;
                                    //    int elementalBonusDamage = 0;

                                    //    if (!curBullet.isEnemyBullet())
                                    //    {
                                    //        break;
                                    //    }

                                    //    if (checkEntity is EnemyEntity && !curBullet.isEnemyBullet())
                                    //    {
                                    //        EnemyEntity enemy = (EnemyEntity)checkEntity;
                                    //        if (enemy.isIceEnemy())
                                    //        {
                                    //            if (curBullet is FireBullet)
                                    //            {
                                    //                elementalBonusDamage += 250;
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            if (curBullet is IceBullet)
                                    //            {
                                    //                elementalBonusDamage += 250;
                                    //            }
                                    //        }
                                    //    }

                                    //    checkEntity.doDamage(curBullet.getDamage() + elementalBonusDamage);
                                    //    currentEntity.setAlive(false);
                                    //}
                                    if (checkEntity is Bullet)
                                    {

                                        Bullet curBullet = (Bullet)checkEntity;
                                        int elementalBonusDamage = 0;

                                        //if(curBullet.isEnemyBullet()){
                                        //    break;
                                        //}

                                        if (currentEntity is EnemyEntity)
                                        {
                                            if (!curBullet.getFriendly())
                                            {
                                                break;
                                            }

                                            EnemyEntity enemy = (EnemyEntity)currentEntity;
                                            if (enemy.isIceEnemy())
                                            {
                                                if (curBullet is FireBullet)
                                                {
                                                    elementalBonusDamage += (int)(250 * _difficultyManager._bonusElementalDamage);
                                                }
                                            }
                                            else
                                            {
                                                if (curBullet is IceBullet)
                                                {
                                                    elementalBonusDamage += (int)(250 * _difficultyManager._bonusElementalDamage);
                                                }
                                            }
                                            currentEntity.doDamage((int)(((Bullet)checkEntity).getDamage() * _difficultyManager._bulletDamage + elementalBonusDamage));
                                            checkEntity.setAlive(false);
                                        }
                                        else if (currentEntity is PlayerEntity)
                                        {
                                            if (curBullet.getFriendly())
                                            {
                                                break;
                                            }

                                            PlayerEntity player = (PlayerEntity)currentEntity;
                                            if (player.isIceElement())
                                            {
                                                if (curBullet is FireBullet)
                                                {
                                                    elementalBonusDamage += (int)(250 * _difficultyManager._bonusElementalDamage);
                                                }
                                            }
                                            else
                                            {
                                                if (curBullet is IceBullet)
                                                {
                                                    elementalBonusDamage += (int)(250 * _difficultyManager._bonusElementalDamage);
                                                }
                                            }
                                            currentEntity.doDamage((int)(((Bullet)checkEntity).getDamage() * _difficultyManager._bulletDamage + elementalBonusDamage));
                                            checkEntity.setAlive(false);
                                        }


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
                else if (_worldEntities[entity] is PlaySphere) //Special check for play sphere
                {
                    //As we know it is a PlaySphere, cast it to get access to functions.
                    PlaySphere currentEntity = (PlaySphere)_worldEntities[entity];

                    //Loop over all the entities for the special case
                    for (int nextEntity = 0; nextEntity < _worldEntities.Count; ++nextEntity)
                    {
                        //Check to see if this entity is a EnemyEntity
                        if (_worldEntities[nextEntity] is EnemyEntity)
                        {
                            //Cast to get functionality
                            EnemyEntity checkEntity = (EnemyEntity)_worldEntities[nextEntity];

                            //Do a check to see if either have been flagged dead as they may have collided so no point re-checking.
                            if (currentEntity.getAlive() == false)
                            {
                                break; //Quit this inner for loop, no point checking the rest.
                            }
                            if (checkEntity.getAlive() == false)
                            {
                                continue; //Continue to next entity in this loop.
                            }

                            //Check for intersect
                            if (currentEntity.getBoundingSphere().Intersects(checkEntity.getBoundingSphere()))
                            {
                                //Set isSpawning to false so they now move around the sphere
                                checkEntity.setIsSpawning(false);
                            }
                        }
                    }
                }
                else if (_worldEntities[entity] is PowerUp) //Special check for Power Ups
                {
                    //As we know it is a Power Up, cast it to get access to functions.
                    PowerUp currentEntity = (PowerUp)_worldEntities[entity];

                    //Check Collision between player and power up
                    if (_playerOne.getBoundingSphere().Intersects(currentEntity.getBoundingSphere()))
                    {
                        _playerOne.addPowerUp(currentEntity);
                        currentEntity.setAlive(false);
                    }

                }
                else if (_worldEntities[entity] is EnemyIceBoss)
                {
                    //Do damage if colliding with ice bit
                    foreach (EnemyIceBit iceBit in ((EnemyIceBoss)_worldEntities[entity]).getMyBits())
                    {
                        if (iceBit.getBoundingSphere().Intersects(_playerOne.getBoundingSphere()))
                        {
                            if (!_playerOne.getInDeathCooldown())
                            {
                                _playerOne.doDamage(_playerOne.getHealth());
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }

            //Do a remove all which will check if the entitiy should be removed
            _worldEntities.RemoveAll(removeEntity);

            #endregion


            //If player is bossting, modify the trail to emit a blue boost
            if (_playerOne.isBoosting())
            {
                if (_playerOne.isIceElement())
                {
                    _shipExaustParticles.TrailStartColor = Color.Blue;
                }
                else
                {
                    _shipExaustParticles.TrailStartColor = Color.Orange;
                }
            }
            else
            {
                _shipExaustParticles.TrailStartColor = Color.Red;
            }

            //Update emitter positions
            _shipExaustParticles.Emitter.PositionData.Position = _playerOne.getMatrix().Translation + _playerOne.getMatrix().Backward * 10;
            _shipBoostParticles.Emitter.PositionData.Position = _playerOne.getMatrix().Translation + _playerOne.getMatrix().Forward * 20;
            _shipBoostParticles.Emitter.EmitParticlesAutomatically = _playerOne.isBoosting();
            _shipBoostGlowParticles.Emitter.PositionData.Position = _playerOne.getMatrix().Translation +  _playerOne.getMatrix().Up * 10 + _playerOne.getMatrix().Forward * 10;
            _shipBoostGlowParticles.Emitter.EmitParticlesAutomatically = _playerOne.isBoosting();

            //Give the universe and planet some rotation
            _universe.addRoll(-0.001f);
            _universe.addYaw(-0.005f);
            _planet.addRoll(0.02f);
            _planet.addPitch(0.05f);

            //Call update on managers
            _soundManager.update(gameTime);
            _fpsManager.update(gameTime);
            if (!_currentLevel.isEndGame())
            {
                _spawnManager.update(gameTime, _planet, _playerOne);
            }
            _currentLevel.update(gameTime, _planet, _playerOne);
            _renderer.update(gameTime);

            //Reduce last shot time
            if (_lastBulletShot > 0)
            {
                _lastBulletShot -= gameTime.ElapsedGameTime.Milliseconds;
            }

            //Save current control states
            _controllerState = curGamepadState;
            _keyboardState = curKeyboardState;
            _mouseState = curMouseState;


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

            

            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //Begin the bloom drawing
            bloom.BeginDraw();

            Viewport viewport = graphics.GraphicsDevice.Viewport;
            float aspectRatio = viewport.AspectRatio;

            //Vector3 cameraPosition = _playerOne.getMatrix().Translation + _playerOne.getMatrix().Left * 500;
            Vector3 cameraPosition = _playerOne.getMatrix().Translation + _playerOne.getMatrix().Up * 500;
            Vector3 cameraTarget = _playerOne.getMatrix().Translation;
            camUp.Normalize();

            Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, camUp);
            Matrix proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 50000.0f);

            //Render all models that are loaded in.
            foreach (Entity entity in _worldEntities)
            {
                _renderer.renderEntity(view, proj, entity);
            }

            // Set the Particle System's World, View, and Projection matrices so that it knows how to draw the particles properly.
            _particleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(Matrix.Identity, view, proj);

            // Update the Particle System
            _particleSystemManager.SetCameraPositionForAllParticleSystems(cameraPosition);
            _particleSystemManager.UpdateAllParticleSystems((float)gameTime.ElapsedGameTime.TotalSeconds);
            _particleSystemManager.DrawAllParticleSystems();

            base.Draw(gameTime);

            ////Draw 2D things - AFTER bloom
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            drawCount++;
            spriteBatch.DrawString(_hudFont, "FPS: " + _fpsManager.getFPS().ToString(), new Vector2(viewport.Width - 100, viewport.Height - 30), Color.White);
            spriteBatch.DrawString(_hudFont, "Draw: " + drawCount.ToString(), new Vector2(viewport.Width - 160, viewport.Height - 60), Color.White);
            spriteBatch.DrawString(_hudFont, "Update: " + updateCount.ToString(), new Vector2(viewport.Width - 180, viewport.Height - 90), Color.White);

            //Draw boost graphic
            int flameHeight = (int)(_boostFlame.Height * _playerOne.getBoostReplenishedScalar());
            spriteBatch.Draw(_boostFlame, new Rectangle((int)_boostFlamePosition.X, (int)_boostFlamePosition.Y + _boostFlame.Height - flameHeight, _boostFlame.Width, flameHeight),
                                          new Rectangle(0, _boostFlame.Height - flameHeight, _boostFlame.Width, flameHeight), Color.White);
            spriteBatch.Draw(_boostOverlay, _boostOverlayPosition, Color.White);

            //Draw the number of lives remaining
            for (int numLives = 0; numLives < _playerOne.getLives(); ++numLives)
            {
                spriteBatch.Draw(_lifeIcon, new Vector2(_lifeIconPosition.X + ((_lifeIcon.Width + 5) * numLives), _lifeIconPosition.Y), Color.White);
                if (numLives == 2 && _playerOne.getLives() > 3)
                {
                    _fontRenderer.DrawText(spriteBatch, new Vector2(_lifeIconPosition.X + ((_lifeIcon.Width + 5) * (numLives + 1)), _lifeIconPosition.Y - 10), "+" + (_playerOne.getLives() - 3).ToString());
                    break;
                }
            }

            //Draw the number of bombs remaining
            for (int numBombs = 0; numBombs < _playerOne.getBombs(); ++numBombs)
            {
                spriteBatch.Draw(_bombIcon, new Vector2(_bombIconPosition.X + ((_bombIcon.Width + 5) * numBombs), _bombIconPosition.Y), Color.White);
                if (numBombs == 2 && _playerOne.getBombs() > 3)
                {
                    _fontRenderer.DrawText(spriteBatch, new Vector2(_bombIconPosition.X + ((_bombIcon.Width + 5) * (numBombs + 1)), _bombIconPosition.Y - 10), "+" + (_playerOne.getBombs() - 3).ToString());
                    break;
                }
            }

            //Draw all the current scores that should be visible
            foreach (ScoreText score in _renderer.getScores())
            {
                Vector3 screenSpace = GraphicsDevice.Viewport.Project(Vector3.Zero, proj, view, score.getPositionMatrix());
                String scoreText = score.getScoreAmount().ToString();
                Vector2 textOffset = _hudFont.MeasureString(scoreText) / 2;
                spriteBatch.DrawString(_hudFont, score.getScoreAmount().ToString(), new Vector2(screenSpace.X, screenSpace.Y), Color.Red, 0, textOffset, score.getScale(), SpriteEffects.None, 0);
            }

            //Draw any warnings from next spawn
            if (_currentLevel.drawWarning())
            {
                _soundManager.playWarningSound((int)_currentLevel.warningTime().TotalMilliseconds);
                string outputString = "Incoming! " + _currentLevel.warningTime().Seconds.ToString() + ":" + _currentLevel.warningTime().Milliseconds.ToString();
                float textScale = 1.0f;
                //int textWidth = _fontRenderer.TextWidth(outputString, textScale); //Not a fixed width so moves about as lot...
                _fontRenderer.DrawText(spriteBatch, new Vector2((viewport.Width / 2) - 250, viewport.Height - 100), outputString, textScale, _currentLevel.warningColor());
            }

            //Draw Score
            _fontRenderer.DrawText(spriteBatch, new Vector2(viewport.Width - 25, 20), formatNumericToString(_currentScore), 0.8f, Color.Red, true);
            _fontRenderer.DrawText(spriteBatch, new Vector2(viewport.Width - 25, 80), _playerOne.getScoreMultiplier().ToString("0.0") + "x", 0.3f, Color.White, true);


            //Draw Boss health if one is up
            _worldEntities.ForEach(delegate(Entity entity)
            {
                if (entity is EnemyIceBoss)
                {
                    EnemyIceBoss curEntity = (EnemyIceBoss)entity;
                    //_fontRenderer.DrawText(spriteBatch, new Vector2(400, 100), curEntity.getHealth().ToString());

                    Vector2 destination = new Vector2(110, 150);

                    //Draw Background
                    spriteBatch.Draw(_healthShieldBackground, new Vector2(destination.X - _healthShieldBackground.Width / 2, destination.Y), Color.White);

                    Rectangle healthRectangleCrop = new Rectangle(0, 0, (int)(_healthShieldBar.Width * ((float)curEntity.getHealth() / (float)curEntity.getInitialHealth())), _healthShieldBar.Height);
                    spriteBatch.Draw(_healthShieldBar, new Vector2(destination.X - _healthShieldBar.Width / 2, destination.Y), healthRectangleCrop, Color.White);
                    spriteBatch.Draw(_healthShieldFrame, new Vector2(destination.X - _healthShieldFrame.Width / 2, destination.Y), Color.White);
                }
            });

            spriteBatch.End();

            //Draw the menus if we're in them
            if (_isInMenus)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                if (_currentLevel.timePlayed().Milliseconds < 1)
                {
                    spriteBatch.Draw(_menuBackground, new Vector2(0, 0), Color.White);
                }
                else
                {
                    spriteBatch.Draw(_menuBackground, new Vector2(0, 0), Color.FromNonPremultiplied(255, 255, 255, 100));
                }
                for (int curItem = 0; curItem < _currentMenu.getMenuItems().Count; ++curItem)
                {
                    int textWidth = _fontRenderer.TextWidth(_currentMenu.getMenuItems()[curItem], 0.5f);
                    if (curItem == _currentMenu.getSelectedItem())
                    {
                        _fontRenderer.DrawText(spriteBatch, new Vector2(_currentMenu.getOffset().X - (textWidth / 2), _currentMenu.getOffset().Y + (40 * curItem)), _currentMenu.getMenuItems()[curItem], 0.5f, Color.White);
                    }
                    else
                    {
                        _fontRenderer.DrawText(spriteBatch, new Vector2(_currentMenu.getOffset().X - (textWidth / 2), _currentMenu.getOffset().Y + (40 * curItem)), _currentMenu.getMenuItems()[curItem], 0.5f, Color.LightBlue);
                    }
                }

                //Would have been a better idea to haw a draw call in the menus.
                if (_currentMenu is WinMenu)
                {
                    int youWinWidth = _fontRenderer.TextWidth("You Win!");
                    _fontRenderer.DrawText(spriteBatch, new Vector2(viewport.Width / 2 - youWinWidth / 2, viewport.Height - 300), "You Win!", 1, Color.Green);
                    int finalScoreWidth = _fontRenderer.TextWidth("Final Score: " + formatNumericToString(_currentScore));
                    _fontRenderer.DrawText(spriteBatch, new Vector2(viewport.Width / 2 - finalScoreWidth / 2, viewport.Height - 200), "Final Score: " + formatNumericToString(_currentScore), 1, Color.Green);
                }
                else if (_currentMenu is ControlsMenu)
                {
                    Rectangle source = new Rectangle(0, 0, _gameControls.Width, _gameControls.Height);
                    Rectangle destination = new Rectangle((int)(_currentMenu.getOffset().X - _gameControls.Width / 2), (int)(_currentMenu.getOffset().Y - _gameControls.Height),
                                                          _gameControls.Width, _gameControls.Height);

                    //Texture2D fakeTexture = new Texture2D(GraphicsDevice, 1, 1);
                    //fakeTexture.SetData(new Color[] { Color.Black });
                    //spriteBatch.Draw(fakeTexture, destination, Color.FromNonPremultiplied(0, 0, 0, 160));

                    spriteBatch.Draw(_gameControls, destination, source, Color.White);
                }

                //Draw Buttons
                Vector2 aButtonPos = new Vector2(viewport.Width / 2 - 150, viewport.Height - 100);
                spriteBatch.Draw(_xboxA, new Rectangle((int)aButtonPos.X, (int)aButtonPos.Y, _xboxA.Width / 2, _xboxA.Height / 2), Color.White);
                _fontRenderer.DrawText(spriteBatch, new Vector2(aButtonPos.X + 55, aButtonPos.Y + 4), "Select", 0.5f);
                Vector2 bButtonPos = new Vector2(viewport.Width / 2 + 50, viewport.Height - 100);
                spriteBatch.Draw(_xboxB, new Rectangle((int)bButtonPos.X, (int)bButtonPos.Y, _xboxB.Width / 2, _xboxB.Height / 2), Color.White);
                _fontRenderer.DrawText(spriteBatch, new Vector2(bButtonPos.X + 55, bButtonPos.Y + 4), "Back", 0.5f);

                _fontRenderer.DrawText(spriteBatch, new Vector2(viewport.Width / 2 - 150, viewport.Height - 60), "       (SPACE)         (ESC)", 0.38f);

                spriteBatch.End();
            }
        }

        //Returns a formatted number like "10,000,000"
        string formatNumericToString(int number)
        {
            if (number <= 999 && number >= -999)
            {
                return number.ToString();
            }
            else
            {
                return number.ToString("0,0");
            }
        }

        //Shoots a bullet in given angle from player
        private void shootBullet(float angle)
        {
            if (_lastBulletShot <= 0)
            {
                if (!_playerOne.getInDeathCooldown())
                {
                    float oldYaw = MathHelper.ToDegrees(_playerOne.getYaw());
                    _playerOne.setYaw(0);
                    Quaternion playerRotation = _playerOne.getRotation();
                    _playerOne.setYaw(oldYaw);

                    float calcYaw = /*-getEntity("player").getYaw() +*/ angle + 90;

                    if (_playerOne.isIceElement())
                    {
                        for (int iceOffset = 0; iceOffset < 4; ++iceOffset)
                        {
                            Bullet newBullet = new IceBullet(_playerOne.getMatrix().Translation, playerRotation, calcYaw - 15 + (iceOffset * 10), _renderer.getModel("iceBullet"), _randomGen.Next(1500, 3000), (float)_randomGen.NextDouble() + 0.5f);
                            newBullet.setSpeed((float)_randomGen.NextDouble() * 0.5f + 0.5f);
                            _worldEntities.Add(newBullet);
                            _soundManager.addAttatchment(LoadedSounds.ICE_BULLET_FIRED, newBullet);
                        }
                    }
                    else
                    {
                        Bullet newBullet = new FireBullet(_playerOne.getMatrix().Translation, playerRotation, calcYaw, _renderer.getModel("fireBullet"));
                        _worldEntities.Add(newBullet);
                        _soundManager.addAttatchment(LoadedSounds.FIRE_BULLET_FIRED, newBullet);
                    }

                    //Reset bullet timer
                    _lastBulletShot = 250 * _difficultyManager._playerShootSpeed * _playerOne.getShootingSpeed();

                    //If multiple shooting speeds have been picke dup, just cap at 50, seems like a good speed.
                    if (_lastBulletShot < 50)
                    {
                        _lastBulletShot = 50;
                    }
                }
            }
        }

        //Will load in all particle systems
        private void loadParticleSystems()
        {
            // Declare a new Particle System instance and Initialize it
            _particleSystemManager = new ParticleSystemManager();

            _shipExaustParticles = new TrailParticleSystem(this);
            _shipBoostParticles = new BoostParticleSystem(this);
            _shipBoostGlowParticles = new BoostGlowParticleSystem(this);
            //_bulletSmokeParticles = new SmokeParticleSystem(this);
            _bulletFireParticles = new FireBulletParticleSystem(this);
            _bulletIceParticles = new IceBulletParticleSystem(this);
            _rockExplodeParticles = new ExplosionRockParticleSystem(this);
            _mineExplodeParticles = new ExplosionMineParticleSystem(this);
            _shipExplodeParticles = new ShipExplodeParticleSystem(this);
            _shipBombExplodeParticles = new ShipBombExplodeParticleSystem(this);
            //_powerUpParticles = new PowerUpParticleSystem(this);
            //_pushPullParticles = new PushPullParticleSystem(this);

            _particleSystemManager.AddParticleSystem(_shipExaustParticles);
            _particleSystemManager.AddParticleSystem(_shipBoostParticles);
            _particleSystemManager.AddParticleSystem(_shipBoostGlowParticles);
            //_particleSystemManager.AddParticleSystem(_bulletSmokeParticles);
            _particleSystemManager.AddParticleSystem(_bulletFireParticles);
            _particleSystemManager.AddParticleSystem(_bulletIceParticles);
            _particleSystemManager.AddParticleSystem(_rockExplodeParticles);
            _particleSystemManager.AddParticleSystem(_mineExplodeParticles);
            _particleSystemManager.AddParticleSystem(_shipExplodeParticles);
            _particleSystemManager.AddParticleSystem(_shipBombExplodeParticles);
            //_particleSystemManager.AddParticleSystem(_powerUpParticles);
            //_particleSystemManager.AddParticleSystem(_pushPullParticles);
            _particleSystemManager.AutoInitializeAllParticleSystems(this.GraphicsDevice, this.Content, null);

            _shipBoostParticles.Emitter.EmitParticlesAutomatically = false;
            _shipBoostGlowParticles.Emitter.EmitParticlesAutomatically = false;
            //_bulletSmokeParticles.Emitter.EmitParticlesAutomatically = false;
            _bulletFireParticles.Emitter.EmitParticlesAutomatically = false;
            _bulletIceParticles.Emitter.EmitParticlesAutomatically = false;
            _shipExplodeParticles.Emitter.EmitParticlesAutomatically = false;
            _shipBombExplodeParticles.Emitter.EmitParticlesAutomatically = false;
            //_powerUpParticles.Emitter.EmitParticlesAutomatically = false;
            //_pushPullParticles.Emitter.EmitParticlesAutomatically = false;

            _rockExplodeParticles.Emitter.LerpEmittersPositionAndOrientation = false;
            _mineExplodeParticles.Emitter.LerpEmittersPositionAndOrientation = false;
            _shipExplodeParticles.Emitter.LerpEmittersPositionAndOrientation = false;
            //_pushPullParticles.Emitter.LerpEmittersPositionAndOrientation = false;

            //_rockExplodeParticles.ChangeExplosionColor(Color.Cyan);
            //_mineExplodeParticles.ChangeExplosionColor(Color.Red);
        }

        //internal void changeMenu(Menu newMenu)
        //{
        //    _currentMenu = newMenu;
        //}

        //Changes the game difficulty
        public void setGameDifficulty(int difficulty)
        {
            switch (difficulty)
            {
                case 0:
                    _difficultyManager.setEasy();
                    break;
                case 1:
                    _difficultyManager.setMedium();
                    break;
                case 2:
                    _difficultyManager.setHard();
                    break;
                default:
                    _difficultyManager.setMedium();
                    break;
            }
        }

    }
}
