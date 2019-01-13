using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatformerProject.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Enemies
{
    abstract class EnemyTurret : IGameObject, ICollidable
    {
        public abstract Rectangle CollisionBox { get; set; }
        public abstract bool Fixed { get; set; }
        public abstract Vector2 Position { get; set; }
        public abstract bool Active { get; set; }

        public bool CheckFloorCollision(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public bool CheckWallCollision(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void Update(GameTime gameTime);
    }
}
