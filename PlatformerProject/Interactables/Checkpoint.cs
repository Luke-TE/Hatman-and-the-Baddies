using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatformerProject.Core;

namespace PlatformerProject.Interactables
{
    class Checkpoint : IGameObject, IInteractable
    {
        #region Properties

        public Rectangle InteractionArea { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Fixed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Active { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion


        #region Methods

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public void InteractedWith(ISprite sprite)
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
