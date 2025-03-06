using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    class Player : GameObject
    {
        public Bullet Bullet;
        public Keys Left, Right, Fire, Jump;

        private float jumpStrength = 1250f;
        public int Speed;

        private int direction = 1; // 1 = Right, -1 = Left

        // Constants
        private float coyoteTime = 0.1f; // 100ms of coyote time
        private float jumpBufferTime = 0.15f; // 150ms jump buffer

        // Timers
        private float coyoteTimeCounter = 0f;
        private float jumpBufferCounter = 0f;

        public Player(Texture2D texture) : base(texture)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Viewport, Color.White);
            base.Draw(spriteBatch);
        }

        public override void Reset()
        {
            Position = new Vector2(62, 640);
            direction = 1; // Reset direction to right
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Singleton.Instance.IsKeyPressed(Left))
            {
                Velocity.X = -Speed;
                direction = -1;
            }
            if (Singleton.Instance.IsKeyPressed(Right))
            {
                Velocity.X = Speed;
                direction = 1;
            }

            if (Singleton.Instance.IsKeyJustPressed(Fire))
            {
                var newBullet = Bullet.Clone() as Bullet;
                newBullet.Position = new Vector2(Rectangle.Width / 2 + Position.X - newBullet.Rectangle.Width / 2,
                                                Position.Y);
                newBullet.Velocity = new Vector2(800 * direction, 0);
                newBullet.Reset();
                gameObjects.Add(newBullet);
            }

            // Jump Buffer: Store jump input for a short period
            if (Singleton.Instance.IsKeyJustPressed(Jump))
            {
                jumpBufferCounter = jumpBufferTime; // Store jump input
            }
            else if(Velocity.Y != 0)
            {
                jumpBufferCounter -= deltaTime; // Decrease over time
            }

            // Apply coyote time: Reset if on ground
            if (Velocity.Y == 0)
            {
                coyoteTimeCounter = coyoteTime; // Reset coyote time when on ground
            }
            else
            {
                coyoteTimeCounter -= deltaTime; // Decrease coyote time when falling
            }

            // Jumping logic with Coyote Time and Jump Buffer
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                Velocity.Y = -jumpStrength;
                jumpBufferCounter = 0; // Prevent multiple jumps
                coyoteTimeCounter = 0; // Consume coyote time
            }

            Position.X += Velocity.X * deltaTime;
            foreach (var tile in gameObjects.OfType<Tile>())
            {
                if (IsTouchingLeft(tile) || IsTouchingRight(tile))
                {
                    if (Velocity.X > 0) // Moving right
                    {
                        Position.X = tile.Rectangle.Left - Rectangle.Width;
                    }
                    else if (Velocity.X < 0) // Moving left
                    {
                        Position.X = tile.Rectangle.Right;
                    }
                    Velocity.X = 0;
                }
            }

            // Apply gravity
            Velocity.Y += Singleton.GRAVITY * deltaTime;

            Position.Y += Velocity.Y * deltaTime;
            foreach (var tile in gameObjects.OfType<Tile>())
            {
                if (IsTouchingTop(tile) || IsTouchingBottom(tile))
                {
                    if (Velocity.Y > 0) // Falling down
                    {
                        Position.Y = tile.Rectangle.Top - Rectangle.Height;
                    }
                    else if (Velocity.Y < 0) // Moving up
                    {
                        Position.Y = tile.Rectangle.Bottom;
                    }
                    Velocity.Y = 0;
                }
            }

            // Keep player within screen bounds
            Position.X = MathHelper.Clamp(Position.X, 0, Singleton.SCREEN_WIDTH - Rectangle.Width);
            
            Velocity.X = 0; // Reset horizontal velocity each frame

            base.Update(gameTime, gameObjects);
        }

    }
}