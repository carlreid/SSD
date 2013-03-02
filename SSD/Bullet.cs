using Microsoft.Xna.Framework;

namespace SSD
{
    class Bullet
    {
        public Bullet(Vector3 position)
        {
            _position = position;
        }

        float _velocity;
        float _direction;
        Vector3 _position;
    }
}
