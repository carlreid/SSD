using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSD
{
    class DifficultyManager
    {
        public DifficultyManager()
        {
            _enemyDensity = 1.0f;
            _playerShootSpeed = 1.0f;
            _powerUpChance = 1.0f;
            _bulletDamage = 1.0f;
            _bonusElementalDamage = 1.0f;
        }

        public void setEasy()
        {
            _enemyDensity = 0.5f;
            _playerShootSpeed = 0.5f;
            _powerUpChance = 0.5f;
            _bulletDamage = 2.0f;
            _bonusElementalDamage = 4.0f;
        }

        public void setMedium()
        {
            _enemyDensity = 1.0f;
            _playerShootSpeed = 1.0f;
            _powerUpChance = 1.0f;
            _bulletDamage = 1.0f;
            _bonusElementalDamage = 1.0f;
        }

        public void setHard()
        {
            _enemyDensity = 2.0f;
            _playerShootSpeed = 1.5f;
            _powerUpChance = 8.0f;
            _bulletDamage = 0.5f;
            _bonusElementalDamage = 3.0f;
        }

        public float _enemyDensity {get; set;}
        public float _playerShootSpeed { get; set; }
        public float _powerUpChance { get; set; }
        public float _bulletDamage { get; set; }
        public float _bonusElementalDamage { get; set; }

    }
}
