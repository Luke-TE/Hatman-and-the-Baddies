using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Core
{
    interface ICollidable : IGameObject
    {
        Rectangle CollisionBox { get; }
        
    }
}
