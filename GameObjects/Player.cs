using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    class Player : GameObject
    {
        public Bullet Bullet;
        public Keys Left, Right, Fire, Jump;

        private bool isJumping = false;
        private float jumpStrength = 1250f;

        private int direction = 1; // 1 = Right, -1 = Left

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

            if(Singleton.Instance.CurrentKey.IsKeyDown(Left))
            {
                Velocity.X = -500;
                direction = -1; // Facing left
            }
            if(Singleton.Instance.CurrentKey.IsKeyDown(Right))
            {
                Velocity.X = 500;
                direction = 1; // Facing right
            }

            if (Singleton.Instance.CurrentKey.IsKeyDown(Fire) &&
                Singleton.Instance.PreviousKey != Singleton.Instance.CurrentKey)
            {
                var newBullet = Bullet.Clone() as Bullet;
                newBullet.Position = new Vector2(Rectangle.Width / 2 + Position.X - newBullet.Rectangle.Width / 2,
                                                 Position.Y);
                newBullet.Velocity = new Vector2(800 * direction, 0); // Bullet moves in facing direction
                newBullet.Reset();
                gameObjects.Add(newBullet);
            }

            // Jumping logic
            if (Singleton.Instance.CurrentKey.IsKeyDown(Jump) && !isJumping)
            {
                Velocity.Y = -jumpStrength;
                isJumping = true;
            }

            // Apply gravity
            Velocity.Y += Singleton.GRAVITY * deltaTime;

            // Update position
            float newX = Position.X + Velocity.X * deltaTime;
            newX = MathHelper.Clamp(newX, 0, Singleton.SCREEN_WIDTH - Rectangle.Width);

            float newY = Position.Y + Velocity.Y * deltaTime;

            // Check if the player lands on the ground
            if (newY >= Singleton.SCREEN_HEIGHT - Rectangle.Height)
            {
                newY = Singleton.SCREEN_HEIGHT - Rectangle.Height;
                Velocity.Y = 0;
                isJumping = false; // Reset jump state
            }

            Position = new Vector2(newX, newY);

            Velocity.X = 0;

            base.Update(gameTime, gameObjects);
        }
    }
}