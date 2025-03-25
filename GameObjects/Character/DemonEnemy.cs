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

        public Projectile DemonBullet;

        public DemonEnemy(Texture2D texture, SpriteFont font) 
            : base(texture, font)
        {
            DetectionRange =300f;
            AttackRange = 400f;
            DetectionRange =300f;
            AttackRange = 400f;
            verticalOffset = Singleton.Instance.Random.Next(0, 100); // Randomize hover start
            CanCollideTile = true;
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            shootTimer += deltaTime;
            UpdateHorizontalMovement(deltaTime,gameObjects,tileMap);
            UpdateVerticalMovement(deltaTime,gameObjects,tileMap);

            if (Singleton.Instance.Player != null)
            {
                switch (CurrentState)
                {
                    case EnemyState.Idle:
                        AI_Idle(gameTime,gameObjects,tileMap,deltaTime);
                        break;

                    case EnemyState.Chase:
                        AI_Chase(gameTime,gameObjects,tileMap);
                        break;
                }
            }


            base.Update(gameTime, gameObjects, tileMap);
        }
        private void AI_Idle(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap, float deltaTime){
            float distanceToPlayer = Vector2.Distance(Position, Singleton.Instance.Player.GetPlayerCenter());

            if (distanceToPlayer <= DetectionRange && HaveLineOfSight(tileMap))
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
            if (distanceToPlayer > DetectionRange || !HaveLineOfSight(tileMap))
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

                if (distanceToPlayer <= AttackRange && shootTimer >= shootCooldown && HaveLineOfSight(tileMap))
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
            Projectile bullet = DemonBullet.Clone() as Projectile;
            bullet.Shoot(Position, direction);
            gameObjects.Add(bullet);
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
            string displayText = $"State {CurrentState}\n ";
            spriteBatch.DrawString(_DebugFont, displayText, textPosition, Color.White);
        }
    }
}
