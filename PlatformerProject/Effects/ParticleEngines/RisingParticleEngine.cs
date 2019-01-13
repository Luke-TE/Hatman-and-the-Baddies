using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Effects.ParticleEngines
{
    class RisingParticleEngine : ParticleEngine
    {
        #region Properties

        public override bool Fixed => false;
        public float RiseRate { get; set; }

        #endregion


        #region Methods

        public RisingParticleEngine(Texture2D texture, Vector2 position, float minVelocity, float maxVelocity, Vector2 direction, double lifetime, float delay, float curvature = 1.004f)
            : base(texture, position, minVelocity, maxVelocity, direction, lifetime, delay)
        {
            RiseRate = curvature;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Particle particle in particles)
            {
                particle.Colour = new Color(particle.Colour, 0.75f);
                particle.Velocity = new Vector2(particle.Velocity.X, RiseRate * particle.Velocity.Y);
                particle.Scale *= 0.99995f;
            }
            base.Update(gameTime);
        }

        #endregion
    }
}
