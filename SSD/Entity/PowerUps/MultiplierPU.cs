using Microsoft.Xna.Framework;
using System;

namespace SSD
{
    class MultiplierPU : PowerUp
    {
        public MultiplierPU(Vector3 position, ModelContainer model)
            : base(position, model, 10000, 1.0f)
        {
            base._increaseBy = 0.2f;
            base._timeTillRunsOut = 0;
            base._runsOut = false;
            _powerUpColour = Color.Yellow;
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
