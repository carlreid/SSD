﻿using Microsoft.Xna.Framework;

namespace SSD
{
    class Entity
    {

        public Entity(Vector3 position, ModelContainer model, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0)
        {
            _position = position;
            _model = model;
            _matrix = Matrix.Identity;
            //_position = Vector3.Zero;
            _rotation = Quaternion.Identity;

            _scale = scale;
            _yaw = yaw;
            _pitch = pitch;
            _roll = roll;
        }

        public Matrix getMatrix()
        {
            Matrix transform = Matrix.Identity;

            Quaternion accumulateRotation = _rotation * Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, _roll);

            //Doing Scale->Translate->Rotation - things will orbit in the game so translate and rotate is ideal
            transform *= Matrix.CreateScale(_scale);
            transform *= Matrix.CreateTranslation(_position);
            transform *= Matrix.CreateFromQuaternion(accumulateRotation);

            transform *= _matrix;
            return transform;
        }

        public Vector3 getPosition()
        {
            return _position;
        }

        public void setScale(float scale)
        {
            _scale = scale;
        }

        public void setPosition(Vector3 pos)
        {
            _position = pos;
        }

        public void setPosition(float x, float y, float z)
        {
            _position = new Vector3(x, y, z);
        }

        public void setRotation(float yaw, float pitch, float roll)
        {
            _yaw = MathHelper.ToRadians(yaw);
            _pitch = MathHelper.ToRadians(pitch);
            _roll = MathHelper.ToRadians(roll);

            _rotation = Quaternion.CreateFromYawPitchRoll(_scale, _pitch, _roll);
        }

        public void addYaw(float yaw)
        {
            _yaw += MathHelper.ToRadians(yaw);
            //_rotation = Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
        }

        public void addPitch(float pitch)
        {
            _pitch += MathHelper.ToRadians(pitch);
            //_rotation = Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
        }

        public void addRoll(float roll)
        {
            _roll += MathHelper.ToRadians(roll);
            //_rotation = Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
        }

        public void setYaw(float yaw)
        {
            _yaw = MathHelper.ToRadians(yaw);
            //Debug.WriteLine("Got yaw of: " + yaw);
            //_rotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, yaw); 
        }

        public void setPitch(float pitch)
        {
            _pitch = MathHelper.ToRadians(pitch);
            //_rotation = Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
        }

        public void setRoll(float roll)
        {
            _roll = MathHelper.ToRadians(roll);
            //_rotation = Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
        }

        public void applyMatrix(Matrix mat)
        {
            //_position = mat.Translation;
            //_rotation *= Quaternion.CreateFromRotationMatrix(mat);

            _matrix *= mat; //Acumulating...
        }

        public float getYaw()
        {
            return _yaw;
        }

        public float getPitch()
        {
            return _pitch;
        }

        public float getRoll()
        {
            return _roll;
        }

        public void addRotation(Quaternion rotation)
        {
            _rotation *= rotation;
        }

        public Quaternion getRotation()
        {
            Quaternion accumulateRotation = _rotation * Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
            return accumulateRotation;
        }

        public ModelContainer getModelContainer()
        {
            return _model;
        }


        ModelContainer _model;
        Matrix _matrix;
        float _scale;
        float _yaw;
        float _pitch;
        float _roll;

        Vector3 _position;
        Quaternion _rotation;
    }
}