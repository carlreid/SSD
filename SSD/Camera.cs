using Microsoft.Xna.Framework;

namespace SSD
{
    class Camera
    {
        public Camera(Vector3 position, Vector3 lookAt, Vector3 up)
        {
            _position = position;
            _lookAt = lookAt;
            _up = up;
        }

        public void setPosition(Vector3 position){
            _position = position;
        }

        public void setLookAt(Vector3 lookAt)
        {
            _lookAt = lookAt;
        }

        public Matrix getLookAt(){
            return Matrix.CreateLookAt(_position, _lookAt, _up);
        }

        /// <summary>
        /// Roll (around forward axis) clockwise with respect to camera
        /// TODO: Is it CW or CCW?
        /// </summary>
        /// <param name="amount">Angle in degrees</param>
        public void Roll(float amount)
        {
            _up.Normalize();
            var left = Vector3.Cross(_up, _lookAt);
            left.Normalize();

            _up = Vector3.Transform(_up, Matrix.CreateFromAxisAngle(_lookAt, MathHelper.ToRadians(amount)));
        }

        /// <summary>
        /// Yaw (turn/steer around up vector) to the left
        /// </summary>
        /// <param name="amount">Angle in degrees</param>
        public void Yaw(float amount)
        {
            _lookAt.Normalize();

            _lookAt = Vector3.Transform(_lookAt, Matrix.CreateFromAxisAngle(_up, MathHelper.ToRadians(amount)));
        }

        /// <summary>
        /// Pitch (around leftward axis) upward
        /// </summary>
        /// <param name="amount"></param>
        public void Pitch(float amount)
        {
            _lookAt.Normalize();
            var left = Vector3.Cross(_up, _lookAt);
            left.Normalize();

            _lookAt = Vector3.Transform(_lookAt, Matrix.CreateFromAxisAngle(left, MathHelper.ToRadians(amount)));
            _up = Vector3.Transform(_up, Matrix.CreateFromAxisAngle(left, MathHelper.ToRadians(amount)));
        }

        Vector3 _up;
        Vector3 _lookAt;
        Vector3 _position;

    }
}
