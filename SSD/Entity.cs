using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SSD
{
    abstract class Entity
    {

        public Entity(Vector3 position, ModelContainer model, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0)
        {
            _position = position;
            _model = model;
            ////_matrix = Matrix.Identity;
            //_position = Vector3.Zero;
            _rotation = Quaternion.Identity;

            _scale = scale;
            _yaw = MathHelper.ToRadians(yaw);
            _pitch = MathHelper.ToRadians(pitch);
            _roll = MathHelper.ToRadians(roll);
            _isAlive = true;
            //_boundingSphere = new BoundingSphere(_position, scale);

        }

        virtual public Matrix getMatrix()
        {
            Matrix transform = Matrix.Identity;

            Quaternion accumulateRotation = getRotation() /* * Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, _roll) */;

            //Doing Scale->Translate->Rotation - things will orbit in the game so translate and rotate is ideal
            transform *= Matrix.CreateScale(_scale);
            transform *= Matrix.CreateTranslation(_position);
            transform *= Matrix.CreateFromQuaternion(accumulateRotation);

            //transform *= _matrix;
            return transform;
        }

        virtual public void update(TimeSpan deltaTime)
        {
            //foreach (ModelMesh mesh in getModelContainer().getModel().Meshes)
            //{
            //    updateBoundingSphere(getMatrix().Translation, mesh.BoundingSphere.Radius * _scale);
            //}
        }

        virtual public void draw(Matrix view, Matrix proj, GraphicsDevice graphicsDevice)
        {
            foreach (ModelMesh mesh in getModelContainer().getModel().Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = getModelContainer().getBoneTransform(mesh.ParentBone.Index) * getMatrix();
                        effect.View = view;
                        effect.Projection = proj;

                        //effect.LightingEnabled = true; // turn on the lighting subsystem.
                        //effect.DirectionalLight0.Enabled = true;
                        //effect.DirectionalLight1.Enabled = true;
                        //effect.DirectionalLight2.Enabled = true;

                        //effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0, 0); // a red light
                        //effect.DirectionalLight0.Direction = new Vector3(1, 0, 0);  // coming along the x-axis
                        //effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0); // with green highlights

                        //effect.DirectionalLight1.DiffuseColor = new Vector3(0, 0.5f, 0); // a red light1
                        //effect.DirectionalLight1.Direction = new Vector3(0, 1, 0);  // coming along the x-axis
                        //effect.DirectionalLight1.SpecularColor = new Vector3(0, 1, 0); // with green highlights

                        //effect.DirectionalLight2.DiffuseColor = new Vector3(0, 0, 0.5f); // a red light
                        //effect.DirectionalLight2.Direction = new Vector3(0, 0, 1);  // coming along the x-axis
                        //effect.DirectionalLight2.SpecularColor = new Vector3(0, 1, 0); // with green highlights

                    }
                    //BoundingSphereRenderer.Render(entity.getBoundingSphere(), _graphicsDevice, view, proj, Color.Red);
                    mesh.Draw();
                }
            }
        }

        //public Matrix getTransformMatrix()
        //{
        //    return _matrix;
        //}

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

        //public void applyMatrix(Matrix mat)
        //{
        //    //_position = mat.Translation;
        //    //_rotation *= Quaternion.CreateFromRotationMatrix(mat);

        //    _matrix *= mat; //Acumulating...
        //}

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

        public float getScale(){
            return _scale;
        }

        public void setAlive(bool isAlive)
        {
            _isAlive = isAlive;
        }

        public bool getAlive()
        {
            return _isAlive;
        }

        //public void updateBoundingSphere(Vector3 position, float radius){
        //    _boundingSphere.Radius = radius;
        //    _boundingSphere.Center = position;
        //}

        //public BoundingSphere getBoundingSphere()
        //{
        //    return _boundingSphere;
        //}

        ModelContainer _model;
        //Matrix _matrix;
        float _scale;
        float _yaw;
        float _pitch;
        float _roll;
        //BoundingSphere _boundingSphere;
        bool _isAlive;

        //Vector3 _orbitPosition;
        //Quaternion _orbitRotation;

        Vector3 _position;
        Quaternion _rotation;
    }
}
