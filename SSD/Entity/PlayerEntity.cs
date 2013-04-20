using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class PlayerEntity : ModelEntity
    {
        public PlayerEntity(Vector3 position, ModelContainer model, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0)
            : base(position, model, scale, yaw, pitch, roll)
        {
            _lives = 3;
            _bombs = 3;
            _boosts = 0;
            _lastBoostApplied = 0; //20 Seconds
            _boostReplenishTime = 20000;
            _shipSpeed = 0.01f;
            _inBoostPhase = false;
            _boostPhaseTimer = 0;
            _isInDeathCooldown = false;
            _isIceElement = true;
            _currentColour = Color.Cyan;
        }

        public override void update(TimeSpan deltaTime)
        {
            base.update(deltaTime);

            //If in death cooldown, reduce the timer and skip any boost replenish time.
            if (_isInDeathCooldown)
            {
                _deathCooldownTimer -= deltaTime.Milliseconds;
                if (_deathCooldownTimer <= 0)
                {
                    _isInDeathCooldown = false;
                }
                else
                {
                    return;
                }
            }

            //Add boost to player (based on time, maybe apply to score?)
            if (_boosts < 1)
            {
                _lastBoostApplied -= deltaTime.Milliseconds;
                if (_lastBoostApplied <= 0)
                {
                    _boosts += 1;
                    _lastBoostApplied = _boostReplenishTime;
                }
            }
            else if (_boosts == 1)
            {
                _lastBoostApplied = 0;
            }

            if (_inBoostPhase)
            {
                _boostPhaseTimer -= deltaTime.Milliseconds;
                _shipSpeed = 0.01f + (0.05f * (_boostPhaseTimer / 1000f));
                if (_boostPhaseTimer <= 0)
                {
                    _inBoostPhase = false;
                    _shipSpeed = 0.01f; //Return to default speed
                }
            }

        }

        override public void draw(Matrix view, Matrix proj, GraphicsDevice graphicsDevice)
        {
            //If the player is dead, don't draw.
            if (_isInDeathCooldown)
            {
                return;
            }

            foreach (ModelMesh mesh in getModelContainer().getModel().Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.DiffuseColor = _currentColour.ToVector3();
                        effect.EmissiveColor = _currentColour.ToVector3();

                        effect.World = getModelContainer().getBoneTransform(mesh.ParentBone.Index) * getMatrix();
                        effect.View = view;
                        effect.Projection = proj;
                    }
                    //BoundingSphereRenderer.Render(entity.getBoundingSphere(), _graphicsDevice, view, proj, Color.Red);
                    mesh.Draw();
                }
            }
        }

        public float getSpeed()
        {
            return _shipSpeed;
        }

        public void useBoost(SoundManager soundManager)
        {
            if (_boosts > 0 && _inBoostPhase == false)
            {
                _inBoostPhase = true;
                _boosts -= 1;
                _boostPhaseTimer = 1000; //2 Seconds boost time
                _lastBoostApplied = _boostReplenishTime;
                soundManager.addAttatchment(LoadedSounds.SHIP_BOOST, this);
            }
        }

        public bool isBoosting()
        {
            return _inBoostPhase;
        }

        public void switchElement()
        {
            _isIceElement = !_isIceElement;
            if (_isIceElement)
            {
                _currentColour = Color.Cyan;
            }
            else
            {
                _currentColour = Color.Red;
            }
        }

        public bool isIceElement()
        {
            return _isIceElement;
        }

        public int getBoostCount()
        {
            return _boosts;
        }

        public float getBoostReplenishedScalar()
        {
            return 1.0f - ((float)_lastBoostApplied / (float)_boostReplenishTime);
        }

        public int getLives()
        {
            return _lives;
        }

        public int getBombs()
        {
            return _bombs;
        }

        public void removeLife()
        {
            --_lives;
        }

        public void addLife()
        {
            ++_lives;
        }

        public bool getInDeathCooldown()
        {
            return _isInDeathCooldown;
        }

        public void setInDeathCooldown(bool inDeathCooldown)
        {
            if (inDeathCooldown)
            {
                _deathCooldownTimer = 2500;
            }
            _isInDeathCooldown = inDeathCooldown;
        }

        float _shipSpeed;
        float _deathCooldownTimer;
        int _lives;
        int _boosts;
        int _bombs;
        bool _inBoostPhase;
        int _boostPhaseTimer;
        int _lastBoostApplied;
        int _boostReplenishTime;
        bool _isInDeathCooldown;
        bool _isIceElement;
        Color _currentColour;

    }
}
