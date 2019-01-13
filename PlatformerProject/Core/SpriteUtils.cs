using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Core
{
    static class SpriteUtils
    {
        #region Collidable Collisions

        public static bool IsTouchingLeft<T>(this T sprite, ICollidable collidable, GameTime gameTime) where T : ISprite, ICollidable
        {
            return sprite.CollisionBox.Right + (float)Math.Truncate(sprite.Velocity.X * gameTime.ElapsedGameTime.TotalSeconds) > collidable.CollisionBox.Left &&
                sprite.CollisionBox.Left + (float)Math.Truncate(sprite.Velocity.X * gameTime.ElapsedGameTime.TotalSeconds) < collidable.CollisionBox.Left &&
                sprite.CollisionBox.Bottom > collidable.CollisionBox.Top &&
                sprite.CollisionBox.Top < collidable.CollisionBox.Bottom;
        }

        public static bool IsTouchingRight<T>(this T sprite, ICollidable collidable, GameTime gameTime) where T : ISprite, ICollidable
        {
            return sprite.CollisionBox.Left + (float)Math.Truncate(sprite.Velocity.X * gameTime.ElapsedGameTime.TotalSeconds) < collidable.CollisionBox.Right &&
                sprite.CollisionBox.Right + (float)Math.Truncate(sprite.Velocity.X * gameTime.ElapsedGameTime.TotalSeconds) > collidable.CollisionBox.Right &&
                sprite.CollisionBox.Bottom > collidable.CollisionBox.Top &&
                sprite.CollisionBox.Top < collidable.CollisionBox.Bottom;
        }


        public static bool IsTouchingBottom<T>(this T sprite, ICollidable collidable, GameTime gameTime) where T : ISprite, ICollidable
        {
            return sprite.CollisionBox.Top + sprite.Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds < collidable.CollisionBox.Bottom &&
                sprite.CollisionBox.Bottom + sprite.Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds > collidable.CollisionBox.Bottom &&
                sprite.CollisionBox.Right > collidable.CollisionBox.Left &&
                sprite.CollisionBox.Left < collidable.CollisionBox.Right;
        }

        public static bool IsTouchingTop<T>(this T sprite, ICollidable collidable, GameTime gameTime) where T : ISprite, ICollidable
        {
            return sprite.CollisionBox.Bottom + sprite.Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds > collidable.CollisionBox.Top &&
                sprite.CollisionBox.Top + sprite.Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds < collidable.CollisionBox.Top &&
                sprite.CollisionBox.Right > collidable.CollisionBox.Left &&
                sprite.CollisionBox.Left < collidable.CollisionBox.Right;
        }

        #endregion


        #region Rectangle Collisions

        public static bool IsTouchingLeft<T>(this T sprite, Rectangle rect, GameTime gameTime) where T : ISprite, ICollidable
        {            
            return sprite.CollisionBox.Right + sprite.Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds > rect.Left &&
                sprite.CollisionBox.Left + sprite.Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds < rect.Left &&
                sprite.CollisionBox.Bottom > rect.Top &&
                sprite.CollisionBox.Top < rect.Bottom;
        }
        
        public static bool IsTouchingRight<T>(this T sprite, Rectangle rect, GameTime gameTime) where T : ISprite, ICollidable
        {
            return sprite.CollisionBox.Left + sprite.Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds < rect.Right &&
                sprite.CollisionBox.Right + sprite.Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds > rect.Right &&
                sprite.CollisionBox.Bottom > rect.Top &&
                sprite.CollisionBox.Top < rect.Bottom;
        }


        public static bool IsTouchingBottom<T>(this T sprite, Rectangle rect, GameTime gameTime) where T : ISprite, ICollidable
        {
            return sprite.CollisionBox.Top + sprite.Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds < rect.Bottom &&
                sprite.CollisionBox.Bottom + sprite.Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds > rect.Bottom &&
                sprite.CollisionBox.Right > rect.Left &&
                sprite.CollisionBox.Left < rect.Right;
        }

        public static bool IsTouchingTop<T>(this T sprite, Rectangle rect, GameTime gameTime) where T : ISprite, ICollidable
        {
            return sprite.CollisionBox.Bottom + sprite.Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds > rect.Top &&
                sprite.CollisionBox.Top + sprite.Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds < rect.Top &&
                sprite.CollisionBox.Right > rect.Left &&
                sprite.CollisionBox.Left < rect.Right;
        }

        #endregion


        #region Environment Collisions

        public static bool CheckWallColl<T>(this T sprite, GameObjectManager manager, GameTime gameTime) where T : ISprite, ICollidable
        {
            foreach (var wallRect in manager.TileColls)
                if (sprite.IsTouchingLeft(wallRect, gameTime) || sprite.IsTouchingRight(wallRect, gameTime))
                {
                    Console.WriteLine("");
                    return true;
                }                    
            return false;
        }

        public static bool CheckFloorColl<T>(this T sprite, GameObjectManager manager, GameTime gameTime) where T : ISprite, ICollidable
        {
            foreach (var floorRect in manager.TileColls)
                if (sprite.IsTouchingTop(floorRect, gameTime))
                    return true;
            return false;
        }

        public static bool CheckCeilingColl<T>(this T sprite, GameObjectManager manager, GameTime gameTime) where T : ISprite, ICollidable
        {
            foreach (var ceilRect in manager.TileColls)
                if (sprite.IsTouchingBottom(ceilRect, gameTime))
                    return true;
            return false;
        }

        public static bool CheckJumpThroughColl<T>(this T sprite, GameObjectManager manager, GameTime gameTime) where T : ISprite, ICollidable
        {
            foreach (var jtRect in manager.JumpThroughColls)
            {                
                if (sprite.CollisionBox.Bottom + sprite.Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds > jtRect.Top &&
                    //sprite.CollisionBox.Bottom < jtRect.Top &&
                sprite.CollisionBox.Top + sprite.Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds < jtRect.Top &&
                sprite.CollisionBox.Right > jtRect.Left &&
                sprite.CollisionBox.Left < jtRect.Right)                                    
                    return true;
            }
                
            return false;
        }

        #endregion
    }
}
