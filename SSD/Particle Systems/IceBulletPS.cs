#region Using Statements
using System;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DPSF.ParticleSystems
{
    /// <summary>
    /// Create a new Particle System class that inherits from a
    /// Default DPSF Particle System
    /// </summary>
#if (WINDOWS)
    [Serializable]
#endif
    class IceBulletParticleSystem : DefaultTexturedQuadParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public IceBulletParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        public Color TrailStartColor = Color.Blue;
        public Color TrailEndColor = Color.Cyan;

        public int TrailStartSize = 20;
        public int TrailEndSize = 5;

        /// <summary>
        /// Adjust the scale to produce more or less particles.
        /// 1.0 = second particle will be touching (no overlapping) next particle.
        /// 0.5 = second particle will overlap half of first particle.
        /// 0.25 = second particle will overlap 3/4 of first particle.
        /// </summary>
        public float NumberOfParticlesToEmitScale = 1f;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        /// <summary>
        /// Initializes the render properties.
        /// </summary>
        protected override void InitializeRenderProperties()
        {
            base.InitializeRenderProperties();

            // Use additive blending
            RenderProperties.BlendState = BlendState.Additive;

                        //RenderProperties.RasterizerState.CullMode = CullMode.None;
        }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000,
                                                    UpdateVertexProperties, "Textures/Particle");

            LoadParticleSystem();
            //LoadSpinningTrailParticleSystem();
            Name = "Trail";
        }

        public void InitializeParticleTrail(DefaultTexturedQuadParticle cParticle)
        {
            cParticle.Lifetime = 1.0f;

            cParticle.Position = Emitter.PositionData.Position;
            cParticle.StartSize = cParticle.Size = TrailStartSize;
            cParticle.EndSize = TrailEndSize;
            cParticle.StartColor = cParticle.Color = TrailStartColor;
            cParticle.EndColor = TrailEndColor;

            cParticle.Velocity = Vector3.Zero;
            cParticle.Acceleration = Vector3.Zero;
            cParticle.Orientation = Emitter.OrientationData.Orientation;
            cParticle.RotationalVelocity = new Vector3(0, 0, (float)Math.PI);
        }

        public void LoadParticleSystem()
        {
            ParticleInitializationFunction = InitializeParticleTrail;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 100);

            ParticleSystemEvents.AddEveryTimeEvent(UpdateParticleSystemDynamicallyUpdateParticlesEmittedBasedOnSpeed);

            Emitter.PositionData.Position = new Vector3(0, 50, 0);
            Emitter.OrientationData.RotationalVelocity = new Vector3(0, 0, (float)Math.PI);
            Emitter.ParticlesPerSecond = 5000;

            TrailStartColor = Color.Blue;
            TrailEndColor = Color.Cyan;
            TrailStartSize = 20;
            TrailEndSize = 4;
            SetTexture("Textures/Particle");

            NumberOfParticlesToEmitScale = 0f;
        }

        public void LoadSpinningTrailParticleSystem()
        {
            ParticleInitializationFunction = InitializeParticleTrail;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 100);

            ParticleSystemEvents.AddEveryTimeEvent(UpdateParticleSystemDynamicallyUpdateParticlesEmittedBasedOnSpeed);

            Emitter.PositionData.Position = new Vector3(0, 0, 0);
            Emitter.OrientationData.RotationalVelocity = new Vector3(0, 0, (float)Math.PI);
            Emitter.ParticlesPerSecond = 100;

            TrailStartColor = Color.Blue;
            TrailEndColor = Color.Cyan;
            TrailStartSize = 30;
            TrailEndSize = 5;
            //            SetTexture("Textures/Cloud");
            SetTexture("Textures/MoveArrow");
            NumberOfParticlesToEmitScale = 0.25f;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        // Persistent variables used by the UpdateParticleSystemDynamicallyUpdateParticlesEmittedBasedOnSpeed() function.
        private float _timeSinceLastUpdate = 0;
        private const float _waitTimeBetweenUpdates = 0.05f;
        private const float _numberOfUpdatesPerSecond = (1.0f / _waitTimeBetweenUpdates);
        private Vector3 _emittersLastPosition = new Vector3();

        // Temp variables used by UpdateParticleSystemDynamicallyUpdateParticlesEmittedBasedOnSpeed() to avoid creating garbage for the collector.
        private float _distanceTravelled = 0;
        private float _distancePerSecond = 0;

        /// <summary>
        /// Dynamically update the number of particles to emit based on how fast the emitter is moving, so if it
        /// is moving fast, more particles will be released, and fewer will be released if it is moving slowly.
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        protected void UpdateParticleSystemDynamicallyUpdateParticlesEmittedBasedOnSpeed(float fElapsedTimeInSeconds)
        {
            // If not enough time has elapsed to perform an update yet, then just exit.
            _timeSinceLastUpdate += fElapsedTimeInSeconds;
            if (_timeSinceLastUpdate < _waitTimeBetweenUpdates)
                return;

            // We're doing an update now, so update the amount of time since the last update.
            _timeSinceLastUpdate -= _waitTimeBetweenUpdates;

            // Calculate how far the emitter has travelled since the last update.
            _distanceTravelled = (this.Emitter.PositionData.Position - _emittersLastPosition).Length();

            // Calculate how many particles to emit based on the distance the emitter has travelled, and the given Scale.
            _distancePerSecond = _distanceTravelled * _numberOfUpdatesPerSecond;
            this.Emitter.ParticlesPerSecond = _distancePerSecond / (TrailStartSize * NumberOfParticlesToEmitScale);

            // Record the emitter's position for the next update.
            _emittersLastPosition = this.Emitter.PositionData.Position;
        }

        //===========================================================
        // Other Particle System Functions
        //===========================================================
    }
}