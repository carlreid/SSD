using Microsoft.Xna.Framework;
using System;

namespace SSD
{
    abstract class GameLevel
    {
        public GameLevel(SpawnManager spawnManager, Random randomGen, DifficultyManager difficultyManager)
        {
            _spawnManager = spawnManager;
            _currentPhase = 0;
            _randomGen = randomGen;
            _drawWarning = false;
            _warningTime = TimeSpan.FromSeconds(0);
            _spawnCounter = 0;
            _warningColour = Color.Red;
            _colorGoingDown = true;
            _difficultyManager = difficultyManager;
            _isEndGame = false;
        }

        public virtual void update(GameTime gameTime, Entity targetPlanet, PlayerEntity player)
        {
            _levelTime += gameTime.ElapsedGameTime;
            //_spawnManager.update(gameTime, targetPlanet);
        }

        public TimeSpan timePlayed(){
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

        public string getPlanetModelString()
        {
            return _planetModel;
        }

        public bool isEndGame()
        {
            return _isEndGame;
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
        protected string _planetModel;
        protected DifficultyManager _difficultyManager;
        protected bool _isEndGame;
    }
}
