using Microsoft.Xna.Framework;
using System;

namespace SSD
{
    class EnemyEntity : ModelEntity
    {
        public EnemyEntity(Vector3 position, ModelContainer model, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0)
            : base(position, model, scale, yaw, pitch, roll)
        {
            base.setFriendly(false);
            _isSpawning = true;
        }

        //override public Matrix getMatrix()
        //{
        //    Matrix transform = Matrix.Identity;

        //    Quaternion accumulateRotation = base.getRotation();

        //    transform *= Matrix.CreateScale(base.getScale());
        //    transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(this.getYaw(), this.getPitch(), this.getRoll()));
        //    transform *= Matrix.CreateTranslation(base.getPosition());
        //    transform *= Matrix.CreateFromQuaternion(this.getRawRotation());
        //    //transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(base.getMatrix().Forward, MathHelper.ToRadians(0.1f)));

        //    //setPosition(transform.Translation);
        //    //addRotation(Quaternion.CreateFromAxisAngle(base.getMatrix().Forward, MathHelper.ToRadians(0.1f)));

        //    return transform;
        //}

        //override public void update(TimeSpan deltaTime)
        //{
        //    base.update(deltaTime);
        //}

        public void setIsSpawning(bool isSpawning)
        {
            _isSpawning = isSpawning;
        }

        protected bool _isSpawning;

    }
}
