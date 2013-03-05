using Microsoft.Xna.Framework;

namespace SSD
{
    class Bullet : Entity
    {
        public Bullet(Vector3 position, float yaw, ModelContainer bulletModel)
            : base(position, bulletModel, 5)
        {
            //_position = startMatrix.Translation;
            _direction = yaw;
            
        }

        //override public Matrix getMatrix()
        //{
        //    Matrix transform = Matrix.Identity;

        //    Quaternion accumulateRotation = base.getRotation() * Quaternion.CreateFromYawPitchRoll(base.getYaw(), base.getPitch(), base.getRoll());

        //    //Doing Scale->Translate->Rotation - things will orbit in the game so translate and rotate is ideal
        //    transform *= Matrix.CreateScale(base.getScale());
        //    transform *= Matrix.CreateFromQuaternion(accumulateRotation);
        //    transform *= Matrix.CreateTranslation(base.getPosition());

        //    transform *= base.getTransformMatrix();
        //    return transform;
        //}

        public void update()
        {
            //base.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Backward, 0.1f));
            //base.setPosition((Matrix.CreateTranslation(_position) * Matrix.CreateRotationZ(MathHelper.ToRadians(_curAngle)) * Matrix.CreateRotationX(MathHelper.ToRadians(_curAngle)) * Matrix.CreateRotationY(MathHelper.ToRadians(_curAngle))).Translation);
            //base.addPitch(2);
            //_curAngle += 3f;

            base.setYaw(MathHelper.ToDegrees(_direction));
            base.addRotation(Quaternion.CreateFromAxisAngle(base.getMatrix().Forward, 0.01f));
            base.setYaw(MathHelper.ToDegrees(0));
        }

        float _velocity;
        float _direction;
        //Vector3 _position;
    }
}
