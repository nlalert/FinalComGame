using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class DemonEnemy : BaseEnemy
    {
        private float shootCooldown = 2.5f;
        private float shootTimer;
        private float hoverSpeed = 50f; // Speed of horizontal movement
        private float hoverAmplitude = 75f; // Max height difference for smooth hovering
        private float hoverFrequency = 2f; // Speed of hover oscillation
        private float preferredHeight = 150f; // Preferred height above player
        private float verticalOffset; // Randomized offset for smooth hovering
        private float loopOffset =0;
        private float loopSpeed =2f;

        public DemonBullet DemonBullet;

        public DemonEnemy(Texture2D texture) 
            : base(texture)
        {
            _texture = texture;
            DetectionRange =300f;
            AttackRange = 400f;
            verticalOffset = Singleton.Instance.Random.Next(0, 100); // Randomize hover start
            CanCollideTile = true;
        }
        public override void Reset()
        {
            shootCooldown = 2.5f;
            hoverSpeed = 50f; // Speed of horizontal movement
            hoverAmplitude = 75f; // Max height difference for smooth hovering
            hoverFrequency = 2f; // Speed of hover oscillation
            preferredHeight = 150f; // Preferred height above player
            loopOffset =0;
            loopSpeed =2f;
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            shootTimer += deltaTime;
            if (CurrentState == EnemyState.Idle && shootTimer >= shootCooldown * 3/5)
            {
                shootTimer = shootCooldown * 3/5;
            }

            UpdateHorizontalMovement(deltaTime,gameObjects,tileMap);
            UpdateVerticalMovement(deltaTime,gameObjects,tileMap);
            UpdateInvincibilityTimer(deltaTime);
            if (Singleton.Instance.Player != null)
            {
                switch (CurrentState)
                {
                    case EnemyState.Idle:
                        AI_Idle(gameTime,gameObjects,tileMap,deltaTime);
                        if (Velocity.X > 0)
                            Direction = 1;
                        else if (Velocity.X < 0)
                            Direction = -1;
                        break;

                    case EnemyState.Chase:
                        AI_Chase(gameTime,gameObjects,tileMap);
                        if (Singleton.Instance.Player.GetPlayerCenter().X > Position.X)
                            Direction = 1;
                        else if (Singleton.Instance.Player.GetPlayerCenter().X < Position.X)
                            Direction = -1;
                        break;
                }
            }

            UpdateAnimation(deltaTime);
            base.Update(gameTime, gameObjects, tileMap);
        }

        public override void AddAnimation()
        {
            Animation = new Animation(_texture, 80 , 64, new Vector2(80*8, 64*3), 24);

            Animation.AddAnimation("idle", new Vector2(0, 0), 16);
            Animation.AddAnimation("charge", new Vector2(0, 2), 4);
            Animation.AddAnimation("charge_idle", new Vector2(4, 2), 4);

            Animation.ChangeAnimation("idle");
            base.AddAnimation();
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            string animation = "idle";

            if (shootTimer > shootCooldown * 3/5)
                animation = "charge";
            else
                animation = "idle";

            if(_currentAnimation != animation && !Animation.IsTransition)
            {
                _currentAnimation = animation;
                switch (animation)
                {
                    case "idle":
                            Animation.ChangeAnimation(_currentAnimation);
                        break;
                    case "charge":
                            Animation.ChangeTransitionAnimation(_currentAnimation, "charge_idle");
                            break;
                    default:
                            Animation.ChangeAnimation(_currentAnimation);
                        break;
                }    
            }

            base.UpdateAnimation(deltaTime);
        }
        private void AI_Idle(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap, float deltaTime){
            if (HaveLineOfSightOfPlayer(tileMap))
            {
                // Transition to chase state
                CurrentState = EnemyState.Chase;
            }
            else
            {
                // **Flying up and down continuously**
                loopOffset += deltaTime * loopSpeed;  // Continual loop offset

                // **Vertical motion (up and down)**
                float hoverY = (float)Math.Sin(loopOffset) * hoverAmplitude;  // Sine wave for smooth up/down motion
                Velocity.Y = hoverY ;  // Apply hover movement

                // **Move left/right constantly**
                Velocity.X = Direction * hoverSpeed ;  // Move in the current direction

                // **Collision Check: If hits a wall, switch direction**
            }
        }
        private void AI_Chase(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap){
            float distanceToPlayer = Vector2.Distance(Position, Singleton.Instance.Player.GetPlayerCenter());
            if (!HaveLineOfSightOfPlayer(tileMap))
            {
                CurrentState = EnemyState.Idle;
                Velocity = Vector2.Zero; // Stop moving
            }
            else
            {
                // **Adjust Height to Stay Around 450 Above Player**
                float targetY = Singleton.Instance.Player.GetPlayerCenter().Y - preferredHeight;
                float hoverY = (float)Math.Sin((gameTime.TotalGameTime.TotalSeconds + verticalOffset) * hoverFrequency) * hoverAmplitude;
                float newY = targetY + hoverY;
                float verticalVelocity = MathHelper.Clamp((newY - Position.Y) * 2f, -hoverSpeed, hoverSpeed);
                Velocity.Y = verticalVelocity;

                // **Move Left & Right Above Player**
                float targetX = Singleton.Instance.Player.GetPlayerCenter().X + (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 1.5f) * 150f;
                float horizontalVelocity = MathHelper.Clamp((targetX - Position.X) * 2f, -hoverSpeed, hoverSpeed);
                Velocity.X = horizontalVelocity;

                if (distanceToPlayer <= AttackRange && shootTimer >= shootCooldown && HaveLineOfSightOfPlayer(tileMap))
                {
                    ShootBullet(gameObjects);
                    shootTimer = 0;
                }
            }
        }
        public override void OnCollisionHorizon()
        {
            Direction  *= -1;
            base.OnCollisionHorizon();
        }
        private void ShootBullet(List<GameObject> gameObjects)
        {
            Vector2 direction = Vector2.Normalize(Singleton.Instance.Player.Position - Position);
            DemonBullet bullet = DemonBullet.Clone() as DemonBullet;

            Vector2 bulletPosition = new Vector2(Position.X + Viewport.Width / 2, Position.Y + Viewport.Height / 2);

            bullet.DamageAmount = bullet.BaseDamageAmount;
            bullet.Shoot(bulletPosition, direction);
            gameObjects.Add(bullet);
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

            //DrawDebug(spriteBatch);
        }
        protected override void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 20);
            string directionText = Direction != 1 ? "Left" : "Right";
            string displayText = $"State {CurrentState}\n ";
            spriteBatch.DrawString(Singleton.Instance.GameFont, displayText, textPosition, Color.White);
        }
    }
}
