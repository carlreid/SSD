using Microsoft.Xna.Framework;
using System;

namespace SSD
{
    class BulletSpeedPU : PowerUp
    {
        public BulletSpeedPU(Vector3 position, ModelContainer model)
            : base(position, model, 5000, 3.5f)
        {
            base._increaseBy = -0.5f;
            base._timeTillRunsOut = 10000;
            base._runsOut = true;
            _powerUpColour = Color.Purple;
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
