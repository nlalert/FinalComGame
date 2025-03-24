using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class HellhoundEnemy : BaseEnemy
    {
        private int _limitIdlePatrol = 100;
        private Vector2 _patrolCenterPoint;
        private Vector2 _dashTarget;
        private float chaseDuration = 5f; // 5 seconds of chasing
        private float chaseTimer = 0f;
        private float chargeTime = 2.0f; 
        private float chargeTimer = 0f;
        private bool isDashing = false;
        private float dashTimer = 0.0f;
        private float dashTime = 1.0f;


        public HellhoundEnemy(Texture2D texture, SpriteFont font) : base(texture, font) { }
        public override void Reset()
        {
            Console.WriteLine("Reset Hellhound");
            MaxHealth = 100f;
            AttackDamage = 8f;
            _patrolCenterPoint = Position;
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!HasSpawned) return;

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
                case EnemyState.Charging:
                    AI_Charging(deltaTime, gameObjects, tileMap);
                    break;
                case EnemyState.Dash:
                    AI_Dash(deltaTime, gameObjects, tileMap);
                    break;
                case EnemyState.Chase:
                    AI_ChasingPlayer(deltaTime, gameObjects, tileMap);
                    break;
            }

            UpdateAnimation(deltaTime);
            base.Update(gameTime, gameObjects, tileMap);
        }

        public override void OnSpawn()
        {
            Console.WriteLine("Hellhound emerges from the shadows!");
        }

        public override void OnDead()
        {
            Console.WriteLine("Hellhound fades into darkness...");
            base.OnDead();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            DrawDebug(spriteBatch);
        }
        private void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 20);
            string directionText = Direction != 1 ? "Left" : "Right";
            string displayText = $"State {CurrentState}\n Charge timer {chargeTimer} ";
            spriteBatch.DrawString(_DebugFont, displayText, textPosition, Color.White);
        }
        private void AI_IdlePatrol(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

            if (Math.Abs(Position.X - _patrolCenterPoint.X) >= _limitIdlePatrol)
            {
                Direction *= -1;
            }

            Velocity.X = 60f * Direction;

            if (Singleton.Instance.Player != null && this.HaveLineOfSight(tileMap) && Vector2.Distance(Singleton.Instance.Player.Position, this.Position) <= 100)
            {
                Console.WriteLine("Hellhound spots the player! Preparing to charge.");
                CurrentState = EnemyState.Charging;
                chargeTimer = chargeTime;
                _dashTarget = Singleton.Instance.Player.GetPlayerCenter(); // Lock on to player's position
            }
        }

        private void AI_Charging(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

            chargeTimer -= deltaTime;
            if (chargeTimer <= 0)
            {
                Console.WriteLine("Hellhound dashes!");
                CurrentState = EnemyState.Dash;
                isDashing = true;
                dashTimer = dashTime;
                Velocity = (_dashTarget - Position);
                Velocity.Normalize();
                Velocity *= 1000f;//Dash speed
                
            }
            else{
                Velocity.X *= 0.95f;
                if(this.HaveLineOfSight(tileMap)){ //while have line of sight will aim add player all time 
                    _dashTarget = Singleton.Instance.Player.GetPlayerCenter();
                }
            }
        }

        private void AI_Dash(float deltaTime,List<GameObject> gameObjects, TileMap tileMap)
        { 
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            if (isDashing)
            {
                dashTimer -= deltaTime;
                if (dashTimer <=0 )
                {
                    Console.WriteLine("Hellhound finished dashing, switching to chase mode.");
                    isDashing = false;
                    CurrentState = EnemyState.Chase;
                    chaseTimer = chaseDuration;
                }
            }
        }

        private void AI_ChasingPlayer(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

            chaseTimer -= deltaTime;

            if (!this.HaveLineOfSight(tileMap) || chaseTimer <= 0)
            {
                Console.WriteLine("Hellhound lost sight of the player, returning to idle.");
                CurrentState = EnemyState.Idle;
                _patrolCenterPoint = this.Position;
                return;
            }

            // Move toward the player
            int moveDirection = (Singleton.Instance.Player.Position.X > this.Position.X) ? 1 : -1;
            Direction = moveDirection;
            Velocity.X = 100f * Direction; // Faster speed when chasing

            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);

            if (chaseTimer <= 0 && this.HaveLineOfSight(tileMap))
            {
                Console.WriteLine("Hellhound still sees the player, preparing to charge again.");
                CurrentState = EnemyState.Charging;
                chargeTimer = chargeTime;
                _dashTarget = Singleton.Instance.Player.Position;
            }
        }

        public override void OnCollisionHorizon()
        {
            if (CurrentState == EnemyState.Idle || CurrentState == EnemyState.Charging)
            {
                Console.WriteLine("Hellhound collided and turned around.");
                Direction *= -1;
            }
            else if (CurrentState == EnemyState.Chase )
            {
                Console.WriteLine("Hellhound jumps over obstacles.");
                if (Velocity.Y == 0)
                    Velocity.Y = -900f; // Jump over obstacles
            }else if(CurrentState == EnemyState.Dash){
                Console.WriteLine("Hellhound dash Hit wall, changing to chase state");
                isDashing = false;
                CurrentState = EnemyState.Chase;
                chaseTimer = chaseDuration;
            }

            base.OnCollisionHorizon();
        }

        public override void OnCollidePlayer()
        {
            Console.WriteLine("Hellhound bites the player!");
            Singleton.Instance.Player.OnCollideNPC(this,AttackDamage);
            if(CurrentState == EnemyState.Chase){
                CurrentState = EnemyState.Idle;
            }
            base.OnCollidePlayer();
        }
    }
}
