using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class EnemyIceBoss : EnemyEntity
    {
        public EnemyIceBoss(Vector3 position, ModelContainer model, ModelContainer modelForBit, Entity planetSphere, PlayerEntity player, Random ran, float scale = 1, float yaw = 0, float pitch = 0, float roll = 0)
            : base(position, model, false, scale, yaw, pitch, roll)
        {
            _planetTarget = planetSphere;
            _player = player;
            _forwardSpeed = (((float)ran.NextDouble() - 0.5f) * ran.Next(4, 10)) * 0.001f;
            _sideSpeed = (((float)ran.NextDouble() - 0.5f) * ran.Next(4, 10)) * 0.001f;
            _speed = 1.0f;
            _myScore = 10000;
            _initialHealth = 6000;
            _initialTailSize = 200;
            base.setHealth(_initialHealth);

            _myRandomer = ran;
            _timeTillNextRandomDir = 250;

            _currentPosition = getMatrix().Translation;
            _previousPosition = _currentPosition;

            _myBits = new List<EnemyIceBit>();
            _myBitsModel = modelForBit;

            Matrix currentOrientationToPlanet = getOrientationToPlanet();

            for (int numBits = 0; numBits < _initialTailSize; ++numBits)
            {
                if (numBits == 0)
                {
                    Vector3 backwards = currentOrientationToPlanet.Backward;
                    backwards.Normalize();
                    Vector3 spawnPoint = getMatrix().Translation + backwards;

                    BoundingSphere targetBitSphere = new BoundingSphere();
                    targetBitSphere.Center = spawnPoint;

                    foreach (ModelMesh mesh in _myBitsModel.getModel().Meshes)
                    {
                        targetBitSphere.Radius = mesh.BoundingSphere.Radius;
                    }

                    //Keep moving till intersecting with the head
                    while (targetBitSphere.Intersects(getBoundingSphere()))
                    {
                        targetBitSphere.Center += backwards;
                    }

                    _myBits.Add(new EnemyIceBit(targetBitSphere.Center, targetBitSphere.Radius, _myBitsModel));
                }
                else
                {
                    BoundingSphere startingSphere = _myBits[_myBits.Count - 1].getBoundingSphere();
                    Vector3 backwards = currentOrientationToPlanet.Backward;
                    backwards.Normalize();
                    //Keep moving till intersecting with the last bit
                    while (startingSphere.Intersects(_myBits[_myBits.Count - 1].getBoundingSphere()))
                    {
                        startingSphere.Center += backwards;
                    }

                    _myBits.Add(new EnemyIceBit(startingSphere.Center, startingSphere.Radius, _myBitsModel));
                }
            }


        }

        public Matrix getOrientationToPlanet()
        {
            Matrix rotationMat = Matrix.Identity;
            Matrix currentPosition = getMatrix();
            rotationMat.Down = currentPosition.Translation - _planetTarget.getMatrix().Translation;
            rotationMat.Down.Normalize();
            //rotationMat.Up = -rotationMat.Down;
            rotationMat.Right = Vector3.Cross(currentPosition.Forward, rotationMat.Up);
            rotationMat.Right.Normalize();
            //rotationMat.Left = -rotationMat.Right;
            rotationMat.Forward = Vector3.Cross(rotationMat.Left, rotationMat.Up);
            rotationMat.Backward = -rotationMat.Forward;

            return rotationMat;
        }

        override public Matrix getMatrix()
        {
            Matrix transform = Matrix.Identity;

            Quaternion accumulateRotation = base.getRotation();

            transform *= Matrix.CreateScale(base.getScale());
            transform *= Matrix.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(this.getYaw(), this.getPitch(), this.getRoll()));
            transform *= Matrix.CreateTranslation(base.getPosition());
            transform *= Matrix.CreateFromQuaternion(this.getRawRotation());

            return transform;
        }

        override public void update(TimeSpan deltaTime)
        {
            float dt = deltaTime.Milliseconds / 10.0f;

            if (_isSpawning)
            {
                this.setPosition(this.getPosition() + (_planetTarget.getMatrix().Translation - this.getMatrix().Translation) * 0.001f * dt);
                this.addRoll(_forwardSpeed * 10 * -1 * deltaTime.Milliseconds);
                this.addPitch(_sideSpeed * 10 * -1 * deltaTime.Milliseconds);
            }
            else
            {
                this.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Forward, _forwardSpeed * _speed * dt));
                this.addRotation(Quaternion.CreateFromAxisAngle(Vector3.Left, _sideSpeed * _speed * dt));

                this.addRoll(_forwardSpeed * 1000 * dt);
                this.addPitch(_sideSpeed * 1000 * dt);
            }

            _previousPosition = _currentPosition;
            _currentPosition = getMatrix().Translation;

            for (int curBit = 0; curBit < _myBits.Count; ++curBit)
            {
                if (curBit == 0)
                {
                    Vector3 newPosition = _currentPosition;
                    _myBits[curBit].updatePosition(newPosition);
                }
                else
                {
                    //BoundingSphere startingSphere = _myBits[curBit - 1].getBoundingSphere();
                    //Vector3 moveDirection = _myBits[curBit - 1].getLastPosition() - _myBits[curBit - 1].getPosition();
                    //moveDirection.Normalize();
                    ////Keep moving till intersecting with the last bit
                    //while (startingSphere.Intersects(_myBits[curBit - 1].getBoundingSphere()))
                    //{
                    //    startingSphere.Center += moveDirection;
                    //}

                    //_myBits[curBit].updatePosition(startingSphere.Center);

                    _myBits[curBit].updatePosition(_myBits[curBit - 1].getLastPosition());

                }
            }

            _timeTillNextRandomDir -= deltaTime.Milliseconds;

            if (_timeTillNextRandomDir <= 0)
            {
                _forwardSpeed = (((float)_myRandomer.NextDouble() + 0.2f) * 0.0075f);
                _sideSpeed = (((float)_myRandomer.NextDouble() + 0.2f) * 0.0075f);

                if (_myRandomer.Next(0, 2) == 1)
                {
                    _forwardSpeed = -_forwardSpeed;
                }
                if (_myRandomer.Next(0, 2) == 1)
                {
                    _sideSpeed = -_sideSpeed;
                }

                _timeTillNextRandomDir = _myRandomer.Next(250, 4000);
            }


            int amountOfTailBits = (int)(((float)base.getHealth() / (float)_initialHealth) * (float)_initialTailSize);
            if (_myBits.Count > amountOfTailBits)
            {
                if (base.getHealth() <= 0)
                {
                    _myBits.Clear();
                }
                else
                {
                    int amountToRemove = _myBits.Count - amountOfTailBits;
                    _myBits.RemoveRange(_myBits.Count - amountToRemove - 1, amountToRemove);
                }
            }

            base.update(deltaTime);
        }

        public override void draw(Matrix view, Matrix proj, Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice)
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

                        effect.SpecularColor = Color.Red.ToVector3();
                        effect.SpecularPower = 200f;

                        effect.LightingEnabled = true; // turn on the lighting subsystem.
                        effect.DirectionalLight0.Enabled = true;
                        effect.DirectionalLight1.Enabled = true;
                        //effect.DirectionalLight2.Enabled = true;

                        effect.DirectionalLight0.Direction = new Vector3(1, 1, 1);
                        effect.DirectionalLight0.SpecularColor = Color.White.ToVector3();
                        effect.DirectionalLight1.Direction = new Vector3(-1, -1, -1);
                        effect.DirectionalLight1.SpecularColor = Color.White.ToVector3();
                    }
                    //BoundingSphereRenderer.Render(getBoundingSphere(), graphicsDevice, view, proj, Color.Red);
                    mesh.Draw();
                }
            }

            _myBits.ForEach(delegate(EnemyIceBit bit)
            {
                foreach (ModelMesh mesh in _myBitsModel.getModel().Meshes)
                {
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            Matrix snakeBitMat = Matrix.Identity;
                            snakeBitMat *= Matrix.CreateTranslation(bit.getPosition());

                            effect.World = getModelContainer().getBoneTransform(mesh.ParentBone.Index) * snakeBitMat;
                            effect.View = view;
                            effect.Projection = proj;

                            effect.SpecularColor = Color.Red.ToVector3();
                            effect.SpecularPower = 200f;

                            effect.LightingEnabled = true; // turn on the lighting subsystem.
                            effect.DirectionalLight0.Enabled = true;
                            effect.DirectionalLight1.Enabled = true;
                            //effect.DirectionalLight2.Enabled = true;

                            effect.DirectionalLight0.Direction = new Vector3(1, 1, 1);
                            effect.DirectionalLight0.SpecularColor = Color.White.ToVector3();
                            effect.DirectionalLight1.Direction = new Vector3(-1, -1, -1);
                            effect.DirectionalLight1.SpecularColor = Color.White.ToVector3();
                        }
                        //BoundingSphereRenderer.Render(bit.getBoundingSphere(), graphicsDevice, view, proj, Color.Red);
                        mesh.Draw();
                    }
                }
            });

        }

        public List<EnemyIceBit> getMyBits()
        {
            return _myBits;
        }

        public int getInitialHealth()
        {
            return _initialHealth;
        }

        Entity _planetTarget;
        PlayerEntity _player;
        float _forwardSpeed;
        float _sideSpeed;
        float _speed;

        int _initialHealth;
        int _initialTailSize;

        Random _myRandomer;
        float _timeTillNextRandomDir;

        Vector3 _currentPosition;
        Vector3 _previousPosition;

        List<EnemyIceBit> _myBits;
        ModelContainer _myBitsModel;
        BoundingSphere _bitsBoundingSphere;

    }
}
