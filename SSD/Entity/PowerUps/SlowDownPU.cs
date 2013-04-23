using Microsoft.Xna.Framework;
using System;

namespace SSD
{
    class SlowDownPU : PowerUp
    {
        public SlowDownPU(Vector3 position, ModelContainer model)
            : base(position, model, 5000, 4.5f)
        {
            base._increaseBy = -0.9f;
            base._timeTillRunsOut = 5000;
            base._runsOut = true;
            _powerUpColour = Color.Blue;
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
