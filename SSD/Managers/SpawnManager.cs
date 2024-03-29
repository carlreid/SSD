﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace SSD
{
    class SpawnManager
    {

        public SpawnManager(ref List<Entity> worldEntities, ref Draw renderer, ref SoundManager soundManager){
            _worldEntities = worldEntities;
            _renderer = renderer;
            _soundManager = soundManager;
            _randomGen = new Random();
            _spawnAmount = 1;
        }

        public void update(GameTime gameTime, Entity targetPlanet, PlayerEntity player)
        {
            _elapsedTime += gameTime.ElapsedGameTime;
            _timeLastSpawn -= gameTime.ElapsedGameTime;

            if (_worldEntities.Count > 100)
            {
                return;
            }

            if (_timeLastSpawn <= TimeSpan.Zero)
            {
                if (_spawnAmount < 5)
                {
                    //Exponential growth - maybe needs tweaking
                    _spawnAmount = Math.Pow(1.0001, _elapsedTime.Seconds);
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

                //_worldEntities.Add(new EnemyPushPull((Matrix.CreateTranslation(0, _randomGen.Next(600, 1000), 0) * Matrix.CreateRotationX(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                //                                                       * Matrix.CreateRotationY(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                //                                                       * Matrix.CreateRotationZ(MathHelper.ToRadians(_randomGen.Next(0, 360)))).Translation,
                //                                                       _renderer.getModel("e_pushPull"), targetPlanet, player, _randomGen, 1f));

                //_worldEntities.Add(new EnemyTurret((Matrix.CreateTranslation(0, _randomGen.Next(600, 1000), 0) * Matrix.CreateRotationX(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                //                                                       * Matrix.CreateRotationY(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                //                                                       * Matrix.CreateRotationZ(MathHelper.ToRadians(_randomGen.Next(0, 360)))).Translation,
                //                                                       _renderer.getModel("e_mine"), targetPlanet, _randomGen, 0.2f));

                //_worldEntities.Add(new EnemyTurret(Matrix.CreateTranslation(0, _randomGen.Next(600, 1000), 0).Translation, _renderer.getModel("e_mine"), targetPlanet, _randomGen, 0.2f));
                //_worldEntities[_worldEntities.Count - 1].setRotation(Quaternion.CreateFromYawPitchRoll(0, 90, 0));

                _timeLastSpawn = TimeSpan.FromSeconds(_randomGen.Next(1, 5));
                //Debug.WriteLine(_spawnAmount);
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
                _soundManager.addAttatchment(LoadedSounds.ROCK_ENEMY_MOVE, _worldEntities[_worldEntities.Count - 1]);
            }
        }

        public void spawnRocks(int amount, Entity targetPlanet, int xRot, int yRot, int zRot)
        {
            for (int enemyCount = 0; enemyCount < amount; ++enemyCount)
            {
                _worldEntities.Add(new EnemyRock((Matrix.CreateTranslation(0, _randomGen.Next(600, 1000), 0) * Matrix.CreateRotationX(MathHelper.ToRadians(xRot))
                                                                                       * Matrix.CreateRotationY(MathHelper.ToRadians(yRot))
                                                                                       * Matrix.CreateRotationZ(MathHelper.ToRadians(zRot))).Translation,
                                                                                       _renderer.getModel("e_rock"), targetPlanet, _randomGen, (float)_randomGen.NextDouble() + 0.2f));
                _soundManager.addAttatchment(LoadedSounds.ROCK_ENEMY_MOVE, _worldEntities[_worldEntities.Count - 1]);
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
                _soundManager.addAttatchment(LoadedSounds.MINE_ENEMY_MOVE, _worldEntities[_worldEntities.Count - 1]);
            }
        }

        public void spawnMines(int amount, Entity targetPlanet, int xRot, int yRot, int zRot)
        {
            for (int enemyCount = 0; enemyCount < amount; ++enemyCount)
            {
                _worldEntities.Add(new EnemyMine((Matrix.CreateTranslation(0, _randomGen.Next(600, 1000), 0) * Matrix.CreateRotationX(MathHelper.ToRadians(xRot))
                                                                                       * Matrix.CreateRotationY(MathHelper.ToRadians(yRot))
                                                                                       * Matrix.CreateRotationZ(MathHelper.ToRadians(zRot))).Translation,
                                                                                       _renderer.getModel("e_mine"), targetPlanet, _randomGen, 0.2f));
                _soundManager.addAttatchment(LoadedSounds.MINE_ENEMY_MOVE, _worldEntities[_worldEntities.Count - 1]);
            }
        }

        public void spawnPushPull(int amount, Entity targetPlanet, PlayerEntity player)
        {
            _worldEntities.Add(new EnemyPushPull((Matrix.CreateTranslation(0, _randomGen.Next(600, 1000), 0) * Matrix.CreateRotationX(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                                                                   * Matrix.CreateRotationY(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                                                                   * Matrix.CreateRotationZ(MathHelper.ToRadians(_randomGen.Next(0, 360)))).Translation,
                                                                   _renderer.getModel("e_pushPull"), targetPlanet, player, _randomGen, 1f));
        }

        public void spawnPushPull(int amount, Entity targetPlanet, PlayerEntity player, int xRot, int yRot, int zRot)
        {
            _worldEntities.Add(new EnemyPushPull((Matrix.CreateTranslation(0, _randomGen.Next(600, 1000), 0) * Matrix.CreateRotationX(xRot)
                                                                   * Matrix.CreateRotationY(yRot)
                                                                   * Matrix.CreateRotationZ(zRot)).Translation,
                                                                   _renderer.getModel("e_pushPull"), targetPlanet, player, _randomGen, 1f));
        }

        public void spawnIceBoss(int amount, Entity targetPlanet, PlayerEntity player)
        {
            _worldEntities.Add(new EnemyIceBoss((Matrix.CreateTranslation(0, _randomGen.Next(600, 1000), 0) * Matrix.CreateRotationX(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                                                       * Matrix.CreateRotationY(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                                                       * Matrix.CreateRotationZ(MathHelper.ToRadians(_randomGen.Next(0, 360)))).Translation,
                                                       _renderer.getModel("e_iceBossHead"), _renderer.getModel("e_iceBossTail"), targetPlanet, player, _randomGen, 1f));
        }

        List<Entity> _worldEntities;
        Draw _renderer;
        SoundManager _soundManager;
        Random _randomGen;
        TimeSpan _elapsedTime = TimeSpan.Zero;
        TimeSpan _timeLastSpawn = TimeSpan.Zero;
        double _spawnAmount;
    }
}
