﻿#region Using Statements
using System;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF.ParticleSystems
{
    /// <summary>
    /// Create a new Particle System class that inherits from a Default DPSF Particle System.
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    class PowerUpParticleSystem : DefaultSprite3DBillboardParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PowerUpParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        private Color particleColor = Color.Pink;

        public float mfColorBlendAmount = 0.5f;
        public Vector3 mcExternalObjectPosition = Vector3.Zero;
        public float mfAttractRepelForce = 3.0f;
        public float mfAttractRepelRange = 50.0f;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 1000, 2500, "Textures/ParticleBoost", cSpriteBatch);
            LoadSmokeEvents();
            Emitter.ParticlesPerSecond = 50;
            Name = "Boost Particle";
        }

        public void LoadSmokeEvents()
        {
            ParticleInitializationFunction = InitializeParticleRisingBoost;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionAndVelocityUsingAcceleration, 500);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateColor);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
            ParticleEvents.AddEveryTimeEvent(IncreaseSizeBasedOnLifetime);
        }

        public void InitializeParticleRisingBoost(DefaultSprite3DBillboardParticle cParticle)
        {
            cParticle.Lifetime = RandomNumber.Between(1.0f, 4.0f);

            cParticle.Position = Emitter.PositionData.Position;
            cParticle.Position += new Vector3(0, 0, 0);
            cParticle.Size = RandomNumber.Next(5, 10);
            cParticle.Color = particleColor;
            cParticle.Rotation = RandomNumber.Between(0, MathHelper.TwoPi);

            cParticle.Velocity = new Vector3(RandomNumber.Next(-15, 15), RandomNumber.Next(-5, 30), RandomNumber.Next(-15, 15));
            cParticle.Acceleration = Vector3.Zero;
            cParticle.RotationalVelocity = RandomNumber.Between(-MathHelper.Pi, MathHelper.Pi);

            cParticle.StartSize = cParticle.Size;

            mfColorBlendAmount = 0f;
        }

        public void InitializeParticleFoggyBoost(DefaultSprite3DBillboardParticle cParticle)
        {
            cParticle.Lifetime = RandomNumber.Between(2.0f, 8.0f);

            cParticle.Position = Emitter.PositionData.Position;
            cParticle.Position += new Vector3(0, 0, 0);
            cParticle.Size = RandomNumber.Next(5, 12);
            cParticle.Color = particleColor;
            cParticle.Rotation = RandomNumber.Between(0, MathHelper.TwoPi);

            cParticle.Velocity = new Vector3(RandomNumber.Next(-30, 30), RandomNumber.Next(0, 10), RandomNumber.Next(-30, 30));
            cParticle.Acceleration = Vector3.Zero;
            cParticle.RotationalVelocity = RandomNumber.Between(-MathHelper.Pi, MathHelper.Pi);

            cParticle.StartSize = cParticle.Size;

            mfColorBlendAmount = 0.5f;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================
        protected void IncreaseSizeBasedOnLifetime(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Size = ((1.0f + cParticle.NormalizedElapsedTime) / 1.0f) * cParticle.StartSize;
        }

        protected void UpdateColor(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
        {
            cParticle.Color = particleColor;
        }

        protected void RepelParticleFromExternalObject(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Calculate Direction away from the Object and how far the Particle is from the Object
            Vector3 sDirectionAwayFromObject = cParticle.Position - mcExternalObjectPosition;
            float fDistance = sDirectionAwayFromObject.Length();

            // If the Particle is close enough to the Object to be affected by it
            if (fDistance < mfAttractRepelRange)
            {
                // Repel the Particle from the Object
                sDirectionAwayFromObject.Normalize();
                cParticle.Velocity += sDirectionAwayFromObject * (mfAttractRepelRange - fDistance) * mfAttractRepelForce;
                cParticle.RotationalVelocity += 0.005f;
            }
        }

        protected void AttractParticleToExternalObject(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
        {
            // Calculate Direction towards the Object and how far the Particle is from the Object
            Vector3 sDirectionTowardsObject = mcExternalObjectPosition - cParticle.Position;
            float fDistance = sDirectionTowardsObject.Length();

            // If the Particle is close enough to the Object to be affected by it
            if (fDistance < mfAttractRepelRange)
            {
                // Attract the Particle to the Object
                sDirectionTowardsObject.Normalize();
                cParticle.Velocity = sDirectionTowardsObject * (mfAttractRepelRange - fDistance) * mfAttractRepelForce;
            }
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        //===========================================================
        // Other Particle System Functions
        //===========================================================
        public void ChangeColor(Color particleColor)
        {
            this.particleColor = particleColor;
        }

        public void MakeParticlesAttractToExternalObject()
        {
            // Make sure we only apply the Attract function once by first removing the function if it already exists
            this.ParticleEvents.RemoveEveryTimeEvents(AttractParticleToExternalObject);
            this.ParticleEvents.AddEveryTimeEvent(AttractParticleToExternalObject);
        }

        public void MakeParticlesRepelFromExternalObject()
        {
            // Make sure we only apply the Repel function once by first removing the function if it already exists
            this.ParticleEvents.RemoveEveryTimeEvents(RepelParticleFromExternalObject);
            this.ParticleEvents.AddEveryTimeEvent(RepelParticleFromExternalObject);
        }

        public void StopParticleAttractionAndRepulsionToExternalObject()
        {
            this.ParticleEvents.RemoveEveryTimeEvents(RepelParticleFromExternalObject);
            this.ParticleEvents.RemoveEveryTimeEvents(AttractParticleToExternalObject);
        }
    }
}