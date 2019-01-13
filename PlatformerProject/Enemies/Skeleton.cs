using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PlatformerProject.Core;
using PlatformerProject.Effects;

namespace PlatformerProject.Enemies
{
    class Skeleton : Enemy
    {
        enum State { Idle, Walking, Reacting, Attacking, Hurt, Dying }
        enum MoveDirection { None, Left, Right }
        enum AttackState { None = -1, Preparing = 0, Attacking = 7, Finished = 9 }

        #region Fields        

        int elapsedTime, waitTime;
        bool waitingForever;
        State currentState;
        MoveDirection direction;
        Vector2 position;
        AttackState CurrentAttackState;

        #endregion


        #region Properties        

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
                            anim.Value.Position = new Vector2(position.X + 15, position.Y - 5);
                        else if (Direction == MoveDirection.Left)
                            anim.Value.Position = new Vector2(position.X - 15, position.Y - 5);
                    }
                    else anim.Value.Position = position;

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

        public Skeleton(GameObjectManager manager, Dictionary<string, TextureAnimation> animations, Vector2 pos, bool facingRight, bool waiting = false)
        {
            this.manager = manager;
            Animations = animations;
            
            foreach (var anim in Animations)
                anim.Value.Position = pos;

            if (facingRight)
                Direction = MoveDirection.Right;
            else
                Direction = MoveDirection.Left;

            waitingForever = waiting;


            Sounds = new Dictionary<string, SoundEffect>();

            Active = true;

            if (waitingForever)
                CurrentState = State.Idle;
            else
                CurrentState = State.Walking;

            CurrentAttackState = AttackState.None;
            Velocity = Vector2.Zero;
            Knockback = new Vector2(1000, -300);
            Position = currentAnim.Position;

            gravity = 25f;
            Health = 3;
            Damage = 2;
            Invincible = false;
            MaxAcceleration = 10f;
            elapsedTime = 0;                  
            waitTime = 2000;


        }

        public override void Update(GameTime gameTime)
        {                        
            foreach (var player in manager.Players)
            {
                var knockbackDir = Math.Sign(player.Position.X - Position.X);
                var knockback = Knockback * new Vector2(knockbackDir, 1);

                if (CurrentState != State.Dying && CollisionBox.Intersects(player.CollisionBox) && !player.Invincible)
                    player.GetHit(knockback, Damage);

                switch (CurrentState)
                {
                    case State.Dying:
                        if (!currentAnim.Active)
                            Active = false;
                        break;


                    case State.Reacting:
                        if (!currentAnim.Active)
                        {
                            CurrentState = State.Attacking;
                            CurrentAttackState = AttackState.Preparing;

                            PlaySound("react", 0.01f);
                        }
                            
                        
                        break;


                    case State.Attacking:
                        var enumVals = Enum.GetValues(typeof(AttackState));
                        Array.Sort(enumVals);

                        foreach (var enumVal in enumVals)
                        {
                            if (currentAnim.CurrentFrame >= (int)enumVal)
                                CurrentAttackState = (AttackState)enumVal;                                                            
                        }

                        if (currentAnim.CurrentFrame == (int)AttackState.Attacking)
                        {
                            Invincible = false;
                            PlaySound("attack", 0.01f);
                        }

                        switch (CurrentAttackState)
                        {
                            case AttackState.Preparing:                                
                                break;

                            case AttackState.Attacking:
                                if (AttackingHitbox.Intersects(player.CollisionBox) && !player.Invincible)
                                    player.GetHit(knockback, Damage);
                                
                                break;

                            case AttackState.Finished:
                                if (!currentAnim.Active)
                                {
                                    CurrentAttackState = AttackState.None;
                                    if (waitingForever)
                                        CurrentState = State.Idle;
                                    else
                                        CurrentState = State.Walking;
                                }
                                    

                                break;

                            case AttackState.None:
                                throw new Exception();
                        }
                                                
                        break;


                    case State.Hurt:
                        if (!currentAnim.Active)
                        {
                            CurrentState = State.Attacking;
                            CurrentAttackState = AttackState.Preparing;
                        }
                                                                                
                        break;                                        


                    case State.Walking:
                        if (AttackingHitbox.Intersects(player.CollisionBox))
                            CurrentState = State.Reacting;                        

                        if (TouchingWall(gameTime))                        
                            CurrentState = State.Idle;                            
                                                
                        break;


                    case State.Idle:
                        if (AttackingHitbox.Intersects(player.CollisionBox))
                            CurrentState = State.Reacting;

                        if (!waitingForever)
                        {
                            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                            if (elapsedTime > waitTime)
                            {
                                CurrentState = State.Walking;

                                if (Direction == MoveDirection.Right) Direction = MoveDirection.Left;
                                else if (Direction == MoveDirection.Left) Direction = MoveDirection.Right;

                                elapsedTime = 0;
                            }

                        }
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
            if (!waitingForever) Velocity = knockback;
            Invincible = true;                        

            if (Health > 0)
            {
                PlaySound("hit", 0.01f);
                CurrentState = State.Hurt;
            }                
            else
            {
                PlaySound("death", 0.01f);
                CurrentState = State.Dying;
            }
                

            if (knockback.X > 0)
                Direction = MoveDirection.Left;
            else
                Direction = MoveDirection.Right;
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
