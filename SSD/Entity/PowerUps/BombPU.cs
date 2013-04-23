using Microsoft.Xna.Framework;
using System;

namespace SSD
{
    class BombPU : PowerUp
    {
        public BombPU(Vector3 position, ModelContainer model)
            : base(position, model, 5000, 2.0f)
        {
            base._increaseBy = 1.0f;
            base._timeTillRunsOut = 0;
            base._runsOut = false;
            _powerUpColour = Color.OrangeRed;
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
