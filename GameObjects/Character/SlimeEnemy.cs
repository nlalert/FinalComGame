using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    class SlimeEnemy : BaseEnemy
    {
        public float JumpCooldown;
        private float _jumpTimer;
        public float Friction;

        public SlimeEnemy(Texture2D texture, SpriteFont font) : base(texture, font) { }

        public override void Reset()
        {
            Console.WriteLine("Slime respawned!");
            _jumpTimer = 0f;
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (CurrentState == EnemyState.Dead || CurrentState == EnemyState.Dying)
            {
                IsActive = false;
                return;
            }

            UpdateInvincibilityTimer(deltaTime);

            switch (CurrentState)
            {
                case EnemyState.Idle:
                    AI_Idle(deltaTime, gameObjects, tileMap);
                    break;
                case EnemyState.Jump:
                    AI_Jump(deltaTime, gameObjects, tileMap);
                    break;
            }
            
            UpdateAnimation(deltaTime);
            base.Update(gameTime, gameObjects, tileMap);
        }

        private void AI_Idle(float deltaTime,List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            _jumpTimer -= deltaTime;
            Velocity.X *= Friction;
            if (_jumpTimer <= 0)
            {
                CurrentState = EnemyState.Jump;
                _jumpTimer = JumpCooldown;
            }
        }

        private void AI_Jump(float deltaTime,List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            Velocity.X *= Friction;
            if (!_isJumping)
            {
                _isJumping = true;
                int horizontalDir = (Singleton.Instance.Player != null && HaveLineOfSight(tileMap)) ? Math.Sign(Singleton.Instance.Player.GetPlayerCenter().X - Position.X) : (Singleton.Instance.Random.Next(0, 2) == 0) ? 1 : -1;
                // float horizontalDir = Math.Sign(player.GetPlayerCenter().X - Position.X);
                float horizontalSpeed = JumpStrength * 0.5f * (0.8f + 0.4f * (float)Singleton.Instance.Random.NextDouble());
                Velocity = new Vector2(horizontalDir * horizontalSpeed, -JumpStrength * 0.8f);
            }
        }

        public override void OnLandVerticle()
        {
            _isJumping = false;
            CurrentState = EnemyState.Idle;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // spriteBatch.Draw(_texture,
            //     Position,
            //     Viewport,
            //     Color.Red,
            //     0f, 
            //     Vector2.Zero,
            //     1f,
            //     SpriteEffects.None, 
            //     0f);
            base.Draw(spriteBatch);
            DrawDebug(spriteBatch);
        }
        private void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 20);
            string directionText = Direction != 1 ? "Left" : "Right";
            string displayText = $"State {CurrentState}\n Charge timer {_jumpTimer}";
            spriteBatch.DrawString(_DebugFont, displayText, textPosition, Color.White);
        }
        protected override void ApplyGravity(float deltaTime)
        {
            Velocity.Y +=Singleton.GRAVITY/1.75f * deltaTime;
        }
    }
}
