using Microsoft.Xna.Framework;

namespace SSD
{
    class PlayerEntity : ModelEntity
    {
        public PlayerEntity(Vector3 position, ModelContainer model, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0)
            : base(position, model, scale, yaw, pitch, roll)
        {

        }


    }
}
