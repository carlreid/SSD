using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace SSD
{
    class SpawnManager
    {

        public SpawnManager(ref List<Entity> worldEntities, ref Draw renderer){
            _worldEntities = worldEntities;
            _renderer = renderer;
            _randomGen = new Random();
            _spawnAmount = 1;
        }

        public void update(GameTime gameTime, Entity targetPlanet)
        {
            _elapsedTime += gameTime.ElapsedGameTime;
            _timeLastSpawn -= gameTime.ElapsedGameTime;

            if (_timeLastSpawn <= TimeSpan.Zero)
            {
                if (_spawnAmount < 15)
                {
                    //Exponential growth - maybe needs tweaking
                    _spawnAmount = Math.Pow(1.05, _elapsedTime.Seconds);
                }

                switch (_randomGen.Next(1, 3))
                {
                    case 1:
                        spawnRocks((int)_spawnAmount, targetPlanet);
                        break;
                    case 2:
                        spawnMines((int)_spawnAmount, targetPlanet);
                        break;
                    default:
                        break;
                }

                
                _timeLastSpawn = TimeSpan.FromSeconds(_randomGen.Next(1, 5));
                Debug.WriteLine(_spawnAmount);
            }

        }

        public void spawnRocks(int amount, Entity targetPlanet)
        {
            for (int enemyCount = 0; enemyCount < amount; ++enemyCount)
            {
                _worldEntities.Add(new EnemyRock((Matrix.CreateTranslation(0, _randomGen.Next(600, 1000), 0) * Matrix.CreateRotationX(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                                                                                       * Matrix.CreateRotationY(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                                                                                       * Matrix.CreateRotationZ(MathHelper.ToRadians(_randomGen.Next(0, 360)))).Translation,
                                                                                       _renderer.getModel("e_rock"), targetPlanet, _randomGen, (float)_randomGen.NextDouble() + 0.2f));
            }
        }

        public void spawnMines(int amount, Entity targetPlanet)
        {
            for (int enemyCount = 0; enemyCount < amount; ++enemyCount)
            {
                _worldEntities.Add(new EnemyMine((Matrix.CreateTranslation(0, _randomGen.Next(600, 1000), 0) * Matrix.CreateRotationX(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                                                                                       * Matrix.CreateRotationY(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                                                                                       * Matrix.CreateRotationZ(MathHelper.ToRadians(_randomGen.Next(0, 360)))).Translation,
                                                                                       _renderer.getModel("e_mine"), targetPlanet, _randomGen, 0.2f));
            }
        }

        List<Entity> _worldEntities;
        Draw _renderer;
        Random _randomGen;
        TimeSpan _elapsedTime = TimeSpan.Zero;
        TimeSpan _timeLastSpawn = TimeSpan.Zero;
        double _spawnAmount;
    }
}
