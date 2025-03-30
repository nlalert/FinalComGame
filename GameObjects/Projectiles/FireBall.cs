using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class FireBall : Projectile
    {
        // Explosion parameters
        private bool _isExploding = false;
        private float _explosionRadius = 60f; // Radius of explosion area
        private float _explosionDamage; // Damage dealt by explosion
        private float _explosionDuration = 0.5f; // How long the explosion lasts in seconds
        private float _explosionTimer = 0f;
        private List<GameObject> _damagedObjects; // Track already damaged objects
        
        // Optional: explosion visual effect
        private Texture2D _explosionTexture;
        private float _explosionAlpha = 1.0f;

        public FireBall(Texture2D texture, Texture2D explosionTexture) : base(texture)
        {
            CanCollideTile = true;
            _explosionTexture = explosionTexture;
            _damagedObjects = new List<GameObject>();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_isExploding)
            {
                UpdateExplosion(deltaTime, gameObjects);
                return;
            }

            Position += Velocity * deltaTime;
            ApplyGravity(deltaTime);

            // Check enemy collision
            foreach (var enemy in gameObjects.OfType<BaseEnemy>())
            {
                if(IsTouching(enemy))
                {
                    OnProjectileHit(enemy);
                    enemy.OnHitByProjectile(this, DamageAmount);
                    StartExplosion();
                    return;
                }
            }

            // Check collision with other projectiles
            foreach (var bullet in gameObjects.OfType<Projectile>())
            {
                if(bullet != this && IsTouching(bullet))
                {
                    if(bullet is not PlayerBullet)
                    {
                        StartExplosion();
                        return;
                    }
                } 
            }

            // Check collision with tiles
            if(CanCollideTile)
            {
                int radius = 5;
                for (int i = -radius; i <= radius; i++)
                {
                    for (int j = -radius; j <= radius; j++)
                    {
                        Vector2 newPosition = new(Position.X + i * Singleton.TILE_SIZE, Position.Y + j * Singleton.TILE_SIZE);
                        Tile tile = tileMap.GetTileAtWorldPostion(newPosition);
                        if(tile != null && tile.IsSolid)
                        {
                            if (IsTouching(tile))
                            {
                                StartExplosion();
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void StartExplosion()
        {
            _isExploding = true;
            _explosionTimer = 0f;
            _explosionAlpha = 1.0f;
            _explosionDamage = DamageAmount * 0.8f; // Explosion does 80% of direct hit damage
            Velocity = Vector2.Zero; // Stop moving when exploding
            _damagedObjects.Clear(); // Reset the list of damaged objects
            
            // TODO: Play explosion sound here
            // Singleton.Instance.SoundManager.PlaySound("explosion");
            
            // Shake the camera
            Singleton.Instance.Camera.ShakeScreen(0.25f, _explosionDuration / 2);
        }

        private void UpdateExplosion(float deltaTime, List<GameObject> gameObjects)
        {
            _explosionTimer += deltaTime;
            _explosionAlpha = MathHelper.Lerp(1.0f, 0.0f, _explosionTimer / _explosionDuration);
            
            // Apply explosion damage to objects within radius
            if (_explosionTimer < _explosionDuration / 3) // Only apply damage at the start of explosion
            {
                // Handle damage to enemies
                foreach (var enemy in gameObjects.OfType<BaseEnemy>())
                {
                    if (!_damagedObjects.Contains(enemy) && IsInExplosionRadius(enemy))
                    {
                        enemy.OnHitByProjectile(this, _explosionDamage);
                        _damagedObjects.Add(enemy);
                    }
                }
                
                // Handle damage to player
                if (!_damagedObjects.Contains(Singleton.Instance.Player) && 
                    IsInExplosionRadius(Singleton.Instance.Player))
                {
                    Singleton.Instance.Player.OnHitByProjectile(this, _explosionDamage);
                    _damagedObjects.Add(Singleton.Instance.Player);
                }
            }
            
            // End explosion
            if (_explosionTimer >= _explosionDuration)
            {
                IsActive = false;
            }
        }

        private bool IsInExplosionRadius(GameObject gameObject)
        {
            // Calculate center points
            Vector2 explosionCenter = new Vector2(
                Position.X + Viewport.Width / 2, 
                Position.Y + Viewport.Height / 2
            );
            
            Vector2 objectCenter = new Vector2(
                gameObject.Position.X + gameObject.Viewport.Width / 2,
                gameObject.Position.Y + gameObject.Viewport.Height / 2
            );
            
            // Calculate distance between centers
            float distance = Vector2.Distance(explosionCenter, objectCenter);
            
            // Check if object is within explosion radius
            return distance <= _explosionRadius;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_isExploding)
            {
                DrawExplosion(spriteBatch);
            }
            else
            {
                base.Draw(spriteBatch);
            }
        }

        private void DrawExplosion(SpriteBatch spriteBatch)
        {
            float scale = (_explosionRadius * 2) / _explosionTexture.Width;
            Vector2 origin = new Vector2(_explosionTexture.Width / 2, _explosionTexture.Height / 2);
            Vector2 center = new Vector2(Position.X + Viewport.Width / 2, Position.Y + Viewport.Height / 2);
            
            spriteBatch.Draw(
                _explosionTexture, 
                center, 
                null, 
                Color.White * _explosionAlpha, 
                0f, 
                origin, 
                scale, 
                SpriteEffects.None, 
                0f
            );
        }
    }
} 