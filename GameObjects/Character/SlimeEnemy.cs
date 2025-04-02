using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class SlimeEnemy : BaseEnemy
    {
        public float JumpCooldown;
        private float _jumpTimer;
        public float Friction;

        public SlimeEnemy(Texture2D texture) : base(texture) { 
            _texture = texture;
            CanCollideTile = true;
        }

        public override void AddAnimation()
        {
            Animation = new Animation(_texture, 48, 48, new Vector2(48*8 , 48*6), 12);

            Animation.AddAnimation("idle", new Vector2(0, 0), 8);
            Animation.AddAnimation("charge", new Vector2(0, 1), 6);
            Animation.AddAnimation("jump", new Vector2(0, 2), 5);
            Animation.AddAnimation("float", new Vector2(0, 3), 1);
            Animation.AddAnimation("land", new Vector2(0, 4), 5);
            Animation.AddAnimation("die", new Vector2(0, 5), 7);

            Animation.ChangeAnimation(_currentAnimation);
        }

        public override void Reset()
        {
            Console.WriteLine("Slime respawned!");
            _jumpTimer = 0f;
            base.Reset();
            Animation.ChangeAnimation(_currentAnimation);
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

        protected override void UpdateAnimation(float deltaTime)
        {
            string animation = "idle";
            if (CurrentState == EnemyState.Dying){
                animation = "die";
            }
            else if (_jumpTimer < 0.75 && Velocity.Y == 0){
                animation = "charge"; 
            }
            else if(Velocity.Y != 0){
                animation = "jump";
            }
            else if (_jumpTimer >= 1)
                animation = "land";

            if(_currentAnimation != animation)
            {
                _currentAnimation = animation;
                switch (animation)
                {
                    case "jump" :
                        Animation.ChangeTransitionAnimation(_currentAnimation, "float");
                        break;
                    case "land" :
                        Animation.ChangeTransitionAnimation(_currentAnimation, "idle");
                        break;
                    case "charge": 
                    case "die":
                        Animation.ChangeAnimation(_currentAnimation);
                        Animation.IsLooping = false;
                        break;
                    default:
                        Animation.ChangeAnimation(_currentAnimation);
                        break;
                }    
            }
            
            base.UpdateAnimation(deltaTime);
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
            Color color = IsInvincible() ? Color.Red : Color.White;

            spriteBatch.Draw(
                Animation.GetTexture(),
                GetDrawingPosition(),
                Animation.GetCurrentFrame(),
                color,
                0f, 
                Vector2.Zero,
                Scale,
                SpriteEffects.None, 
                0f
            );

            DrawDebug(spriteBatch);
        }
        
        protected override void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 20);
            string directionText = Direction != 1 ? "Left" : "Right";
            string displayText = $"State {CurrentState}\nHP {Health} \nCharge timer {_jumpTimer}";
            spriteBatch.DrawString(Singleton.Instance.GameFont, displayText, textPosition, Color.White);
        }
        protected override void ApplyGravity(float deltaTime)
        {
            Velocity.Y +=Singleton.GRAVITY/1.75f * deltaTime;
        }
    }
}
