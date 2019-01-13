using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Core
{
    interface IGameObject
    {
        #region Properties

        bool Fixed { get; }
        Vector2 Position { get; set; }
        bool Active { get; set; }        

        #endregion


        #region Methods

        void Update(GameTime gameTime);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        #endregion
    }
}
