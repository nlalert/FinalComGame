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
        public Keys Left, Right, Fire, Jump, Attack, Dash;

        public int Speed;
        private int direction = 1; // 1 = Right, -1 = Left
        
        //Attack
        private bool isAttacking = false;
        private float attackDuration = 0.2f; // How long the attack lasts
        private float attackCooldown = 0.5f; // Cooldown before attacking again
        private float attackTimer = 0f;
        private float attackCooldownTimer = 0f;
        private Rectangle attackHitbox;

        //Jump
        private bool isJumping = false;
        private float jumpStrength = 1000f;
        private float coyoteTime = 0.1f; // 100ms of coyote time
        private float coyoteTimeCounter = 0f;
        private float jumpBufferTime = 0.15f; // 150ms jump buffer
        private float jumpBufferCounter = 0f;

        // Dash 
        private bool isDashing = false;
        private float dashSpeed = 800f;
        private float dashDuration = 0.2f; // Dash lasts for 0.2 seconds
        private float dashCooldown = 0.5f; // Cooldown before dashing again
        private float dashTimer = 0f;
        private float dashCooldownTimer = 0f;

        //Animation
        private Animation _idleAnimation;
        private Animation _runAnimation;
        private Animation _jumpAnimation;
        private Animation _dashAnimation;
        private Animation _glideAnimation;

        public Player(Texture2D idleTexture, Texture2D runTexture, Texture2D jumpTexture, Texture2D dashTexture, Texture2D glideTexture) : base(idleTexture)
        {
            _idleAnimation = new Animation(idleTexture, 16, 32, 16, 24); // 24 fps
            _runAnimation = new Animation(runTexture, 16, 32, 16, 24); //  24 fps
            _jumpAnimation = new Animation(jumpTexture, 16, 32, 16, 24); //  24 fps
            _dashAnimation = new Animation(dashTexture, 16, 32, 16, 24); //  24 fps
            _glideAnimation = new Animation(glideTexture, 16, 32, 16, 24); //  24 fps

            Animation = _idleAnimation;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffect = direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(
                Animation.GetTexture(),
                Position,
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
            Position = new Vector2(Singleton.SCREEN_WIDTH/2, Singleton.SCREEN_HEIGHT/2);
            direction = 1; // Reset direction to right
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
            HandleInput(deltaTime, gameObjects);
            UpdateCoyoteTime(deltaTime);
            CheckAndJump();
            if (!isDashing) 
                ApplyGravity(deltaTime);
            UpdateDash(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            UpdateAnimation(deltaTime);

            if (!isDashing) Velocity.X = 0;

            base.Update(gameTime, gameObjects, tileMap);
        }

        private void UpdateAnimation(float deltaTime)
        {
            if (isDashing)
                Animation = _dashAnimation;
            else if (isJumping || Velocity.Y != 0)
                Animation = _jumpAnimation;
            else if (Velocity.X != 0)
                Animation = _runAnimation;
            else
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
                if (!isDashing) 
                {
                    direction = -1;
                    Velocity.X = -Speed;
                }
            }
            if (Singleton.Instance.IsKeyPressed(Right))
            {
                if (!isDashing) 
                {
                    direction = 1;
                    Velocity.X = Speed;
                }
            }

            if (Singleton.Instance.IsKeyJustPressed(Attack))
                StartAttack();

            if (isAttacking)
            {
                attackTimer -= deltaTime;
                if (attackTimer <= 0)
                    isAttacking = false;
            }
            else
            {
                attackCooldownTimer -= deltaTime;
            }
            
            if (Singleton.Instance.IsKeyJustPressed(Fire))
                Shoot(gameObjects);

            if (Singleton.Instance.IsKeyJustPressed(Jump))
                jumpBufferCounter = jumpBufferTime;
            else
                jumpBufferCounter -= deltaTime;

            if (Singleton.Instance.IsKeyJustPressed(Dash))
                StartDash();
        }

        private void StartAttack()
        {
            if (attackCooldownTimer <= 0 && !isAttacking)
            {
                isAttacking = true;
                attackTimer = attackDuration;
                attackCooldownTimer = attackCooldown;

                // Set attack hitbox in front of the player
                int attackWidth = 20; // Adjust the size of the attack area
                int attackHeight = 32;
                int offsetX = direction == 1 ? Rectangle.Width : -attackWidth;

                attackHitbox = new Rectangle((int)Position.X + offsetX, (int)Position.Y, attackWidth, attackHeight);

                // TODO: Detect enemies within this hitbox
            }
        }

        private void StartDash()
        {
            if (dashCooldownTimer <= 0 && !isDashing)
            {
                isDashing = true;
                dashTimer = dashDuration;
                dashCooldownTimer = dashCooldown;
                Velocity.X = dashSpeed * direction;
            }
        }

        private void UpdateDash(float deltaTime)
        {
            if (isDashing)
            {
                dashTimer -= deltaTime;
                if (dashTimer <= 0)
                {
                    isDashing = false;
                    Velocity.X = 0;
                }
            }
            else
            {
                dashCooldownTimer -= deltaTime;
            }
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
            foreach (Tile tile in tileMap.tiles)
            {
                ResolveHorizontalCollision(tile);
            }
        }

        private void UpdateVerticalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position.Y += Velocity.Y * deltaTime;
            foreach (Tile tile in tileMap.tiles)
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
            //TODO apex of jump is grounded?
            return Velocity.Y == 0;
        }
    }
}