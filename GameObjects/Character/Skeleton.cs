using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    class SkeletonEnemy : BaseEnemy
    {
        public int LimitIdlePatrol;
        private Vector2 _patrolCenterPoint;
        public float IgnorePlayerDuration;
        private float _ignorePlayerTimer;
        private bool _isIgnoringPlayer;
        public SkeletonEnemy(Texture2D texture) : base(texture) { 
            _texture = texture;
        }

        public override void Reset()
        {
            Console.WriteLine("Reset Skeleton");

            CanCollideTile = true;
            _ignorePlayerTimer = 0f;
            _isIgnoringPlayer = false;
            _patrolCenterPoint = Position;

            base.Reset();
        }

        public override void AddAnimation()
        {
            Animation = new Animation(_texture, 64, 64, new Vector2(64*8 , 64*3), 12);

            Animation.AddAnimation("idle", new Vector2(0, 0), 8);
            Animation.AddAnimation("walk", new Vector2(0, 1), 8);
            Animation.AddAnimation("hurt",new Vector2(0, 7), 1);
            Animation.AddAnimation("die",new Vector2(0, 2), 6);

            Animation.ChangeAnimation(_currentAnimation);
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (CurrentState == EnemyState.Dead || CurrentState == EnemyState.Dying)
            {
                IsActive = false;
            }

            UpdateInvincibilityTimer(deltaTime);
            switch (CurrentState)
            {
                case EnemyState.Idle:
                    AI_IdlePatrol(deltaTime, gameObjects, tileMap);
                    break;
                case EnemyState.Chase:
                    AI_ChasingPlayer(deltaTime, gameObjects, tileMap);
                    break;
                case EnemyState.Cooldown:
                    AI_CoolDown(deltaTime, gameObjects, tileMap);
                    break;
            }
            
            UpdateAnimation(deltaTime);
            base.Update(gameTime, gameObjects, tileMap);
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            string animation = "idle";

            if (Velocity.X != 0f)
            {
                animation = "walk";
            }

            if (CurrentState == EnemyState.Idle && CurrentState == EnemyState.Cooldown)
            {
                Animation.SetFPS(12);
            }
            else
            {
                Animation.SetFPS(24);
            }

            if(_currentAnimation != animation)
            {
                _currentAnimation = animation;
                switch (animation)
                {
                    default:
                        Animation.ChangeAnimation(_currentAnimation);
                        break;
                }    
            }
            base.UpdateAnimation(deltaTime);
        }

        public override void DropItem(List<GameObject> gameObjects)
        {
            base.DropItem(gameObjects);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            ////DrawDebug(spriteBatch);
        }

        protected override void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 20);
            string directionText = Direction != 1 ? "Left" : "Right";
            string displayText = $"State {CurrentState}\nPatrolDis {(Position.X - _patrolCenterPoint.X)}\nCHp {Health} \nignore{_ignorePlayerTimer}";
            spriteBatch.DrawString(Singleton.Instance.GameFont, displayText, textPosition, Color.White);
        }

        private void AI_IdlePatrol(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            if (Math.Abs(Position.X - _patrolCenterPoint.X) >= LimitIdlePatrol)
            {
                Direction *= -1;
            }

            Velocity.X = 50f * Direction;
            if(this.HaveLineOfSightOfPlayer(tileMap) && Vector2.Distance(Singleton.Instance.Player.Position,this.Position) <=100)
            {
                Console.WriteLine("Skeleton sees the player! Switching to chase mode.");
                CurrentState = EnemyState.Chase;
            }
        }

        private void AI_ChasingPlayer(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

            if (Singleton.Instance.Player == null) return; // Ensure player exists

            float distanceToPlayer = Vector2.Distance(Singleton.Instance.Player.Position, this.Position);

            // Check if the enemy still sees the player
            if (!this.HaveLineOfSightOfPlayer(tileMap) || distanceToPlayer > 400) // Max chase range
            {
                Console.WriteLine("Skeleton lost sight of the player. Returning to idle patrol.");
                CurrentState = EnemyState.Idle;
                _patrolCenterPoint = this.Position;
                return;
            }

            // Determine direction towards player
            int moveDirection = (Singleton.Instance.Player.Position.X > this.Position.X) ? 1 : -1;
            Direction = moveDirection;

            // Move towards the player
            Velocity.X = 80f * Direction; //faster when chasing
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
        }

        private void AI_CoolDown(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            if (Math.Abs(Position.X - _patrolCenterPoint.X) >= LimitIdlePatrol)
            {
                Direction *= -1;
            }

            Velocity.X = 50f * Direction;
            _ignorePlayerTimer -= deltaTime;

            if(_ignorePlayerTimer <=0 && _isIgnoringPlayer == true){
                CurrentState = EnemyState.Idle;
                _isIgnoringPlayer = false;
            }
        }


        public override void OnCollisionHorizon()
        {
            if (CurrentState == EnemyState.Idle || CurrentState == EnemyState.Cooldown)
            {
                Console.WriteLine("Collided");
                Direction *= -1;
            }
            else if (CurrentState == EnemyState.Chase)
            {
                Console.WriteLine("test jump");
                if(Velocity.Y == 0)
                    Velocity.Y = -500;
            }
            
            base.OnCollisionHorizon();
        }

        public override void OnCollidePlayer()
        {
            Console.WriteLine("Skeleton hit player");
            //skeleton hurt it self as his bone is fragiles
            OnHit(Health / 10, true);
            CurrentState = EnemyState.Cooldown;
            _patrolCenterPoint = Position;
            _ignorePlayerTimer = IgnorePlayerDuration;
            _isIgnoringPlayer = true;
            base.OnCollidePlayer();
        }
    }
}
