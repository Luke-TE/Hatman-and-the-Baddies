using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using PlatformerProject.Core;
using PlatformerProject.Effects;

namespace PlatformerProject.Enemies
{
    class Goblin : Enemy
    {
        static Random random;
        enum State { Idle, Running, Attacking, Dying }
        enum MoveDirection { None, Left, Right }

        #region Fields        

        State currentState;
        MoveDirection direction;
        int range;
        Vector2 position;

        #endregion


        #region Properties        

        public float AttackChance { get; set; }
        public override Rectangle CollisionBox => Animations["Idle"].DestRect;        

        public override Vector2 Position
        {
            get => position;
            set
            {
                position = value;

                foreach (var anim in Animations)
                    anim.Value.Position = position;
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

        public Rectangle AttackingHitbox => CollisionBox;

        #endregion


        #region Methods

        static Goblin()
        {
            random = new Random();
        }

        public Goblin(GameObjectManager manager, Dictionary<string, TextureAnimation> animations, Vector2 pos, bool facingRight)
        {
            this.manager = manager;
            Animations = animations;

            foreach (var anim in Animations)
                anim.Value.Position = pos;

            if (facingRight)
                Direction = MoveDirection.Right;
            else
                Direction = MoveDirection.Left;
            

            Sounds = new Dictionary<string, SoundEffect>();

            Active = true;

            Velocity = Vector2.Zero;
            Knockback = new Vector2(200, -300);
            CurrentState = State.Idle;
            Position = currentAnim.Position;
            AttackChance = 0.025f;

            range = 500;
            gravity = 15f;
            Health = 1;
            Damage = 1;
            Invincible = false;
            MaxAcceleration = 50f;

        }

        public override void Update(GameTime gameTime)
        {
            foreach (var player in manager.Players)
            {                                        
                switch (CurrentState)
                {
                    case State.Dying:
                        if (!currentAnim.Active)
                            Active = false;
                        break;


                    case State.Attacking:
                        if (currentAnim.Active)
                        {                                                        
                            if (AttackingHitbox.Intersects(player.CollisionBox) && !player.Invincible)
                            {
                                var knockbackDir = Math.Sign(player.Position.X - Position.X);
                                var knockback = Knockback * new Vector2(knockbackDir, 1);

                                PlaySound("attack", 0.01f);
                                player.GetHit(knockback, Damage);
                            }
                                
                        }
                        else
                            CurrentState = State.Idle;
                        
                        break;

                    case State.Running:
                        if (player.CollisionBox.Center.X - CollisionBox.Center.X >= 0 && player.CollisionBox.Center.X - CollisionBox.Center.X <= range)
                        {
                            Direction = MoveDirection.Right;
                            CurrentState = State.Running;
                        }

                        else if (CollisionBox.Center.X - player.CollisionBox.Center.X > 0 && CollisionBox.Center.X - player.CollisionBox.Center.X <= range)
                        {
                            Direction = MoveDirection.Left;
                            CurrentState = State.Running;
                        }

                        else
                            CurrentState = State.Idle;

                        if (AttackingHitbox.Intersects(player.CollisionBox) && random.NextDouble() < AttackChance && !player.Invincible)                                                                                
                            CurrentState = State.Attacking;
                        
                            

                        break;

                    case State.Idle:
                        if (player.CollisionBox.Center.X - CollisionBox.Center.X >= 0 && player.CollisionBox.Center.X - CollisionBox.Center.X <= range)
                        {
                            Direction = MoveDirection.Right;
                            CurrentState = State.Running;
                        }

                        else if (CollisionBox.Center.X - player.CollisionBox.Center.X > 0 && CollisionBox.Center.X - player.CollisionBox.Center.X <= range)
                        {
                            Direction = MoveDirection.Left;
                            CurrentState = State.Running;
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

            if (CurrentState == State.Running)
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
            Health = 0;
            Velocity = knockback;
            Invincible = true;
            PlaySound("death", 0.01f);
            CurrentState = State.Dying;
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
