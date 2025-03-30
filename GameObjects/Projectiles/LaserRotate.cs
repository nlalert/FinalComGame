using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FinalComGame
{
    public class DemonLaser : Projectile
    {
        private float _length;
        private float _angle;
        private Vector2 _startPosition;
        private Vector2 _endPosition;

        public DemonLaser(Texture2D texture, float length = 100f) : base(texture, damage: 20f, speed: 0f)
        {
            _length = length;
        }

        public void Activate(Vector2 startPosition, float angle)
        {
            _startPosition = startPosition;
            _angle = angle;
            UpdateLaserPositions();
        }

        private void UpdateLaserPositions()
        {
            _endPosition = _startPosition + new Vector2(
                _length * (float)Math.Cos(_angle),
                _length * (float)Math.Sin(_angle)
            );
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            // Keep updating laser position in case the enemy moves
            _startPosition = Position; // Attach to enemy
            UpdateLaserPositions();

            // Check collision with the player along the laser line
            if (LineIntersectsPlayer(_startPosition, _endPosition))
            {
                Singleton.Instance.Player.OnHitByProjectile(this, DamageAmount);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawLaser(spriteBatch, _startPosition, _endPosition);
            // base.Draw(spriteBatch);
        }

        private void DrawLaser(SpriteBatch spriteBatch, Vector2 start, Vector2 end)
        {
            Vector2 direction = end - start;
            float rotation = (float)Math.Atan2(direction.Y, direction.X);
            float laserWidth = 10f; // Adjust width as needed
            float laserLength = direction.Length();

            spriteBatch.Draw(_texture, start, null, Color.Red, rotation, Vector2.Zero,
                             new Vector2(laserLength / _texture.Width, laserWidth / _texture.Height),
                             SpriteEffects.None, 0);
        }

        private bool LineIntersectsPlayer(Vector2 start, Vector2 end)
        {
            Rectangle playerBounds = Singleton.Instance.Player.Rectangle;

            return playerBounds.Contains(start) || playerBounds.Contains(end);
        }
    }
}
