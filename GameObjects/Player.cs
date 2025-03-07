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

        private float jumpStrength = 1000f;
        public int Speed;
        private bool isJumping = false;

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

            HandleInput(deltaTime, gameObjects);
            UpdateCoyoteTime(deltaTime);
            CheckAndJump();
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects);
            UpdateVerticalMovement(deltaTime, gameObjects);

            // Keep player within screen bounds for now 
            Position.X = MathHelper.Clamp(Position.X, 0, Singleton.SCREEN_WIDTH - Rectangle.Width);
            Velocity.X = 0; // Reset horizontal velocity each frame
            base.Update(gameTime, gameObjects);
        }

        private void ApplyGravity(float deltaTime)
        {
            Velocity.Y += Singleton.GRAVITY * deltaTime; // gravity

            //TODO: Check and cap terminal velocity of player if want later
        }

        private void HandleInput(float deltaTime, List<GameObject> gameObjects)
        {
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
                Shoot(gameObjects);

            // Jump Buffer: Store jump input for a short period
            if (Singleton.Instance.IsKeyJustPressed(Jump))
                jumpBufferCounter = jumpBufferTime; // Store jump input
            else
                jumpBufferCounter -= deltaTime; // Decrease over time
        }

        private void UpdateCoyoteTime(float deltaTime)
        {
            // Apply coyote time: Reset if on ground
            if (IsOnGround())
            {
                coyoteTimeCounter = coyoteTime; // Reset coyote time when on ground
            }
            else
            {
                coyoteTimeCounter -= deltaTime; // Decrease coyote time when falling
            }
        }

        private void CheckAndJump()
        {
            // Jumping logic with Coyote Time and Jump Buffer
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                Velocity.Y = -jumpStrength;
                jumpBufferCounter = 0; // Prevent multiple jumps
                coyoteTimeCounter = 0; // Consume coyote time
                isJumping = true;
            }

            // Jump Modulation 
            if (Singleton.Instance.IsKeyJustReleased(Jump) && isJumping)
            {
                Velocity.Y *= 0.5f; // Reduce upwards velocity to shorten jump
                isJumping = false;
            }
        }

        private void UpdateHorizontalMovement(float deltaTime, List<GameObject> gameObjects)
        {
            Position.X += Velocity.X * deltaTime;
            foreach (var tile in gameObjects.OfType<Tile>())
            {
                ResolveHorizontalCollision(tile);
            }
        }

        private void UpdateVerticalMovement(float deltaTime, List<GameObject> gameObjects)
        {
            Position.Y += Velocity.Y * deltaTime;
            foreach (var tile in gameObjects.OfType<Tile>())
            {
                ResolveVerticalCollision(tile);
            }
        }

        private void Shoot(List<GameObject> gameObjects)
        {
            var newBullet = Bullet.Clone() as Bullet;
            newBullet.Position = new Vector2(Rectangle.Width / 2 + Position.X - newBullet.Rectangle.Width / 2,
                                            Position.Y);
            newBullet.Velocity = new Vector2(800 * direction, 0);
            newBullet.Reset();
            gameObjects.Add(newBullet);
        }

        private bool IsOnGround()
        {
            return Velocity.Y == 0;
        }
    }
}