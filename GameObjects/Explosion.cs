using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Explosion : GameObject
    {
        
        public SoundEffect ExplosionSound;
        public float Duration; // How long the explosion lasts in seconds
        public float Damage; // Damage dealt by explosion
        private float _alpha;
        private float _timer;
        private List<GameObject> _damagedObjects; // Track already damaged objects

        public Explosion(Texture2D texture, SoundEffect explosionSound) : base(texture)
        {
            _damagedObjects = new List<GameObject>();

            _timer = 0f;
            _alpha = 1.0f;
            Duration = 0.5f;

            ExplosionSound = explosionSound;
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateExplosion(deltaTime, gameObjects);
        }

        public void TriggerExplosion()
        {
            _damagedObjects.Clear(); // Reset the list of damaged objects

            ExplosionSound.Play();
            
            // Shake the camera
            Singleton.Instance.Camera.ShakeScreen(0.25f, Duration / 2);
        }

        private void UpdateExplosion(float deltaTime, List<GameObject> gameObjects)
        {
            _timer += deltaTime;
            _alpha = MathHelper.Lerp(1.0f, 0.0f, _timer / Duration);
            
            // Apply explosion damage to objects within radius
            if (_timer < Duration / 3) // Only apply damage at the start of explosion
            {
                // Handle damage to enemies
                foreach (var enemy in gameObjects.OfType<BaseEnemy>())
                {
                    if (!_damagedObjects.Contains(enemy) && IsInExplosionRadius(enemy))
                    {
                        enemy.OnHitByProjectile(this, Damage);
                        _damagedObjects.Add(enemy);
                    }
                }
                
                // Handle damage to player
                if (!_damagedObjects.Contains(Singleton.Instance.Player) && 
                    IsInExplosionRadius(Singleton.Instance.Player))
                {
                    Singleton.Instance.Player.OnHitByProjectile(this, Damage);
                    _damagedObjects.Add(Singleton.Instance.Player);
                }
            }
            
            // End explosion
            if (_timer >= Duration)
            {
                IsActive = false;
            }
        }

        private bool IsInExplosionRadius(GameObject g)
        {
            // Calculate center points
            Vector2 explosionCenter = new Vector2(
                Position.X + Viewport.Width / 2, 
                Position.Y + Viewport.Height / 2
            );
            
            Vector2 objectCenter = new Vector2(
                g.Position.X + g.Viewport.Width / 2,
                g.Position.Y + g.Viewport.Height / 2
            );
            
            // Calculate distance between centers
            float distance = Vector2.Distance(explosionCenter, objectCenter);
            
            // Check if object is within explosion radius
            return distance <= Radius;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float scale = (Radius * 2) / _texture.Width;
            Vector2 origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
            Vector2 center = new Vector2(Position.X + Viewport.Width / 2, Position.Y + Viewport.Height / 2);
            
            spriteBatch.Draw(
                _texture, 
                center, 
                null, 
                Color.White * _alpha, 
                0f, 
                origin, 
                scale, 
                SpriteEffects.None, 
                0f
            );
        }
    }
} 