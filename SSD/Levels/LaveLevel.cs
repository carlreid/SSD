using Microsoft.Xna.Framework;
using System;

namespace SSD
{
    class LaveLevel : GameLevel
    {
        public LaveLevel(SpawnManager spawnManager, Random randomGen, DifficultyManager difficultyManager) :
            base(spawnManager, randomGen, difficultyManager)
        {
            //base._levelTime = TimeSpan.FromSeconds(532);
            //base._levelTime = TimeSpan.FromSeconds(320);
            //_currentPhase = 16;
            _planetModel = "worldSphere";
        }

        override public void update(GameTime gameTime, Entity targetPlanet, PlayerEntity player)
        {
            base.update(gameTime, targetPlanet, player);

            if (_levelTime < TimeSpan.FromSeconds(5) && _currentPhase == 0)
            {
                _spawnManager.spawnRocks((int)(2 * _difficultyManager._enemyDensity), targetPlanet);
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(5) && _levelTime < TimeSpan.FromSeconds(15) && _currentPhase == 1)
            {
                _spawnManager.spawnRocks((int)(6 * _difficultyManager._enemyDensity), targetPlanet);
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(15) && _levelTime < TimeSpan.FromSeconds(25) && _currentPhase == 2)
            {
                _spawnManager.spawnRocks((int)(12 * _difficultyManager._enemyDensity), targetPlanet);
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(30) && _levelTime < TimeSpan.FromSeconds(45) && _currentPhase == 3)
            {
                for (int spawnAmount = 0; spawnAmount < (int)(20 * _difficultyManager._enemyDensity); ++spawnAmount)
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
                    _spawnManager.spawnMines((int)(80 * _difficultyManager._enemyDensity), targetPlanet);
                    ++_currentPhase;
                    _drawWarning = false;
                    _warningTime = TimeSpan.FromSeconds(0);
                }
            }
            else if (_levelTime > TimeSpan.FromSeconds(80) && _levelTime < TimeSpan.FromSeconds(90) && _currentPhase == 5)
            {
                _spawnManager.spawnRocks((int)(10 * _difficultyManager._enemyDensity), targetPlanet);
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(90) && _levelTime < TimeSpan.FromSeconds(100) && _currentPhase == 6)
            {
                _spawnManager.spawnRocks((int)(12 * _difficultyManager._enemyDensity), targetPlanet);
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(100) && _levelTime < TimeSpan.FromSeconds(110) && _currentPhase == 7)
            {
                _spawnManager.spawnRocks((int)(6 * _difficultyManager._enemyDensity), targetPlanet);
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

                if (_spawnCounter < (int)(50 * _difficultyManager._enemyDensity) && _warningTime <= TimeSpan.FromSeconds(0))
                {
                    _drawWarning = false;
                    _spawnManager.spawnRocks(1, targetPlanet, 0, 0, _spawnCounter * 7);
                    ++_spawnCounter;
                }
                else if (_spawnCounter >= (int)(50 * _difficultyManager._enemyDensity))
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

                if (_spawnCounter < (int)(50 * _difficultyManager._enemyDensity) && _warningTime <= TimeSpan.FromSeconds(0))
                {
                    _drawWarning = false;
                    _spawnManager.spawnMines(1, targetPlanet, _spawnCounter * 2, _spawnCounter * 5, _spawnCounter * 7);
                    ++_spawnCounter;
                }
                else if (_spawnCounter >= (int)(50 * _difficultyManager._enemyDensity))
                {
                    _drawWarning = false;
                    _warningTime = TimeSpan.FromSeconds(0);
                    _spawnCounter = 0;
                    ++_currentPhase;
                }
            }
            else if (_levelTime > TimeSpan.FromSeconds(150) && _levelTime < TimeSpan.FromSeconds(180) && _currentPhase == 10)
            {
                for (int spawnAmount = 0; spawnAmount < (int)(40 * _difficultyManager._enemyDensity); ++spawnAmount)
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
                for (int spawnAmount = 0; spawnAmount < (int)(80 * _difficultyManager._enemyDensity); ++spawnAmount)
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
            else if (_levelTime > TimeSpan.FromSeconds(200) && _levelTime < TimeSpan.FromSeconds(220) && _currentPhase == 12)
            {
                if (!_drawWarning && _spawnCounter == 0)
                {
                    _warningTime = TimeSpan.FromSeconds(10);
                    _drawWarning = true;
                }
                else
                {
                    _warningTime -= gameTime.ElapsedGameTime;
                }

                if (_spawnCounter < (int)(10 * _difficultyManager._enemyDensity) && _warningTime <= TimeSpan.FromSeconds(0))
                {
                    _drawWarning = false;
                    _spawnManager.spawnPushPull(1, targetPlanet, player);
                    ++_spawnCounter;
                }
                else if (_spawnCounter >= (int)(10 * _difficultyManager._enemyDensity))
                {
                    _drawWarning = false;
                    _warningTime = TimeSpan.FromSeconds(0);
                    _spawnCounter = 0;
                    ++_currentPhase;
                }
            }
            else if (_levelTime > TimeSpan.FromSeconds(220) && _levelTime < TimeSpan.FromSeconds(260) && _currentPhase == 13)
            {
                for (int spawnAmount = 0; spawnAmount < (int)(80 * _difficultyManager._enemyDensity); ++spawnAmount)
                {
                    switch (_randomGen.Next(1, 4))
                    {
                        case 1:
                            _spawnManager.spawnRocks(1, targetPlanet);
                            break;
                        case 2:
                            _spawnManager.spawnMines(1, targetPlanet);
                            break;
                        case 3:
                            _spawnManager.spawnPushPull(1, targetPlanet, player);
                            break;
                        default:
                            break;
                    }
                }
                ++_currentPhase;
            }
            else if (_levelTime > TimeSpan.FromSeconds(260) && _levelTime < TimeSpan.FromSeconds(300) && _currentPhase == 14)
            {
                if (!_drawWarning && _spawnCounter == 0)
                {
                    _warningTime = TimeSpan.FromSeconds(20);
                    _drawWarning = true;
                }
                else
                {
                    _warningTime -= gameTime.ElapsedGameTime;
                }

                if (_spawnCounter < (int)(80 * _difficultyManager._enemyDensity) && _warningTime <= TimeSpan.FromSeconds(0))
                {
                    _drawWarning = false;
                    _spawnManager.spawnMines(1, targetPlanet);
                    ++_spawnCounter;
                }
                else if (_spawnCounter >= (int)(80 * _difficultyManager._enemyDensity))
                {
                    _drawWarning = false;
                    _warningTime = TimeSpan.FromSeconds(0);
                    _spawnCounter = 0;
                    ++_currentPhase;
                }
            }
            else if (_levelTime > TimeSpan.FromSeconds(300) && _levelTime < TimeSpan.FromSeconds(320) && _currentPhase == 15)
            {
                if (!_drawWarning && _spawnCounter == 0)
                {
                    _warningTime = TimeSpan.FromSeconds(20);
                    _drawWarning = true;
                }
                else
                {
                    _warningTime -= gameTime.ElapsedGameTime;
                }

                if (_spawnCounter < (int)(80 * _difficultyManager._enemyDensity) && _warningTime <= TimeSpan.FromSeconds(0))
                {
                    _drawWarning = false;
                    _spawnManager.spawnMines(1, targetPlanet);
                    ++_spawnCounter;
                }
                else if (_spawnCounter >= (int)(80 * _difficultyManager._enemyDensity))
                {
                    _drawWarning = false;
                    _warningTime = TimeSpan.FromSeconds(0);
                    _spawnCounter = 0;
                    ++_currentPhase;
                }
            }
            else if (_levelTime > TimeSpan.FromSeconds(320) && _levelTime < TimeSpan.FromSeconds(360) && _currentPhase == 16)
            {
                if (!_drawWarning && _spawnCounter == 0)
                {
                    _warningTime = TimeSpan.FromSeconds(1); //30
                    _drawWarning = true;
                }
                else
                {
                    _warningTime -= gameTime.ElapsedGameTime;
                }

                if (_warningTime <= TimeSpan.FromSeconds(0))
                {
                    _drawWarning = false;
                    _warningTime = TimeSpan.FromSeconds(0);
                    _spawnManager.spawnIceBoss(1, targetPlanet, player);
                    ++_currentPhase;
                }
            }
            else if (_currentPhase == 17)
            {
                _isEndGame = true;
                ++_currentPhase;
            }

        }

    }
}
