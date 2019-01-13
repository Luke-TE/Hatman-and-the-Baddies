using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatformerProject.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Effects.ParticleEngines
{
    class ParticleEngine : IGameObject
    {
        #region Fields

        static Random random;
        protected List<Particle> particles;        
        TimeSpan elapsedTime;

        #endregion


        #region Properties

        public virtual bool Fixed  => false;

        public Vector2 Position { get; set; }
        public bool Active { get; set; }        
        public Texture2D ParticleTexture { get; set; }
        public float MinParticleVelocity { get; set; }
        public float MaxParticleVelocity { get; set; }
        public int EmitterWidth { get; set; }
        public int EmitterHeight { get; set; }
        public float ParticleAngularVelocity { get; set; }
        public TimeSpan ParticleLifetime { get; set; }
        public Vector2 Direction { get; set; }
        public Color ParticleColour { get; set; }
        public TimeSpan SpawnDelay { get; set; }                

        public Rectangle EmitterArea => new Rectangle((int)Position.X, (int)Position.Y, EmitterWidth, EmitterHeight);

        #endregion


        #region Methods

        public ParticleEngine(Texture2D texture, Vector2 position, float minVelocity, float maxVelocity, Vector2 direction, double lifetime, float delay)
        {
            particles = new List<Particle>();
            random = new Random();
            elapsedTime = TimeSpan.Zero;
            Active = true;
            EmitterWidth = 1;
            EmitterHeight = 1;
            ParticleAngularVelocity = 0;
            ParticleColour = Color.White;

            ParticleTexture = texture;
            Position = position;
            MinParticleVelocity = minVelocity;
            MaxParticleVelocity = maxVelocity;                        
            ParticleLifetime = TimeSpan.FromSeconds(lifetime);
            ParticleColour = Color.White;
            SpawnDelay = TimeSpan.FromSeconds(delay);            

            Direction = direction;
            direction = Direction;
            direction.Normalize();
            Direction = direction;
        }        

        public virtual Particle GenerateNewParticle()
        {
            Vector2 velocity = new Vector2(
                ((float)random.NextDouble() * (MaxParticleVelocity - MinParticleVelocity) + MinParticleVelocity) * Direction.X,
                ((float)random.NextDouble() * (MaxParticleVelocity - MinParticleVelocity) + MinParticleVelocity) * Direction.Y
                );
            Vector2 position = new Vector2(
                (float)random.NextDouble() * EmitterWidth + Position.X,
                (float)random.NextDouble() * EmitterHeight + Position.Y
                );            

            return new Particle(ParticleTexture, position, velocity, ParticleAngularVelocity, ParticleLifetime, ParticleColour);
        }

        public virtual void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > SpawnDelay)
            {
                elapsedTime -= SpawnDelay;
                particles.Add(GenerateNewParticle());
            }

            for (var i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update(gameTime);

                if ((!particles[i].Active))
                    particles.RemoveAt(i);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var particle in particles)
                particle.Draw(gameTime, spriteBatch);
        }

        #endregion
    }
}
