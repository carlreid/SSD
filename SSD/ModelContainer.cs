using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace SSD
{
    class ModelContainer
    {

        public ModelContainer(string modelName, ContentManager Content)
        {
            _model = Content.Load<Model>(modelName);
            _boneTransforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(_boneTransforms);
            _matrix = Matrix.Identity;


            _scale = 1;
            _yaw = 0;
            _pitch = 0;
            _roll = 0;

            _position = Vector3.Zero;
            _rotation = Quaternion.Identity;

        }

        public Model getModel()
        {
            return _model;
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

        public Matrix getBoneTransform(int index){
            return _boneTransforms[index];
        }

        public void applyMatrix(Matrix mat){
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

        Model _model;
        Matrix _matrix;
        Matrix[] _boneTransforms;

        float _scale;
        float _yaw;
        float _pitch;
        float _roll;

        Vector3 _position;
        Quaternion _rotation;

    }
}
