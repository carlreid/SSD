using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace SSD
{
    class SpawnManager
    {

        public SpawnManager(ref List<Entity> worldEntities, ref Draw renderer){
            _worldEntities = worldEntities;
            _renderer = renderer;
            _randomGen = new Random();
        }

        public void spawnRocks(int amount, Entity targetPlanet)
        {
            for (int enemyCount = 0; enemyCount < amount; ++enemyCount)
            {
                _worldEntities.Add(new EnemyRock((Matrix.CreateTranslation(0, _randomGen.Next(600, 1000), 0) * Matrix.CreateRotationX(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                                                                                       * Matrix.CreateRotationY(MathHelper.ToRadians(_randomGen.Next(0, 360)))
                                                                                       * Matrix.CreateRotationZ(MathHelper.ToRadians(_randomGen.Next(0, 360)))).Translation,
                                                                                       _renderer.getModel("e_one"), targetPlanet, _randomGen, (float)_randomGen.NextDouble() + 0.2f));
            }
        }

        List<Entity> _worldEntities;
        Draw _renderer;
        Random _randomGen;
    }
}
