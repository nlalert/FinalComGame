using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class TowerEnemy : BaseEnemy
    {
        private float shootCooldown = 2.0f;
        private float shootTimer;

        public TowerBullet TowerBullet;

        public TowerEnemy(Texture2D texture, SpriteFont font) 
            : base(texture, font)
        {
            DetectionRange = 900f;
            AttackRange = 900f;
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
                if (distanceToPlayer <=AttackRange && shootTimer >= shootCooldown && HaveLineOfSight(tileMap))
                {
                    ShootBullet(gameObjects);
                    shootTimer = 0;
                }
            }
        }
        private void ShootBullet(List<GameObject> gameObjects)
        {
            Vector2 direction = Vector2.Normalize(Singleton.Instance.Player.Position - Position);
            Projectile bullet = TowerBullet.Clone() as Projectile;
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
