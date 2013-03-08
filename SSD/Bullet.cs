using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace SSD
{
    class Bullet : Entity
    {
        public Bullet(Vector3 position, Quaternion rotation, float yaw, ModelContainer bulletModel)
            : base(position, bulletModel, 1, yaw)
        {
            //_position = startMatrix.Translation;
            //_direction = new Vector3((float)Math.Sin(yaw), (float)Math.Cos(yaw), 0);
            //_directionYaw = yaw;

            base.addRotation(rotation);

            _curAngle = 0;
            _angleModifier = 0.5f;
            _timeTillDeath = 1000;
            _isAlive = true;
        }

        override public Matrix getMatrix()
        {
            Matrix transform = Matrix.Identity;

            Quaternion accumulateRotation = base.getRotation();

            transform *= Matrix.CreateScale(base.getScale());
            transform *= Matrix.CreateFromQuaternion(accumulateRotation);
            transform *= Matrix.CreateTranslation(base.getPosition());
            transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(base.getMatrix().Forward, MathHelper.ToRadians(_curAngle)));

            return transform;
        }

        public void update(TimeSpan deltaTime)
        {

            _curAngle += _angleModifier;

            if (_angleModifier > 0.05f)
            {
                _angleModifier -= 0.018f;
                if (_angleModifier < 0.05f)
                {
                    _angleModifier = 0.05f;
                }
            }
            

            _timeTillDeath -= deltaTime.Milliseconds;
            if (_timeTillDeath <= 0)
            {
                _isAlive = false;
            }
        }

        public bool getAlive()
        {
            return _isAlive;
        }

        //float _velocity;
        Vector3 _direction;
        float _directionYaw;
        float _timeTillDeath;
        bool _isAlive;

        float _curAngle;
        float _angleModifier;

        //Vector3 _position;
    }
}
