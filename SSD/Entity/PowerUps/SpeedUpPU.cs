using Microsoft.Xna.Framework;
using System;

namespace SSD
{
    class SpeedUpPU : PowerUp
    {
        public SpeedUpPU(Vector3 position, ModelContainer model)
            : base(position, model, 5000, 1.0f)
        {
            base._increaseBy = 0.01f;
            base._timeTillRunsOut = 5000;
            base._runsOut = true;
            _powerUpColour = Color.Red;
        }

        override public void update(TimeSpan deltaTime)
        {
            base.update(deltaTime);
            base.addRoll(1f);
            base.addPitch(10f);
            base.addRoll(5f);
        }

    }
}
