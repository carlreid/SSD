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
            _boosts = 3;
            _lastBoostApplied = 20000; //20 Seconds
            _boostReplenishTime = 20000;
            _shipSpeed = 0.01f;
            _inBoostPhase = false;
            _boostPhaseTimer = 0;

            _isIceElement = true;
            _currentColour = Color.Cyan;
        }

        public override void update(TimeSpan deltaTime)
        {
            base.update(deltaTime);

            //Add boost to player (based on time, maybe apply to score?)
            _lastBoostApplied -= deltaTime.Milliseconds;
            if (_lastBoostApplied <= 0)
            {
                if (_boosts < 5)
                {
                    _boosts += 1;
                }
                _lastBoostApplied = _boostReplenishTime;
            }

            if (_inBoostPhase)
            {
                _boostPhaseTimer -= deltaTime.Milliseconds;
                _shipSpeed = 0.01f + (0.1f * (_boostPhaseTimer / 2000f));
                if (_boostPhaseTimer <= 0)
                {
                    _inBoostPhase = false;
                    _shipSpeed = 0.01f; //Return to default speed
                }
            }

        }

        override public void draw(Matrix view, Matrix proj, GraphicsDevice graphicsDevice)
        {
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

        public void useBoost()
        {
            if (_boosts > 0 && _inBoostPhase == false)
            {
                _inBoostPhase = true;
                _boosts -= 1;
                _boostPhaseTimer = 1000; //2 Seconds boost time
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

        float _shipSpeed;
        int _lives;
        int _boosts;
        bool _inBoostPhase;
        int _boostPhaseTimer;
        int _lastBoostApplied;
        int _boostReplenishTime;

        bool _isIceElement;
        Color _currentColour;

    }
}
