using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PlatformerProject.Core;
using PlatformerProject.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformerProject.Enemies
{
    class Blob_Man : Enemy
    {        
        enum State { Idle, Walking, Attacking, Dying }
        enum MoveDirection { None, Left, Right }

        #region Fields        

        Vector2 position;
        int range;
        State currentState;
        MoveDirection direction;
        int invinTimer;
        int soundTimer;
        
        #endregion


        #region Properties        

        public int InvincibilityTime { get; set; }
        public int MoveSoundDelay { get; set; }
        public override Rectangle CollisionBox => Animations["Idle"].DestRect;

        public override Vector2 Position
        {
            get => position;
            set
            {
                position = value;

                foreach (var anim in Animations)
                {
                    if (anim.Key == "Attacking")
                    {
                        if (Direction == MoveDirection.Right)
                            anim.Value.Position = new Vector2(position.X + 15, position.Y + 5);
                        else if (Direction == MoveDirection.Left)
                            anim.Value.Position = new Vector2(position.X - 15, position.Y + 5);
                    }

                    else if (anim.Key == "Walking")
                    {
                        if (Direction == MoveDirection.Right)
                            anim.Value.Position = new Vector2(position.X, position.Y + 6);
                        else if (Direction == MoveDirection.Left)
                            anim.Value.Position = new Vector2(position.X, position.Y + 6);
                    }                    

                    else
                        anim.Value.Position = position;

                }

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

        public Rectangle AttackingHitbox =>
            new Rectangle(currentAnim.FacingRight ? CollisionBox.Right : CollisionBox.X - CollisionBox.Width / 2,
                CollisionBox.Top,
                CollisionBox.Width / 2,
                CollisionBox.Height);

        #endregion


        #region Methods

        public Blob_Man(GameObjectManager manager, Dictionary<string, TextureAnimation> animations, Vector2 pos, bool facingRight)
        {
            this.manager = manager;
            Animations = animations;
            Sounds = new Dictionary<string, SoundEffect>();

            foreach (var anim in Animations)
                anim.Value.Position = pos;

            if (facingRight)
                Direction = MoveDirection.Right;
            else
                Direction = MoveDirection.Left;

            InvincibilityTime = 400;            
            Active = true;
            Invincible = true;
            invinTimer = InvincibilityTime;
            Velocity = Vector2.Zero;
            CurrentState = State.Idle;
            Position = currentAnim.Position;
            MoveSoundDelay = 350;
            soundTimer = 0;

            gravity = 25f;
            Health = 5;
            Damage = 2;
            range = 250;
            MaxAcceleration = 10f;
            Knockback = new Vector2(2000, -300);
        }

        public override void Update(GameTime gameTime)
        {                        
            foreach (var player in manager.Players)
            {
                if (invinTimer > 0) invinTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                else Invincible = false;

                if (CurrentState != State.Dying && CollisionBox.Intersects(player.CollisionBox) && !player.Invincible)
                {
                    var knockbackDir = Math.Sign(player.Position.X - Position.X);
                    var knockback = Knockback / 4 * new Vector2(knockbackDir, 1);

                    player.GetHit(knockback, Damage);
                }
                    

                switch (CurrentState)
                {
                    case State.Dying:
                        manager.CreateBlob(Position, currentAnim.FacingRight);
                        Active = false;                       
                        break;
                                  

                    case State.Walking:
                        if (soundTimer > 0) soundTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                        else
                        {
                            soundTimer = MoveSoundDelay;
                            PlaySound("move", 0.01f);
                        }
                            

                        if (player.CollisionBox.Center.X - CollisionBox.Center.X > 0 && player.CollisionBox.Center.X - CollisionBox.Center.X <= range)
                        {
                            Direction = MoveDirection.Right;
                            CurrentState = State.Walking;
                        }

                        else if (CollisionBox.Center.X - player.CollisionBox.Center.X > 0 && CollisionBox.Center.X - player.CollisionBox.Center.X <= range)
                        {
                            Direction = MoveDirection.Left;
                            CurrentState = State.Walking;
                        }

                        else CurrentState = State.Idle;
                                                                                
                        break;

                    case State.Idle:
                        if (player.CollisionBox.Center.X - CollisionBox.Center.X > 0 && player.CollisionBox.Center.X - CollisionBox.Center.X <= range)
                        {
                            Direction = MoveDirection.Right;
                            CurrentState = State.Walking;
                        }

                        else if (CollisionBox.Center.X - player.CollisionBox.Center.X > 0 && CollisionBox.Center.X - player.CollisionBox.Center.X <= range)
                        {
                            Direction = MoveDirection.Left;
                            CurrentState = State.Walking;
                        }

                        else CurrentState = State.Idle;
                            
                        break;
                }

                UpdatePos(gameTime);
            }

            currentAnim.Update(gameTime);
        }


        public override void UpdatePos(GameTime gameTime)
        {
            SetXVel(gameTime);
            Position += new Vector2((float)Math.Truncate(Velocity.X * gameTime.ElapsedGameTime.TotalSeconds), 0);

            SetYVel(gameTime);
            Position += new Vector2(0, (float)Math.Truncate(Velocity.Y * gameTime.ElapsedGameTime.TotalSeconds));
        }

        void SetXVel(GameTime gameTime)
        {
            float accel = 0;
            float friction = -Velocity.X / 8;

            if (CurrentState == State.Walking)
            {
                if (Direction == MoveDirection.Right) accel = MaxAcceleration;
                else if (Direction == MoveDirection.Left) accel = -MaxAcceleration;
            }


            Velocity = new Vector2(Velocity.X + accel + friction, Velocity.Y);

            if (TouchingWall(gameTime)) Velocity = new Vector2(0, Velocity.Y);
        }

        void SetYVel(GameTime gameTime)
        {
            Velocity = new Vector2(Velocity.X, Velocity.Y + gravity);

            if (TouchingFloor(gameTime) || TouchingCeil(gameTime) || TouchingJt(gameTime) && Velocity.Y > 0)
                Velocity = new Vector2(Velocity.X, 0);
        }

        public override void GetHit(Vector2 knockback, int damage)
        {
            Health -= damage;
            Invincible = true;
            invinTimer = InvincibilityTime;
            Velocity = knockback / 4;

            if (Health > 0)            
                PlaySound("hit", 0.01f);                
            
            else
            {
                PlaySound("death", 0.01f);
                CurrentState = State.Dying;
            }
        }

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
