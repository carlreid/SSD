using Microsoft.Xna.Framework;
using System;

namespace SSD
{
    class LaveLevel : GameLevel
    {
        public LaveLevel(SpawnManager spawnManager, Random randomGen) :
            base(spawnManager, randomGen)
        {
            //base._levelTime = TimeSpan.FromSeconds(532);
            base._levelTime = TimeSpan.FromSeconds(110);
            _currentPhase = 8;
        }

        override public void update(GameTime gameTime, Entity targetPlanet)
        {

            return;

            base.update(gameTime, targetPlanet);

            if (_levelTime < TimeSpan.FromSeconds(5) && _currentPhase == 0)
            {
                _spawnManager.spawnRocks(2, targetPlanet);
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(5) && _levelTime < TimeSpan.FromSeconds(15) && _currentPhase == 1)
            {
                _spawnManager.spawnRocks(6, targetPlanet);
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(15) && _levelTime < TimeSpan.FromSeconds(25) && _currentPhase == 2)
            {
                _spawnManager.spawnRocks(12, targetPlanet);
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(30) && _levelTime < TimeSpan.FromSeconds(45) && _currentPhase == 3)
            {
                for (int spawnAmount = 0; spawnAmount < 20; ++spawnAmount)
                {
                    switch (_randomGen.Next(1, 3))
                    {
                        case 1:
                            _spawnManager.spawnRocks(1, targetPlanet);
                            break;
                        case 2:
                            _spawnManager.spawnMines(1, targetPlanet);
                            break;
                        default:
                            break;
                    }
                }
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(45) && _levelTime < TimeSpan.FromSeconds(55) && _currentPhase == 4)
            {
                if (!_drawWarning)
                {
                    _warningTime = TimeSpan.FromSeconds(5);
                    _drawWarning = true;
                }
                else
                {
                    _warningTime -= gameTime.ElapsedGameTime;
                }

                if (_warningTime <= TimeSpan.FromSeconds(0))
                {
                    _spawnManager.spawnMines(80, targetPlanet);
                    ++_currentPhase;
                    _drawWarning = false;
                    _warningTime = TimeSpan.FromSeconds(0);
                }
            }
            else if (_levelTime > TimeSpan.FromSeconds(80) && _levelTime < TimeSpan.FromSeconds(90) && _currentPhase == 5)
            {
                _spawnManager.spawnRocks(10, targetPlanet);
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(90) && _levelTime < TimeSpan.FromSeconds(100) && _currentPhase == 6)
            {
                _spawnManager.spawnRocks(12, targetPlanet);
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(100) && _levelTime < TimeSpan.FromSeconds(110) && _currentPhase == 7)
            {
                _spawnManager.spawnRocks(6, targetPlanet);
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(110) && _levelTime < TimeSpan.FromSeconds(130) && _currentPhase == 8)
            {
                if (!_drawWarning && _spawnCounter == 0)
                {
                    _warningTime = TimeSpan.FromSeconds(5);
                    _drawWarning = true;
                }
                else
                {
                    _warningTime -= gameTime.ElapsedGameTime;
                }

                if (_spawnCounter < 50 && _warningTime <= TimeSpan.FromSeconds(0))
                {
                    _drawWarning = false;
                    _spawnManager.spawnRocks(1, targetPlanet, 0, 0, _spawnCounter * 7);
                    ++_spawnCounter;
                }
                else if (_spawnCounter >= 50)
                {
                    _drawWarning = false;
                    _warningTime = TimeSpan.FromSeconds(0);
                    _spawnCounter = 0;
                    ++_currentPhase;
                }
            }
            else if (_levelTime > TimeSpan.FromSeconds(120) && _levelTime < TimeSpan.FromSeconds(150) && _currentPhase == 9)
            {
                if (!_drawWarning && _spawnCounter == 0)
                {
                    _warningTime = TimeSpan.FromSeconds(15);
                    _drawWarning = true;
                }
                else
                {
                    _warningTime -= gameTime.ElapsedGameTime;
                }

                if (_spawnCounter < 50 && _warningTime <= TimeSpan.FromSeconds(0))
                {
                    _drawWarning = false;
                    _spawnManager.spawnMines(1, targetPlanet, _spawnCounter * 2, _spawnCounter * 5, _spawnCounter * 7);
                    ++_spawnCounter;
                }
                else if (_spawnCounter >= 50)
                {
                    _drawWarning = false;
                    _warningTime = TimeSpan.FromSeconds(0);
                    _spawnCounter = 0;
                    ++_currentPhase;
                }
            }
            else if (_levelTime > TimeSpan.FromSeconds(150) && _levelTime < TimeSpan.FromSeconds(180) && _currentPhase == 10)
            {
                for (int spawnAmount = 0; spawnAmount < 40; ++spawnAmount)
                {
                    switch (_randomGen.Next(1, 3))
                    {
                        case 1:
                            _spawnManager.spawnRocks(1, targetPlanet);
                            break;
                        case 2:
                            _spawnManager.spawnMines(1, targetPlanet);
                            break;
                        default:
                            break;
                    }
                }
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(180) && _levelTime < TimeSpan.FromSeconds(200) && _currentPhase == 11)
            {
                for (int spawnAmount = 0; spawnAmount < 80; ++spawnAmount)
                {
                    switch (_randomGen.Next(1, 3))
                    {
                        case 1:
                            _spawnManager.spawnRocks(1, targetPlanet);
                            break;
                        case 2:
                            _spawnManager.spawnMines(1, targetPlanet);
                            break;
                        default:
                            break;
                    }
                }
                ++_currentPhase;
            }

        }

    }
}
