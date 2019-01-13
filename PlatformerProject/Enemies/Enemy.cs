using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PlatformerProject.Core;
using PlatformerProject.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Enemies
{
    abstract class Enemy : IGameObject, ISprite, ICollidable
    {
        #region Fields
                
        protected float gravity;
        protected GameObjectManager manager;              
        protected TextureAnimation currentAnim;

        #endregion


        #region Properties

        protected bool TouchingFloor(GameTime gameTime) => this.CheckFloorColl(manager, gameTime);
        protected bool TouchingCeil(GameTime gameTime) => this.CheckCeilingColl(manager, gameTime);
        protected bool TouchingJt(GameTime gameTime) => this.CheckJumpThroughColl(manager, gameTime);
        protected bool TouchingWall(GameTime gameTime) => this.CheckWallColl(manager, gameTime);
        
        public bool Active { get; set; }
        public Vector2 Velocity { get; set; }        
        public int Health { get; set; }
        public bool Invincible { get; set; }
        public int Damage { get; set; }
        public float MaxAcceleration { get; set; }
        public Vector2 Knockback { get; set; }
        public Dictionary<string, TextureAnimation> Animations { get; set; }
        public Dictionary<string, SoundEffect> Sounds { get; set; }

        public virtual bool Fixed { get; }
        public virtual Rectangle CollisionBox => currentAnim.DestRect;
        public virtual Vector2 Position
        {
            get => currentAnim.Position;
            set
            {
                foreach (var anim in Animations)
                    anim.Value.Position = value;
            }
        }

        #endregion


        #region Methods
        
        public abstract void Update(GameTime gameTime);

        public abstract void UpdatePos(GameTime gameTime);

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            currentAnim.Draw(gameTime, spriteBatch);
        }

        public abstract void GetHit(Vector2 knockback, int damage);

        #endregion


    }
}
