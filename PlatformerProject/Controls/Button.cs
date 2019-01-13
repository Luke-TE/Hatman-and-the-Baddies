using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlatformerProject.Core;

namespace Interface.Controls
{
    class Button : IGameObject
    {
        #region Fields
        MouseState currentMouse;
        MouseState previousMouse;
        SpriteFont font;
        Texture2D texture;
        bool isHovering;
        #endregion

        #region Properties
        public event EventHandler Click;
        public bool Fixed => true;

        public bool Active { get;  set; }
        public bool Clicked { get; private set; }
        public Color PenColour { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);        
        public string Text { get; set; }


        #endregion

        #region Methods
        //Constructor
        public Button(Texture2D texture, SpriteFont font)
        {
            Active = true;
            this.texture = texture;
            this.font = font;
            PenColour = Color.Black; //Default colour
        }
        
        //Draw Method
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = Color.White;
            if (isHovering) colour = Color.Gray; //Change colour if hovering

            //Draw button box
            spriteBatch.Draw(texture, Rectangle, colour);

            if (!string.IsNullOrEmpty(Text))
            {
                //Find top left corner of text rectangle
                var x = Rectangle.X + (Rectangle.Width / 2) - (font.MeasureString(Text).X / 2);
                var y = Rectangle.Y + (Rectangle.Height / 2) - (font.MeasureString(Text).Y / 2);

                //Draw button text
                spriteBatch.DrawString(font, Text, new Vector2(x, y), PenColour);
            }
        }

        //Update Method
        public void Update(GameTime gameTime)
        {
            //Update mouse states
            previousMouse = currentMouse;
            currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(currentMouse.X, currentMouse.Y, 1, 1);

            isHovering = false;
            
            //If mouse is over this button
            if (mouseRectangle.Intersects(Rectangle))
            {
                isHovering = true;

                //If mouse did a single left click
                if (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)                
                    Click?.Invoke(this, new EventArgs()); //Raise click event                
            }

        }
        #endregion
    }
}
