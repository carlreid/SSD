using Microsoft.Xna.Framework;
using System;

namespace SSD
{
    abstract class GameLevel
    {
        public GameLevel(SpawnManager spawnManager, Random randomGen)
        {
            _spawnManager = spawnManager;
            _currentPhase = 0;
            _randomGen = randomGen;
            _drawWarning = false;
            _warningTime = TimeSpan.FromSeconds(0);
            _spawnCounter = 0;
            _warningColour = Color.Red;
            _colorGoingDown = true;
        }

        public virtual void update(GameTime gameTime, Entity targetPlanet)
        {
            _levelTime += gameTime.ElapsedGameTime;
            //_spawnManager.update(gameTime, targetPlanet);
        }

        public TimeSpan timeRemaining(){
            return _levelTime;
        }

        public bool drawWarning()
        {
            return _drawWarning;
        }

        public TimeSpan warningTime()
        {
            return _warningTime;
        }

        public Color warningColor()
        {
            if (_colorGoingDown && _warningColour.R > 150)
            {
                _warningColour.R -= 10;
            }
            else
            {
                _colorGoingDown = false;
                _warningColour.R += 10;
                if (_warningColour.R == 255)
                {
                    _colorGoingDown = true;
                }
            }
            return _warningColour;
        }

        protected TimeSpan _levelTime;
        protected SpawnManager _spawnManager;
        protected int _currentPhase;
        protected Random _randomGen;
        protected bool _drawWarning;
        protected TimeSpan _warningTime;
        protected int _spawnCounter;
        protected Color _warningColour;
        private bool _colorGoingDown;
    }
}
