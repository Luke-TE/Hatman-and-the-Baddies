using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Effects
{
    class FlashAnimation
    {
        #region Fields

        int timer, flashingDuration, flashTime, nonFlashTime, elapsedFlashTime;
        bool flashing;

        #endregion


        #region Properties

        public bool Active { get; set; }
        public TextureAnimation CurrentAnimation { get; set; }
        public bool Flashing
        {
            get => flashing;

            set
            {
                flashing = value;
                if (value) CurrentAnimation.SetBlankSpriteSheet();
                else CurrentAnimation.SetDefaultSpriteSheet();
            }
        }

        #endregion


        #region Methods

        public FlashAnimation(TextureAnimation currentAnim, int flashingDuration, int flashTime = 100, int nonFlashTime = 100)
        {            
            CurrentAnimation = currentAnim;
            this.flashingDuration = flashingDuration;
            this.flashTime = flashTime;
            this.nonFlashTime = nonFlashTime;

            timer = 0;
            Active = false;
            Flashing = false;
        }


        public void UpdateFlash(GameTime gameTime)
        {
            if (!Active) return;

            if (timer > 0)
            {
                elapsedFlashTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                timer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (Flashing)
                {
                    if (elapsedFlashTime > flashTime)
                    {
                        elapsedFlashTime -= flashTime;
                        Flashing = false;
                    }

                }
                else
                {
                    if (elapsedFlashTime > nonFlashTime)
                    {
                        elapsedFlashTime -= nonFlashTime;
                        Flashing = true;
                    }

                }                
            }

            else
            {
                Flashing = false;
                Active = false; 
            }
                    
        }

        public void ActivateFlash()
        {
            timer = flashingDuration;
            Active = true;
            Flashing = true;
        }

        #endregion
    }
}
