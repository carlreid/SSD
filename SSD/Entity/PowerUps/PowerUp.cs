using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SSD
{
    abstract class PowerUp : Entity
    {
        public PowerUp(Vector3 position, ModelContainer model, int timeToAppear = 5000, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0)
            : base(position, model, scale, yaw, pitch, roll)
        {
            _boundingSphere = new BoundingSphere(position, scale);
            //_increaseBy = 0;
            //_timeTillRunsOut = 0;
            _runsOut = false;
            _hasBeenApplied = false;
            _timeToAppear = timeToAppear;
            _timeTillDespawn = _timeToAppear;
            _initialScale = getScale();
            //_powerUpColour = Color.White;
        }

        public override void draw(Matrix view, Matrix proj, GraphicsDevice graphicsDevice)
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
                        effect.DiffuseColor = _powerUpColour.ToVector3();
                        effect.EmissiveColor = _powerUpColour.ToVector3();
                    }
                    BoundingSphereRenderer.Render(getBoundingSphere(), graphicsDevice, view, proj, Color.Red);
                    mesh.Draw();
                }
            }
        }

        override public void update(TimeSpan deltaTime)
        {
            _timeTillDespawn -= deltaTime.Milliseconds;

            setScale(_initialScale * ((float)_timeTillDespawn / (float)_timeToAppear));

            if (_timeTillDespawn <= 0)
            {
                setAlive(false);
            }

            if (_hasBeenApplied && _runsOut)
            {
                //Reduce time
                _timeTillRunsOut -= deltaTime.Milliseconds;
            }

            foreach (ModelMesh mesh in getModelContainer().getModel().Meshes)
            {
                updateBoundingSphere(getMatrix().Translation, mesh.BoundingSphere.Radius * base.getScale());
            }
        }

        override public Matrix getMatrix()
        {
            Matrix transform = Matrix.Identity;

            Quaternion accumulateRotation = base.getRotation();

            transform *= Matrix.CreateScale(base.getScale());
            transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(this.getYaw(), this.getPitch(), this.getRoll()));
            transform *= Matrix.CreateTranslation(base.getPosition());
            transform *= Matrix.CreateFromQuaternion(this.getRawRotation());
            //transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(base.getMatrix().Forward, MathHelper.ToRadians(0.1f)));

            //setPosition(transform.Translation);
            //addRotation(Quaternion.CreateFromAxisAngle(base.getMatrix().Forward, MathHelper.ToRadians(0.1f)));

            return transform;
        }

        public void updateBoundingSphere(Vector3 position, float radius)
        {
            _boundingSphere.Radius = radius;
            _boundingSphere.Center = position;
        }

        public BoundingSphere getBoundingSphere()
        {
            return _boundingSphere;
        }

        public float getIncreaseBy()
        {
            return _increaseBy;
        }

        public float getTimeTillRunOut()
        {
            return _timeTillRunsOut;
        }

        public bool doesRunOut()
        {
            return _runsOut;
        }

        public bool hasBeenApplied()
        {
            return _hasBeenApplied;
        }

        public void setUsed()
        {
            _hasBeenApplied = true;
        }

        public Color getColor()
        {
            return _powerUpColour;
        }

        BoundingSphere _boundingSphere;
        protected float _increaseBy;
        protected float _timeTillRunsOut;
        protected bool _runsOut;
        protected bool _hasBeenApplied;
        protected Color _powerUpColour;
        private int _timeTillDespawn;
        private int _timeToAppear;
        private float _initialScale;
    }
}
