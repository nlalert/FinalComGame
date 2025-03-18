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
        public Keys Left, Right, Fire, Jump, Crouch, Climb;

        private float jumpStrength = 850f;
        public int Speed;

        public int walkSpeed;
        public int crouchSpeed;
        public int climbSpeed;

        private bool isJumping = false;
        private bool isClimbing = false;

        private string overlappedTile = "";

        private int direction = 1; // 1 = Right, -1 = Left

        // Constants
        private float coyoteTime = 0.1f; // 100ms of coyote time
        private float jumpBufferTime = 0.15f; // 150ms jump buffer

        // Timers
        private float coyoteTimeCounter = 0f;
        private float jumpBufferCounter = 0f;

        private Animation _idleAnimation;
        private Animation _runAnimation;
        private Animation _jumpAnimation;
        private Animation _fallAnimation;

        public Player(Texture2D idleTexture, Texture2D runTexture, Texture2D jumpTexture, Texture2D fallTexture) : base(idleTexture)
        {
            _idleAnimation = new Animation(idleTexture, 48, 64, 16, 24); // 24 fps
            _runAnimation = new Animation(runTexture, 48, 64, 8, 24); //  24 fps
            _jumpAnimation = new Animation(jumpTexture, 48, 64, 4, 24); //  24 fps
            _fallAnimation = new Animation(fallTexture, 48, 64, 4, 24); //  24 fps

            Animation = _idleAnimation;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffect = direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 _spritePosition = new Vector2(Position.X - (Animation.GetFrameSize().X - Viewport.Width) / 2 
            , Position.Y - (Animation.GetFrameSize().Y - Viewport.Height) / 2);

            spriteBatch.Draw(
                Animation.GetTexture(),
                _spritePosition,
                Animation.GetCurrentFrame(),
                Color.White,
                0f, 
                Vector2.Zero,
                1f,
                spriteEffect, 
                0f
            );

            base.Draw(spriteBatch);
        }

        public override void Reset()
        {
            Position = new Vector2(Singleton.SCREEN_WIDTH/2, Singleton.SCREEN_HEIGHT/8);
            direction = 1; // Reset direction to right
            walkSpeed = Speed;
            crouchSpeed = Speed/2;
            climbSpeed = Speed/2;
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
            HandleInput(deltaTime, gameObjects);
            UpdateCoyoteTime(deltaTime);
            CheckAndJump();

            if (!isClimbing) ApplyGravity(deltaTime);

            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            UpdateAnimation(deltaTime);

            // Keep player within screen bounds for now 

            Velocity.X = 0; // Reset horizontal velocity each frame

            base.Update(gameTime, gameObjects, tileMap);
        }

        private void UpdateAnimation(float deltaTime)
        {
            if (Velocity.Y < 0) 
                Animation = _jumpAnimation;
            else if (Velocity.Y > 0)
                Animation = _fallAnimation;
            else if (Velocity.X != 0)
                Animation = _runAnimation;
            else // Not moving
                Animation = _idleAnimation;

            Animation?.Update(deltaTime);
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

            // Jump Buffer: Store jump input for a short period
            if (Singleton.Instance.IsKeyJustPressed(Jump))
                jumpBufferCounter = jumpBufferTime; // Store jump input
            else
                jumpBufferCounter -= deltaTime; // Decrease over time

            if (Singleton.Instance.IsKeyPressed(Crouch) && !isJumping && !isClimbing)
            {
                Viewport.Height = 16;
                Speed = crouchSpeed;
            }
            else
            {
                Viewport.Height = 32;
                Speed = walkSpeed;
            }

            if (Singleton.Instance.IsKeyPressed(Climb) && overlappedTile == "Ladder" && !isClimbing)
            {
                isClimbing = true;
                isJumping = false;
                Velocity.Y = 0;
            }

            if (isClimbing)
            {
                if (Singleton.Instance.IsKeyPressed(Climb))
                {
                    Velocity.Y = -climbSpeed;
                }

                else if (Singleton.Instance.IsKeyPressed(Crouch))
                {
                    Velocity.Y = climbSpeed;
                }

                else Velocity.Y = 0;
                
                if (Singleton.Instance.IsKeyJustPressed(Jump) || overlappedTile == "")
                {
                    isClimbing = false;
                }

            }

            if (Singleton.Instance.IsKeyJustPressed(Fire))
                Shoot(gameObjects);

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

        private void UpdateHorizontalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position.X += Velocity.X * deltaTime;
            overlappedTile = "";
            foreach (Tile tile in tileMap.tiles)
            {

                if (tile.IsSolid)
                {
                    ResolveHorizontalCollision(tile);
                }

                if (tile.Type == "Ladder")
                {
                    if (IsTouchingRight(tile) || IsTouchingLeft(tile)) {
                        overlappedTile = tile.Type;
                    }
                }

            }
        }

        private void UpdateVerticalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position.Y += Velocity.Y * deltaTime;
            overlappedTile = "";
            foreach (Tile tile in tileMap.tiles)
            {
                if (tile.IsSolid)
                {
                    ResolveVerticalCollision(tile);
                }

                if (tile.Type == "Ladder")
                {
                    if (IsTouchingTop(tile) || IsTouchingBottom(tile)) {
                        overlappedTile = tile.Type;
                    }
                }

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