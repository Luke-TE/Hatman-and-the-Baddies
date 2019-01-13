using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Core
{
    interface ISprite : IGameObject
    {        
        Vector2 Velocity { get; set; }
        void UpdatePos(GameTime gameTime);
    }
}
