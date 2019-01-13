using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlatformerProject.Core;
using PlatformerProject.Effects;
using PlatformerProject.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject
{
    class Player : IGameObject, ISprite, ICollidable
    {
        enum State { Idle, Running, Jumping, Attacking, Hurt, Dying }
        enum MoveDirection { None, Left, Right }
        enum AttackState { None = -1, Preparing = 0, Attacking = 1, Finished = 2 }

        #region Fields

        float gravity = 25f;
        GameObjectManager manager;
        TextureAnimation currentAnim;        
        State currentState;        
        MoveDirection direction;
        AttackState CurrentAttackState;        

        #endregion


        #region Properties

        bool TouchingFloor(GameTime gameTime) => this.CheckFloorColl(manager, gameTime);
        bool TouchingCeil(GameTime gameTime) => this.CheckCeilingColl(manager, gameTime);
        bool TouchingJt(GameTime gameTime) => this.CheckJumpThroughColl(manager, gameTime);
        bool TouchingWall(GameTime gameTime) => this.CheckWallColl(manager, gameTime);        

        public bool Active { get; set; }
        public bool Alive { get; set; }
        public int Health { get; set; }
        public int Damage { get; set; }
        public bool Invincible { get; set; }
        public float JumpForce { get; set; }
        public float MaxAcceleration { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Knockback { get; set; }
        public bool InputEnabled { get; set; }        
        public Dictionary<string,TextureAnimation> Animations { get; set; }
        public Dictionary<string, SoundEffect> Sounds { get; set; }
        public Dictionary<string, Keys> Inputs { get; set; }

        public bool Fixed => false;

        public Rectangle CollisionBox
        {
            get
            {
                var widthPercent = 0.3f;
                var width = widthPercent * Animations["Idle"].DestRect.Width;
                var x = (1 - widthPercent) / 2 * Animations["Idle"].DestRect.Width + Animations["Idle"].DestRect.X;

                var heightPercent = 0.5f;
                var height = heightPercent * Animations["Idle"].DestRect.Height;
                var y = (1 - heightPercent) / 2 * Animations["Idle"].DestRect.Height + Animations["Idle"].DestRect.Y;

                return new Rectangle((int)x, (int)y, (int)width, (int)height);

            }
        }
            

        public Rectangle AttackingHitbox
        {
            get
            {
                int y = CollisionBox.Top;
                int width = (int)(CollisionBox.Width * 1.5);
                int height = CollisionBox.Height;

                int x = currentAnim.FacingRight ? CollisionBox.Center.X : CollisionBox.Center.X - width;
                return new Rectangle(x, y, width, height);
            }
        }

        public Vector2 Position
        {
            get => currentAnim.Position;
            set
            {
                foreach (var anim in Animations)
                    anim.Value.Position = value;
            }
        }

        State CurrentState
        {
            get => currentState;

            set
            {
                currentState = value;
                if (Animations.ContainsKey(value.ToString()))
                {
                    if (!Animations[value.ToString()].Active)
                        Animations[value.ToString()].Reset();
                    currentAnim = Animations[value.ToString()];
                }

            }
        }

        MoveDirection Direction
        {
            get => direction;
            set
            {
                direction = value;

                if (value == MoveDirection.Right)
                    foreach (var anim in Animations)
                        anim.Value.FacingRight = true;

                else if (value == MoveDirection.Left)
                    foreach (var anim in Animations)
                        anim.Value.FacingRight = false;
            }
        }

        #endregion


        #region Core Methods

        public Player(GameObjectManager manager, Dictionary<string, TextureAnimation> animations)
        {
            Animations = animations;
            Sounds = new Dictionary<string, SoundEffect>();
            Inputs = new Dictionary<string, Keys>
            {
                {"left", Keys.Left},
                {"right", Keys.Right},
                {"jump", Keys.Space},
                {"attack", Keys.Z},
            };

            currentAnim = Animations["Idle"];
            this.manager = manager;

            CurrentState = State.Idle;
            Direction = MoveDirection.None;
            CurrentAttackState = AttackState.None;

            Active = true;
            Alive = true;
            Invincible = false;
            InputEnabled = true;

            Health = 20;
            Damage = 1;
            JumpForce = 600f;
            MaxAcceleration = 50f;
            Velocity = Vector2.Zero;
            Knockback = new Vector2(1000, -200);
            Position = currentAnim.Position;
        } 

        public void Update(GameTime gameTime)
        {            
            if (CollisionBox.Intersects(manager.Goal) && manager.CurrentGameState == GameObjectManager.GameState.Playing)
            {
                CurrentState = State.Jumping;
                manager.CurrentGameState = GameObjectManager.GameState.Won;                                                
                gravity = 0;
                Velocity = new Vector2(0, -700);
                InputEnabled = false;
            }

            if (InputEnabled && CurrentState != State.Attacking && manager.NewKeyState.IsKeyDown(Inputs["right"]))            
                Direction = MoveDirection.Right;
            
            else if (InputEnabled && CurrentState != State.Attacking && manager.NewKeyState.IsKeyDown(Inputs["left"]))
                Direction = MoveDirection.Left;

            else Direction = MoveDirection.None;


            switch (CurrentState)
            {                
                case State.Attacking:
                    var enumVals = Enum.GetValues(typeof(AttackState));
                    Array.Sort(enumVals);

                    foreach (var enumVal in enumVals)
                    {
                        if (currentAnim.CurrentFrame >= (int)enumVal)
                            CurrentAttackState = (AttackState)enumVal;
                    }                    

                    switch (CurrentAttackState)
                    {
                        case AttackState.Preparing:
                            break;

                        case AttackState.Attacking:
                            foreach (var gameObj in manager.GameObjects)
                                if (gameObj is Enemy enemy && AttackingHitbox.Intersects(enemy.CollisionBox) && !enemy.Invincible)
                                {
                                    var knockbackDir = currentAnim.FacingRight ? 1 : -1;
                                    var knockback = Knockback * new Vector2(knockbackDir, 1);

                                    enemy.GetHit(knockback, Damage);
                                }

                            break;

                        case AttackState.Finished:
                            if (!currentAnim.Active)                            
                                CurrentState = State.Idle;

                            break;

                        case AttackState.None:
                            throw new Exception();
                    }

                    break;

                case State.Jumping:
                    if (TouchingFloor(gameTime) || TouchingJt(gameTime) && Velocity.Y >= 0)
                        CurrentState = State.Idle;      
                    
                    break;
                
                case State.Idle:
                case State.Running:                                    
                    switch (Direction)
                    {
                        case MoveDirection.Right:
                            CurrentState = State.Running;
                            break;

                        case MoveDirection.Left:
                            CurrentState = State.Running;
                            break;
                            
                        case MoveDirection.None:
                            CurrentState = State.Idle;
                            break;
                    }

                    if (InputEnabled && manager.NewKeyState.IsKeyDown(Inputs["attack"]) && manager.OldKeyState.IsKeyUp(Inputs["attack"]))
                        Attack();

                    break;

                case State.Hurt:
                    if (!currentAnim.Active)
                    {
                        if (Health > 0)
                        {
                            InputEnabled = true;
                            Invincible = false;
                            CurrentState = State.Idle;
                        }
                            
                        else
                        {
                            CurrentState = State.Dying;
                            PlaySound("death", 0.03f);
                        }
                            
                    }
                    break;

                case State.Dying:
                    manager.CurrentGameState = GameObjectManager.GameState.Lost;
                    break;
            }

            UpdatePos(gameTime);            

            currentAnim.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            currentAnim.Draw(gameTime, spriteBatch);                                    
        }

        #endregion


        #region Action Methods

        public void UpdatePos(GameTime gameTime)
        {            
            SetXVel(gameTime);                        
            Position += new Vector2((float)Math.Round(Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds), 0);            

            SetYVel(gameTime);            
            Position += new Vector2(0, (float)Math.Round(Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds));                        
        }

        void SetXVel(GameTime gameTime)
        {            
            float accel = 0;
            float friction = -Velocity.X / 8;

            int directionSign = 0;
            if (Direction == MoveDirection.Right)
                directionSign = 1;
            else if (Direction == MoveDirection.Left)
                directionSign = -1;

            switch (CurrentState)
            {
                case State.Running:
                    accel = MaxAcceleration * directionSign;
                    break;
                case State.Jumping:
                    accel = MaxAcceleration * directionSign * 0.7f;
                    break;
            }                

            Velocity += new Vector2(accel + friction, 0);

            if (TouchingWall(gameTime)) Velocity = new Vector2(0, Velocity.Y);
        }

        void SetYVel(GameTime gameTime)
        {
            Velocity = new Vector2(Velocity.X, Velocity.Y + gravity);
           
            if (InputEnabled && CurrentState != State.Attacking && manager.NewKeyState.IsKeyDown(Inputs["jump"]) && (TouchingFloor(gameTime) || TouchingJt(gameTime) && Velocity.Y >= 0))
                Jump(gameTime);
            
            if (TouchingFloor(gameTime) || TouchingCeil(gameTime) || TouchingJt(gameTime) && Velocity.Y > 0)            
                Velocity = new Vector2(Velocity.X, 0);            
        }

        void Attack()
        {
            CurrentState = State.Attacking;
            CurrentAttackState = AttackState.Preparing;
            PlaySound("attack", 0.1f);
        }

        void Jump(GameTime gameTime)
        {
            CurrentState = State.Jumping;
            Velocity -= new Vector2(0, JumpForce);
            PlaySound("jump", 0.05f);            
        }

        public void GetHit(Vector2 knockback, int damage)
        {
            PlaySound("hit", 0.03f);
            InputEnabled = false;
            currentAnim.Reset();
            CurrentState = State.Hurt;
            CurrentAttackState = AttackState.None;
            Invincible = true;
            Velocity = knockback;
            
            Health -= damage;
            if (Health < 0) Health = 0;
        }

        #endregion


        #region Helper Methods

        void PlaySound(string soundName, float volume)
        {
            if (Sounds.ContainsKey(soundName))
            {
                var sound = Sounds[soundName].CreateInstance();
                sound.Volume = volume;
                sound.Play();
            }
        }

        #endregion
    }
}

