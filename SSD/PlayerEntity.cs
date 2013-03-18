using Microsoft.Xna.Framework;
using System;

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
                _boostPhaseTimer = 2000; //2 Seconds boost time
            }
        }

        float _shipSpeed;
        int _lives;
        int _boosts;
        bool _inBoostPhase;
        int _boostPhaseTimer;
        int _lastBoostApplied;
        int _boostReplenishTime;

    }
}
