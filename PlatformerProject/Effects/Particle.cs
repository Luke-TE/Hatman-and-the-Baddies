using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatformerProject.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Effects
{
    class Particle : IGameObject, ISprite
    {
        #region Fields

        float angle;
        TimeSpan remainingTime;

        #endregion


        #region Properties

        public bool Fixed => false;
        public Texture2D Texture { get; set; }
        public Vector2 Velocity { get; set; }
        public float AngularVelocity { get; set; }
        public Color Colour { get; set; }
        public float Scale { get; set; }
        public bool AffectedByGravity { get; set; }        
        public Vector2 Position { get; set; }
        public bool Active { get; set; }

        #endregion


        #region Methods

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity, float angularVelocity, TimeSpan lifetime, Color colour, float scale = 1f)
        {
            angle = 0;
            remainingTime = lifetime;
            AffectedByGravity = false;
            Active = true; 

            Position = position;
            Texture = texture;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
            Colour = colour;
            Scale = scale;
        }

        public void Update(GameTime gameTime)
        {            
            remainingTime -= gameTime.ElapsedGameTime;
            if (remainingTime.TotalSeconds <= 0) Active = false;
            UpdatePos(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            spriteBatch.Draw(Texture, Position, null, Colour, angle, origin, Scale, SpriteEffects.None, 0);
        }

        public void UpdatePos(GameTime gameTime)
        {
            float timeElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Vector2.Multiply(Velocity, timeElapsed);
            angle += AngularVelocity * timeElapsed;
        }        

        #endregion
    }
}
