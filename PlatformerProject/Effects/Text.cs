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
    class Text : IGameObject
    {        
        public bool Fixed { get; set; }
        public Vector2 Position { get; set; }
        public int LifeTime { get; set;  }
        public bool Active { get; set; }
        public string Content { get; set;  }
        public SpriteFont Font { get; }
        public bool FadingIn { get; set;  }        
        public bool FadingOut { get; set;  }
        public float FadePercent { get; set; }
        public Color PenColour { get; set; }

        public Text(string content, SpriteFont font, Vector2 position, int lifeTime = 1000, bool fadingOut = false, bool fadingIn = false, float fadePercent = 0.01f)
        {
            Active = true;
            Content = content;
            Font = font;
            Position = position;
            LifeTime = lifeTime;
            FadingIn = fadingIn;
            FadingOut = fadingOut;            
            FadePercent = fadePercent;

            PenColour = Color.Black;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Active) return;

            if (LifeTime > 0)
                LifeTime -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            else if (FadingOut)
            {                                
                if ((float)PenColour.A / 255 > 0)
                    PenColour = new Color(
                        (float)PenColour.R / 255 - FadePercent,
                        (float)PenColour.G / 255 - FadePercent,
                        (float)PenColour.B / 255 - FadePercent,
                        (float)PenColour.A / 255 - FadePercent);
                else
                {
                    Active = false;
                    PenColour = new Color(0);
                }                
            }                        
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {                        
            if (!string.IsNullOrEmpty(Content))
            {
                var x = Position.X - (Font.MeasureString(Content).X / 2);
                var y = Position.Y - (Font.MeasureString(Content).Y / 2);

                spriteBatch.DrawString(Font, Content, new Vector2(x, y), PenColour);
            }
        }
    }
}
