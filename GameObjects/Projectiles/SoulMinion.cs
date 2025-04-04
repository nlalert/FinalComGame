using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class SoulMinion : Projectile
    {
        public SoulBullet soulBullet;
        private float angle; // Angle in radians
        private float orbitSpeed = 1.5f; // Speed of rotation (radians per second)
        private float orbitRadius = 12f; // Radius of the orbi
        private float shootCooldown = 1f; // 
        private float shootTimer = 0f; 
        public SoulMinion(Texture2D texture) : base(texture)
        {
            _texture = texture;
            CanCollideTile = false;
            CanHitPlayer = false;
        }

        public override void AddAnimation(){
            Animation = new Animation(_texture, 16, 16, new Vector2(16*4, 16*4), 12);
            Animation.AddAnimation("idle", new Vector2(0, 2), 4);

            Animation.ChangeAnimation("idle");
        }

        public override void Update(GameTime gameTime, System.Collections.Generic.List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //orbit
            angle += orbitSpeed * deltaTime; // Increase angle over time
            // Get player's position
            Vector2 playerPosition = Singleton.Instance.Player.GetPlayerCenter();
            // Calculate new orbit position
            Position = playerPosition + new Vector2(-Viewport.Width/2,-Singleton.TILE_SIZE * 2) + new Vector2(
                (float)Math.Cos(angle) * orbitRadius,
                (float)Math.Sin(angle) * orbitRadius
            ); 

             // Shooting logic
            shootTimer += deltaTime;
            if (shootTimer >= shootCooldown)
            {
                GameObject target = FindNearestEnemy(gameObjects);
                if (target != null)
                {
                    ShootBullet(gameObjects, target.Position);
                    shootTimer = 0; // Reset cooldown
                }
            }

            base.UpdateAnimation(deltaTime);
            base.Update(gameTime, gameObjects, tileMap);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Animation.GetTexture(),
                GetDrawingPosition(),
                Animation.GetCurrentFrame(),
                Color.White
            );
        }

        protected Vector2 GetDrawingPosition()
        {
            return  new Vector2( Position.X - (Animation.GetFrameSize().X - Viewport.Width) / 2 
                                ,Position.Y - (Animation.GetFrameSize().Y - Viewport.Height) / 2);
        }

        private void DrawDebug(SpriteBatch spriteBatch){
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 40);
            string displayText = $"StarPos : {Position} \nTexture {_texture}";
            spriteBatch.DrawString(Singleton.Instance.GameFont, displayText, textPosition , Color.White);
        }

        private void ShootBullet(List<GameObject> gameObjects, Vector2 targetPosition)
        {
            Vector2 direction = Vector2.Normalize(targetPosition - Position);
            SoulBullet bullet = soulBullet.Clone() as SoulBullet;
            
            if (bullet != null)
            {
                bullet.Shoot(Position, direction);

                gameObjects.Add(bullet);
            }
        }
        private GameObject FindNearestEnemy(List<GameObject> gameObjects)
        {
            GameObject nearestEnemy = null;
            float minDistance = float.MaxValue;

            foreach (GameObject obj in gameObjects)
            {
                if (obj is BaseEnemy enemy)
                {
                    float distance = Vector2.Distance(Position, enemy.Position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestEnemy = enemy;
                    }
                }
            }
            return nearestEnemy;
        }
    }
}
