using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class HellhoundEnemy : BaseEnemy
    {
        public int LimitIdlePatrol;
        private Vector2 _patrolCenterPoint;
        private Vector2 _dashTarget;
        public float ChaseDuration;
        private float _chaseTimer;
        public float ChargeTime; 
        private float _chargeTimer;
        private bool _isDashing;
        private float _dashTimer;
        public float DashDuration;
        public SoundEffect DogSound;

        public HellhoundEnemy(Texture2D texture) : base(texture) { 
            _texture = texture;
        }
        public override void Reset()
        {
            //Console.WriteLine("Reset Hellhound");
            CanCollideTile = true;
            _isDashing = false;
            _dashTimer = 0f;
            _chargeTimer = 0f;
            _chaseTimer = 0f;

            _patrolCenterPoint = Position;

            base.Reset();
        }

        public override void AddAnimation(){
            Animation = new Animation(_texture, 80, 64, new Vector2(80*8, 64*3), 24);

            Animation.AddAnimation("idle", new Vector2(0,0), 8);
            Animation.AddAnimation("charge", new Vector2(0,1), 8);
            Animation.AddAnimation("dash", new Vector2(0,2), 4);

            Animation.ChangeAnimation("idle");
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
                    if (Velocity.X > 0)
                        Direction = 1;
                    else if (Velocity.X < 0)
                        Direction = -1;
                    AI_IdlePatrol(deltaTime, gameObjects, tileMap);
                    break;

                case EnemyState.Charging:
                    if (Singleton.Instance.Player.GetPlayerCenter().X > Position.X)
                        Direction = 1;
                    else if (Singleton.Instance.Player.GetPlayerCenter().X < Position.X)
                        Direction = -1;
                    AI_Charging(deltaTime, gameObjects, tileMap);
                    break;

                case EnemyState.Dash:
                    AI_Dash(deltaTime, gameObjects, tileMap);
                    break;

                case EnemyState.Chase:
                    if (Singleton.Instance.Player.GetPlayerCenter().X > Position.X)
                        Direction = 1;
                    else if (Singleton.Instance.Player.GetPlayerCenter().X < Position.X)
                        Direction = -1;
                    AI_ChasingPlayer(deltaTime, gameObjects, tileMap);
                    break;
            }

            UpdateAnimation(deltaTime);
            base.Update(gameTime, gameObjects, tileMap);
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            string animation = "idle";

            switch (CurrentState)
            {
                case EnemyState.Charging:
                    animation = "charge";
                    break;

                case EnemyState.Dash:
                    animation = "dash";
                    break;
                    
                default:
                    animation = "idle";
                    break;
            }

                
            if(_currentAnimation != animation && !Animation.IsTransition)
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

        public override void OnSpawn()
        {
            //Console.WriteLine("Hellhound emerges from the shadows!");
        }

        public override void OnDead(List<GameObject> gameObjects)
        {
            //Console.WriteLine("Hellhound fades into darkness...");
            base.OnDead(gameObjects);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffect = Direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color color = IsInvincible() ? Color.Red : Color.White;

            spriteBatch.Draw(
                Animation.GetTexture(),
                GetDrawingPosition(),
                Animation.GetCurrentFrame(),
                color,
                0f, 
                Vector2.Zero,
                Scale,
                spriteEffect, 
                0f
            );

            ////DrawDebug(spriteBatch);
        }
        protected override void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 20);
            string directionText = Direction != 1 ? "Left" : "Right";
            string displayText = $"State {CurrentState}\n Charge timer {_chargeTimer} ";
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

            Velocity.X = 60f * Direction;

            if (Singleton.Instance.Player != null && this.HaveLineOfSightOfPlayer(tileMap) && Vector2.Distance(Singleton.Instance.Player.Position, this.Position) <= 100)
            {
                //Console.WriteLine("Hellhound spots the player! Preparing to charge.");
                CurrentState = EnemyState.Charging;
                _chargeTimer = ChargeTime;
                _dashTarget = Singleton.Instance.Player.GetPlayerCenter(); // Lock on to player's position
            }
        }

        private void AI_Charging(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

            _chargeTimer -= deltaTime;
            if (_chargeTimer <= 0)
            {
                //Console.WriteLine("Hellhound dashes!");
                CurrentState = EnemyState.Dash;
                _isDashing = true;
                _dashTimer = DashDuration;
                Velocity = (_dashTarget - Position);
                Velocity.Normalize();
                Velocity *= 600f;//Dash speed
                DogSound.Play();
            }
            else{
                Velocity.X *= 0.95f;
                if(this.HaveLineOfSightOfPlayer(tileMap)){ //while have line of sight will aim add player all time 
                    _dashTarget = Singleton.Instance.Player.GetPlayerCenter();
                }
            }
        }

        private void AI_Dash(float deltaTime,List<GameObject> gameObjects, TileMap tileMap)
        { 
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            if (_isDashing)
            {
                IsIgnorePlatform = true;
                _dashTimer -= deltaTime;
                if (_dashTimer <=0 )
                {
                    //Console.WriteLine("Hellhound finished dashing, switching to chase mode.");
                    _isDashing = false;
                    IsIgnorePlatform = false;
                    CurrentState = EnemyState.Chase;
                    _chaseTimer = ChaseDuration;
                }
            }
        }

        private void AI_ChasingPlayer(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

            _chaseTimer -= deltaTime;

            if (!this.HaveLineOfSightOfPlayer(tileMap) || _chaseTimer <= 0)
            {
                //Console.WriteLine("Hellhound lost sight of the player, returning to idle.");
                CurrentState = EnemyState.Idle;
                _patrolCenterPoint = this.Position;
                return;
            }

            // Move toward the player
            int moveDirection = (Singleton.Instance.Player.Position.X > this.Position.X) ? 1 : -1;
            Direction = moveDirection;
            Velocity.X = 100f * Direction; // Faster speed when chasing

            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);

            if (_chaseTimer <= 0 && this.HaveLineOfSightOfPlayer(tileMap))
            {
                //Console.WriteLine("Hellhound still sees the player, preparing to charge again.");
                CurrentState = EnemyState.Charging;
                _chargeTimer = ChargeTime;
                _dashTarget = Singleton.Instance.Player.Position;
            }
        }

        public override void OnCollisionHorizon()
        {
            if (CurrentState == EnemyState.Idle || CurrentState == EnemyState.Charging)
            {
                //Console.WriteLine("Hellhound collided and turned around.");
                Direction *= -1;
            }
            else if (CurrentState == EnemyState.Chase )
            {
                //Console.WriteLine("Hellhound jumps over obstacles.");
                if (Velocity.Y == 0)
                    Velocity.Y = -600f; // Jump over obstacles
            }else if(CurrentState == EnemyState.Dash){
                //Console.WriteLine("Hellhound dash Hit wall, changing to chase state");
                _isDashing = false;
                CurrentState = EnemyState.Chase;
                _chaseTimer = ChaseDuration;
            }

            base.OnCollisionHorizon();
        }

        public override void OnCollidePlayer()
        {
            //Console.WriteLine("Hellhound bites the player!");
            Singleton.Instance.Player.OnCollideNPC(this,AttackDamage);
            if(CurrentState == EnemyState.Chase){
                CurrentState = EnemyState.Idle;
            }
            base.OnCollidePlayer();
        }
    }
}
