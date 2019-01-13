using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Effects.ParticleEngines
{
    class GrowingParticleEngine : ParticleEngine
    {
        #region Properties

        public float Growth { get; set; }

        #endregion


        #region Methods

        public GrowingParticleEngine(Texture2D texture, Vector2 position, float minVelocity, float maxVelocity, Vector2 direction, double lifetime, float delay, float growth = 0.01f)
        : base(texture, position, minVelocity, maxVelocity, direction, lifetime, delay)
        {
            Growth = growth;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Particle particle in particles)
                particle.Scale *= 1f + Growth / 100;

            base.Update(gameTime);
        }

        #endregion
    }
}
