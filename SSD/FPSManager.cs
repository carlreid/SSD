using Microsoft.Xna.Framework;
using System;

namespace SSD
{
    class FPSManager
    {

        public FPSManager()
        {
            _frameCount = 0;
            _fps = 0;
        }

        public void update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;
            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _fps = _frameCount;
                _frameCount = 0;
            }
            
        }

        public int getFPS()
        {
            ++_frameCount;
            return _fps;
        }

        int _frameCount;
        int _fps;
        TimeSpan _elapsedTime = TimeSpan.Zero;
    }
}
