using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class SoulMinion : Projectile
    {
        public SoulBullet soulBullet; 
        private float spriteTimer; 
        private int _currentFrame; // Current frame index
        private const float frameDuration = 0.1f; // 0.5 sec per frame
        private const int frameWidth = 12; // Each frame is 12x12 pixels
        private const int frameHeight = 12;
        private const int totalFrames = 4; // 4 frames in total
        private float angle; // Angle in radians
        private float orbitSpeed = 1.5f; // Speed of rotation (radians per second)
        private float orbitRadius = 12f; // Radius of the orbi
        private float shootCooldown = 0.5f; // 3 sec cooldown
        private float shootTimer = 0f; 
        public SoulMinion(Texture2D texture) : base(texture)
        {
            CanCollideTile = false;
            CanHitPlayer = false;
            _currentFrame = 0;
            spriteTimer = 0f;
        }
        public override void Update(GameTime gameTime, System.Collections.Generic.List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            spriteTimer += deltaTime;

            if (spriteTimer >= frameDuration)
            {
                _currentFrame = (_currentFrame + 1) % totalFrames; // Loop through frames
                spriteTimer = 0f;
            }

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
            base.Update(gameTime, gameObjects, tileMap);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            // spriteBatch.Draw(_texture,Viewport,Color.White);
            // spriteBatch.Draw(_texture, Position, Color.White);
            // Rectangle sourceRectangle = new Rectangle(_currentFrame * frameWidth, 0, frameWidth, frameHeight);
            Rectangle sourceRectangle = new Rectangle(0, _currentFrame * frameHeight, frameWidth, frameHeight);
            spriteBatch.Draw(_texture, Position, sourceRectangle, Color.White);
            DrawDebug(spriteBatch);
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
