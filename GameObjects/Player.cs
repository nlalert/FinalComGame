using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    public class Player : Character
    {
        public Bullet Bullet;
        public Keys Left, Right, Fire, Jump, Attack, Dash, Crouch, Climb;

        public Particle _particle;

        public int crouchSpeed;
        public int climbSpeed;

        private bool isClimbing = false;
        private bool isCrouching = false;
        private bool isDropping = false;

        private string overlappedTile = "";
        //Jump
        protected float coyoteTime = 0.1f; // 100ms of coyote time
        protected float coyoteTimeCounter = 0f;
        protected float jumpBufferTime = 0.15f; // 150ms jump buffer
        protected float jumpBufferCounter = 0f;
        // Dash 
        private bool isDashing = false;
        private float dashSpeed = 600f;
        private float dashDuration = 0.2f; // Dash lasts for 0.2 seconds
        private float dashCooldown = 0.5f; // Cooldown before dashing again
        private float dashTimer = 0f;
        private float dashCooldownTimer = 0f;

        //Animation
        private Animation _meleeAttackAnimation;
        private Animation _jumpAnimation;
        private Animation _dashAnimation;
        private Animation _glideAnimation;
        private Animation _fallAnimation;
        public Player(Texture2D idleTexture, Texture2D runTexture, Texture2D meleeAttackTexture, Texture2D jumpTexture, Texture2D fallTexture, Texture2D dashTexture, Texture2D glideTexture, Texture2D paticleTexture)
        {
            _idleAnimation = new Animation(idleTexture, 48, 64, 16, 24); // 24 fps
            _runAnimation = new Animation(runTexture, 48, 64, 8, 24); //  24 fps
            _jumpAnimation = new Animation(jumpTexture, 48, 64, 4, 24); //  24 fps
            _fallAnimation = new Animation(fallTexture, 48, 64, 4, 24); //  24 fps
            _meleeAttackAnimation = new Animation(meleeAttackTexture, 48, 64, 8, 24); // 24 fps
            _dashAnimation = new Animation(dashTexture, 16, 32, 16, 24); //  24 fps
            _glideAnimation = new Animation(glideTexture, 16, 32, 16, 24); //  24 fps

            _particle = new Particle(10, Position, paticleTexture);

            Animation = _idleAnimation;
        }

        public override void Reset()
        {
            Position = new Vector2(Singleton.SCREEN_WIDTH/2, Singleton.SCREEN_HEIGHT/8);
            direction = 1; // Reset direction to right
            crouchSpeed = WalkSpeed/2;
            climbSpeed = WalkSpeed/2;
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
            HandleInput(deltaTime, gameObjects);
            UpdateInvincibilityTimer(deltaTime);
            UpdateCoyoteTime(deltaTime);
            CheckAndJump();

            if (!isClimbing && !isDashing) 
                ApplyGravity(deltaTime);
                
            UpdateDash(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            UpdateTileInteraction(tileMap);
            UpdateAttackHitbox();
            CheckAttackHit(gameObjects);
            UpdateAnimation(deltaTime);

            if (!isDashing) Velocity.X = 0;

            base.Update(gameTime, gameObjects, tileMap);
            _particle.Update(Position);    

            //Console.WriteLine(Position);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _particle.Draw(spriteBatch);
            base.Draw(spriteBatch);
            DrawDebug(spriteBatch);
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            if(isAttacking)
                Animation = _meleeAttackAnimation;
            else if (isDashing)
                Animation = _dashAnimation;
            else if (Velocity.Y > 0)
                Animation = _fallAnimation;
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

            if (Singleton.Instance.IsKeyJustPressed(Jump) && !Singleton.Instance.IsKeyPressed(Crouch) && !isDashing)
                jumpBufferCounter = jumpBufferTime;
            else
                jumpBufferCounter -= deltaTime; // Decrease over time

            if (Singleton.Instance.IsKeyPressed(Crouch) && !isJumping && !isClimbing && Velocity.Y == 0)
            {
                if (!isCrouching) Position.Y += 16;
                Viewport.Height = 16;
                WalkSpeed = crouchSpeed;
                isCrouching = true;
            }
            else
            {
                if (isCrouching) Position.Y -= 16;
                Viewport.Height = 32;
                WalkSpeed = 200;
                isCrouching = false;
            }

            if (Singleton.Instance.IsKeyPressed(Crouch) && Singleton.Instance.IsKeyJustPressed(Jump)){
                isDropping = true;
            }
            else
            {
                isDropping = false;
            }

            if ((Singleton.Instance.IsKeyPressed(Climb) || Singleton.Instance.IsKeyPressed(Crouch)) && overlappedTile == "Ladder" && !isClimbing && !isCrouching)
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

            if (Singleton.Instance.IsKeyJustPressed(Dash))
                StartDash();
        }

        private void StartAttack()
        {
            if (attackCooldownTimer <= 0 && !isAttacking)
            {
                _meleeAttackAnimation.Reset();
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
                Velocity.Y = 0;
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

        protected void UpdateTileInteraction (TileMap tileMap){

            overlappedTile = "";
            foreach (Tile tile in tileMap.tiles)
            {
                if (tile.Type == "Ladder" || tile.Type == "Platform_Ladder")
                {
                    if (IsOverlapped(tile)){
                        overlappedTile = "Ladder";
                    }
                }

                if (tile.Type == "Platform" || tile.Type == "Platform_Ladder")
                {
                    if (tile.Position.Y < Position.Y + Viewport.Height || isDropping){
                        tile.IsSolid = false;
                    }

                    else{
                        tile.IsSolid = true;
                    }

                }

                if (tile.IsSolid)
                }

            }
        }

        protected override void UpdateVerticalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Vector2 nextPosition = Position + new Vector2(0, Velocity.Y * deltaTime);
            float distance = Vector2.Distance(Position, nextPosition);
            
            if (distance == 0) return; // No movement, skip checking

            float stepSize = distance / 8; // Divide movement into 8 steps
            int tileX = (int)(Position.X / Singleton.BLOCK_SIZE); // Get current column

            overlappedTile = ""; // Reset tile detection

            for (int i = 1; i <= 8; i++) // Start from 1 to avoid checking current position
            {
                float checkY = Position.Y + Math.Sign(Velocity.Y) * (stepSize * i);
                int tileY = (int)(checkY / Singleton.BLOCK_SIZE) + this.Rectangle.Height/Singleton.BLOCK_SIZE; // Convert to tile grid position

                // Get the tile at (tileX, tileY)
                Tile tile = tileMap.GetTileAt(tileX, tileY);
                if (tile != null)
                {
                    if (IsTouchingTop(tile) && !Singleton.Instance.IsKeyPressed(Climb))
                    {
                        isClimbing = false;
                    // Check collision with solid tiles
                    if (tile.IsSolid)
                    {
                        Velocity.Y = 0; // Stop falling
                        return; // Stop checking further
                    }

                    // Detect special tiles like "Ladder"
                    if (tile.Type == "Ladder" && (IsTouchingTop(tile) || IsTouchingBottom(tile)))
                    {
                        overlappedTile = tile.Type;
                    }
                }
            }

            Position.Y = nextPosition.Y; // Apply movement if no obstacle
        }

        private bool IsOverlapped(Tile tile){
            return IsTouchingRight(tile) || IsTouchingLeft(tile) || IsTouchingTop(tile) || IsTouchingBottom(tile);
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


        public override void OnHitByProjectile(GameObject gameObject,float damageAmount)
        {
            OnHit(damageAmount);
        }
        public override void OnHit(float damageAmount)
        {
            if (invincibilityTimer > 0) 
                return; // If i-frames are active, ignore damage
            // Generic hit handling
            Health -= damageAmount;
            StartInvincibility();
            Console.WriteLine("Damage " + damageAmount + "CurHP" + Health);
            if (Health <= 0)
            {
                OnDead();
            }
        }
        /// <summary>
        /// Call this when player get physical contact with npc
        /// eg. add knockback
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="damageAmount"></param>
        public override void OnCollideNPC(Character npc, float damageAmount)
        {
            OnHit(damageAmount);
            //player.takeKnockback(npc.knockback);
            base.OnCollideNPC(npc, damageAmount);
        }
        private void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 40);
            string directionText = direction != 1 ? "Left" : "Right";
            string displayText = $"Dir {directionText} \nCHp {Health}";
            spriteBatch.DrawString(Singleton.Instance.Debug_Font, displayText, textPosition, Color.White);
        }
    }
}