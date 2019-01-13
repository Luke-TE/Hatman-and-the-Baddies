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
    class Blob : Enemy
    {
        enum State { Idle, Jumping, Attacking, Dying }
        enum MoveDirection { None, Left, Right }

        #region Fields        

        Vector2 position;
        int range;
        State currentState;
        MoveDirection direction;
        int jumpTimer;
        int invinTimer;

        #endregion


        #region Properties        

        public int JumpCooldown { get; set; }
        public Vector2 JumpForce { get; set; }
        public int InvincibilityTime { get; set; }        

        public override Rectangle CollisionBox
        {
            get
            {
                var widthPercent = 0.5f;
                var width = widthPercent * Animations["Idle"].DestRect.Width;
                var x = (1 - widthPercent) / 2 * Animations["Idle"].DestRect.Width + Animations["Idle"].DestRect.X;

                var heightPercent = 0.7f;
                var height = heightPercent * Animations["Idle"].DestRect.Height;
                var y = (1 - heightPercent) / 2 * Animations["Idle"].DestRect.Height + Animations["Idle"].DestRect.Y;

                return new Rectangle((int)x, (int)y, (int)width, (int)height);

            }
        }

        public override Vector2 Position
        {
            get => position;
            set
            {
                position = value;

                foreach (var anim in Animations)
                {
                    if (anim.Key == "Dying")
                    {
                        if (Direction == MoveDirection.Right)
                            anim.Value.Position = new Vector2(position.X - 25, position.Y - 30);
                        else if (Direction == MoveDirection.Left)
                            anim.Value.Position = new Vector2(position.X + 25, position.Y - 30);
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

        public Blob(GameObjectManager manager, Dictionary<string, TextureAnimation> animations, Vector2 pos, bool facingRight)
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
            Velocity = Vector2.Zero;
            CurrentState = State.Idle;
            Position = currentAnim.Position;

            JumpForce = new Vector2(400, -500);
            gravity = 25f;
            Health = 2;
            Damage = 1;
            range = 250;
            MaxAcceleration = 10f;
            Knockback = new Vector2(2000, -300);
            JumpCooldown = 1000;
            jumpTimer = 0;
            invinTimer = InvincibilityTime;
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
                        if (!currentAnim.Active)
                            Active = false;
                        break;


                    case State.Jumping:
                        if (TouchingFloor(gameTime))
                        {
                            PlaySound("land", 0.01f);
                            CurrentState = State.Idle;
                        }                            

                        break;

                    case State.Idle:
                        if (jumpTimer > 0) jumpTimer -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                        if (player.CollisionBox.Center.X - CollisionBox.Center.X > 0 && player.CollisionBox.Center.X - CollisionBox.Center.X <= range && jumpTimer <= 0)
                        {
                            Direction = MoveDirection.Right;
                            Jump();                        
                        }

                        else if (CollisionBox.Center.X - player.CollisionBox.Center.X > 0 && CollisionBox.Center.X - player.CollisionBox.Center.X <= range && jumpTimer <= 0)
                        {
                            Direction = MoveDirection.Left;
                            Jump();
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

            if (CurrentState == State.Jumping)
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

        public void Jump()
        {
            PlaySound("jump", 0.01f);
            CurrentState = State.Jumping;
            jumpTimer = JumpCooldown;
            if (direction == MoveDirection.Right)
                Velocity = JumpForce;
            else if (direction == MoveDirection.Left)
                Velocity = new Vector2(-JumpForce.X, JumpForce.Y);
        }

        public override void GetHit(Vector2 knockback, int damage)
        {
            Health -= damage;
            Invincible = true;
            invinTimer = InvincibilityTime;

            Velocity = knockback / 3;

            if (Health > 0)
                PlaySound("hit", 0.02f);

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
