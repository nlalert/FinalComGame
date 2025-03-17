using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    class Player : Character
    {
        public Bullet Bullet;
        public Keys Left, Right, Fire, Jump, Attack, Dash;
        
        //Jump
        protected float coyoteTime = 0.1f; // 100ms of coyote time
        protected float coyoteTimeCounter = 0f;
        protected float jumpBufferTime = 0.15f; // 150ms jump buffer
        protected float jumpBufferCounter = 0f;
        // Dash 
        private bool isDashing = false;
        private float dashSpeed = 800f;
        private float dashDuration = 0.2f; // Dash lasts for 0.2 seconds
        private float dashCooldown = 0.5f; // Cooldown before dashing again
        private float dashTimer = 0f;
        private float dashCooldownTimer = 0f;

        //Animation
        private Animation _meleeAttackAnimation;
        private Animation _jumpAnimation;
        private Animation _dashAnimation;
        private Animation _glideAnimation;

        public Player(Texture2D idleTexture, Texture2D runTexture, Texture2D meleeAttackTexture, Texture2D jumpTexture, Texture2D dashTexture, Texture2D glideTexture)
        {
            _idleAnimation = new Animation(idleTexture, 16, 32, 16, 24); // 24 fps
            _runAnimation = new Animation(runTexture, 16, 32, 16, 24); //  24 fps
            _meleeAttackAnimation = new Animation(meleeAttackTexture, 16, 32, 16, 24); // 24 fps
            _jumpAnimation = new Animation(jumpTexture, 16, 32, 16, 24); //  24 fps
            _dashAnimation = new Animation(dashTexture, 16, 32, 16, 24); //  24 fps
            _glideAnimation = new Animation(glideTexture, 16, 32, 16, 24); //  24 fps

            Animation = _idleAnimation;
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
            UpdateAttackHitbox();
            CheckAttackHit(gameObjects);
            UpdateAnimation(deltaTime);
            if (!isDashing) Velocity.X = 0;

            base.Update(gameTime, gameObjects, tileMap);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            if(isAttacking)
                Animation = _meleeAttackAnimation;
            else if (isDashing)
                Animation = _dashAnimation;
            else if (isJumping || Velocity.Y != 0)
                Animation = _jumpAnimation;
            else if (Velocity.X != 0)
                Animation = _runAnimation;
            else
                Animation = _idleAnimation;

            base.UpdateAnimation(deltaTime);
        }

        private void HandleInput(float deltaTime, List<GameObject> gameObjects)
        {
            if (Singleton.Instance.IsKeyPressed(Left))
            {
                if (!isDashing) 
                {
                    direction = -1;
                    Velocity.X = -WalkSpeed;
                }
            }
            if (Singleton.Instance.IsKeyPressed(Right))
            {
                if (!isDashing) 
                {
                    direction = 1;
                    Velocity.X = WalkSpeed;
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

        private void UpdateAttackHitbox()
        {
            if (isAttacking)
            {
                int attackWidth = 20; // Adjust as needed
                int attackHeight = 32;
                int offsetX = direction == 1 ? Rectangle.Width : -attackWidth;

                attackHitbox = new Rectangle((int)Position.X + offsetX, (int)Position.Y, attackWidth, attackHeight);
            }
        }
        
        private void CheckAttackHit(List<GameObject> gameObjects)
        {
            if (!isAttacking) return;

            foreach (var enemy in gameObjects.OfType<BaseEnemy>())
            {
                enemy.CheckHit(attackHitbox, attackDamage);
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
    }
}