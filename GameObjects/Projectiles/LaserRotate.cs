using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace FinalComGame
{
    public class DemonLaser : Projectile
    {   
        private Vector2 _origin;
        private Vector2 _endPosition;
        private float _angle;
        private float _lenght;
        private float _lifeTime;

        public DemonLaser(Texture2D texture) : base(texture)
        {
            CanCollideTile =false;
            _lifeTime = 10f;
        }


        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _lifeTime -= deltaTime;
            if(_lifeTime <=0){
                IsActive=false;
            }
            _angle += 0.5f * deltaTime;
            // _endPosition = this.Position + (new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle)) * _lenght);
            // _endPosition = this.Position + (new Vector2((float)Math.Cos(_angle - MathHelper.PiOver2), (float)Math.Sin(_angle - MathHelper.PiOver2)) * _lenght);
            _endPosition = this.Position + (new Vector2((float)Math.Cos(_angle + MathHelper.PiOver2), (float)Math.Sin(_angle + MathHelper.PiOver2)) * _lenght);
            if (LineIntersectsPlayer(Position, _endPosition))
            {
                Console.WriteLine("HIT PLAYER");
                OnProjectileHit(Singleton.Instance.Player);
                Singleton.Instance.Player.OnHitByProjectile(this, DamageAmount, true);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // DrawLaser(spriteBatch, _startPosition, _endPosition);
            Vector2 scale = new Vector2(1.0f, _lenght / _texture.Height);
            spriteBatch.Draw(_texture,Position,null,Color.White,_angle,_origin,scale,SpriteEffects.None,0f);
            // base.Draw(spriteBatch);
            DrawDebug(spriteBatch);
        }

        private void DrawLaser(SpriteBatch spriteBatch, Vector2 start, Vector2 end)
        {
            Vector2 direction = end - start;
            float rotation = (float)Math.Atan2(direction.Y, direction.X);
            float laserWidth = 10f; // Adjust width as needed
            float laserLength = direction.Length();

            spriteBatch.Draw(_texture, start, null, Color.White, rotation, Vector2.Zero,
                             new Vector2(laserLength / _texture.Width, laserWidth / _texture.Height),
                             SpriteEffects.None, 0);
        }

        private bool LineIntersectsPlayer(Vector2 start, Vector2 end)
        {
            Rectangle playerBounds = Singleton.Instance.Player.Rectangle;
            // Get the four edges of the rectangle as line segments
            Vector2 topLeft = new Vector2(playerBounds.Left, playerBounds.Top);
            Vector2 topRight = new Vector2(playerBounds.Right, playerBounds.Top);
            Vector2 bottomLeft = new Vector2(playerBounds.Left, playerBounds.Bottom);
            Vector2 bottomRight = new Vector2(playerBounds.Right, playerBounds.Bottom);

            // Check intersection with each edge of the rectangle
            return LineIntersectsLine(start, end, topLeft, topRight) ||
                LineIntersectsLine(start, end, topRight, bottomRight) ||
                LineIntersectsLine(start, end, bottomRight, bottomLeft) ||
                LineIntersectsLine(start, end, bottomLeft, topLeft);
        }
        private bool LineIntersectsLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            float d = (a2.X - a1.X) * (b2.Y - b1.Y) - (a2.Y - a1.Y) * (b2.X - b1.X);
            if (d == 0) return false; // Lines are parallel

            float u = ((b1.X - a1.X) * (b2.Y - b1.Y) - (b1.Y - a1.Y) * (b2.X - b1.X)) / d;
            float v = ((b1.X - a1.X) * (a2.Y - a1.Y) - (b1.Y - a1.Y) * (a2.X - a1.X)) / d;

            return (u >= 0 && u <= 1 && v >= 0 && v <= 1);
        }
        private void DrawDebug(SpriteBatch spriteBatch){
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 40);
            string displayText = $"_angle = {_angle}\nStarPos : {Position} \nEPos {_endPosition}";
            spriteBatch.DrawString(Singleton.Instance.GameFont, displayText, textPosition , Color.White);
            DrawRotatingLines(spriteBatch);
        }
        /// <summary>
        /// in direction varirable please use 
        /// X = angel of projectiles 
        /// Y = radius of projectiles aka lenght
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        public override void Shoot(Vector2 position, Vector2 direction)
        {
            _origin = Position;
            Position = position;
            //direction 
            //X = angel
            _angle = direction.X;
            //Y = radius
            _lenght = direction.Y;
            // Vector2 dir = new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle)) * _lenght;
            // _endPosition = this.Position+ dir;
            DamageAmount = base.BaseDamageAmount;
        }
        private void DrawRotatingLines(SpriteBatch spriteBatch)
        {
            // float radius = 600f; // Length of each line
            // Vector2 direction = new Vector2((float)Math.Cos(_angle), (float)Math.Sin(_angle)) * radius;
            // Vector2 endPosition = Position + direction;
            DrawLine(spriteBatch, Position, _endPosition, Color.Red);
        }
        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            // Calculate the length and direction of the line
            float length = Vector2.Distance(start, end);
            Vector2 direction = end - start;
            direction.Normalize();

            // Draw the line (scaled 1x1 texture)
            spriteBatch.Draw(Singleton.Instance.PixelTexture, start, null, color, (float)Math.Atan2(direction.Y, direction.X), Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }
    }
}
