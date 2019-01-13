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
    class TextureAnimation : IGameObject, ICloneable
    {
        #region Fields

        Texture2D spriteSheet;

        int rows, columns, elapsedFrameTime, frameTime;                
        Color[] defaultTextureData;
        Color[] blankTextureData;

        #endregion


        #region Properties

        int FrameCount => rows * columns;
        int FrameWidth => spriteSheet.Width / columns;
        int FrameHeight => spriteSheet.Height / rows;
        int CurrentRow => CurrentFrame / columns;
        int CurrentCol => CurrentFrame % columns;

        public bool Fixed { get; set; }
        public bool Fading { get; set; }
        public float FadePercent { get; set; }
        public Vector2 Position { get; set; }
        public Color Colour { get; set; }
        public bool Active { get; set; }
        public bool Looping { get; set; }
        public float Scale { get; set; }
        public bool FacingRight { get; set; } = true;
        public int CurrentFrame { get; private set; }

        public int Width => (int)(Scale * FrameWidth);
        public int Height => (int)(Scale * FrameHeight);
        public Rectangle SourceRect => new Rectangle(CurrentCol * FrameWidth, CurrentRow * FrameHeight, FrameWidth, FrameHeight);
        public Rectangle DestRect => new Rectangle((int)Position.X - Width / 2, (int)Position.Y - Height / 2, Width, Height);
        public object Clone() => new TextureAnimation(spriteSheet, Position, rows, columns, frameTime, Looping, Scale);
        

        #endregion


        #region Methods

        public TextureAnimation(Texture2D spriteSheet, Vector2 position, int rows, int columns, int frameTime, bool looping, float scale = 1f, float fadePercent = 0.01f)
        {
            this.spriteSheet = spriteSheet;
            Position = position;
            this.rows = rows;
            this.columns = columns;
            this.frameTime = frameTime;
            Looping = looping;
            FadePercent = fadePercent;

            Fading = false;
            Active = true;
            Fixed = false;
            Scale = scale;
            CurrentFrame = 0;
            elapsedFrameTime = 0;
            Colour = Color.White;            

            defaultTextureData = new Color[this.spriteSheet.Width * this.spriteSheet.Height];
            this.spriteSheet.GetData(defaultTextureData);

            blankTextureData = new Color[this.spriteSheet.Width * this.spriteSheet.Height];
            this.spriteSheet.GetData(blankTextureData);

            for (int i = 0; i < blankTextureData.Length - 1; i++)
            {
                if (blankTextureData[i].A > 0)
                    blankTextureData[i] = Color.White;
            }
            
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
                return; 

            elapsedFrameTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;            

            if (elapsedFrameTime > frameTime)
            {
                CurrentFrame++;
                
                if (CurrentFrame == FrameCount)
                {
                    CurrentFrame = 0;
                    if (!Looping)
                    {
                        CurrentFrame = FrameCount - 1;
                        Active = false;
                    }                        
                }

                elapsedFrameTime -= frameTime;
            }
                            
            
            if (Fading)
            {
                if ((float)Colour.A / 255 > 0)
                    Colour = new Color(
                        (float)Colour.R / 255 - FadePercent, 
                        (float)Colour.G / 255 - FadePercent, 
                        (float)Colour.B / 255 - FadePercent, 
                        (float)Colour.A / 255 - FadePercent);
                else
                {
                    Active = false;
                    Colour = new Color(0);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var spritefx = SpriteEffects.None;
            if (!FacingRight) spritefx = SpriteEffects.FlipHorizontally;
            
            spriteBatch.Draw(spriteSheet, DestRect, SourceRect, Colour, 0, Vector2.Zero, spritefx, 1);            
        }

        public void Reset()
        {
            CurrentFrame = 0;
            Active = true;
        }

        public void SetBlankSpriteSheet()
        {
            spriteSheet.SetData(blankTextureData);
        }

        public void SetDefaultSpriteSheet()
        {
            spriteSheet.SetData(defaultTextureData);
        }
        
        #endregion
    }
}
