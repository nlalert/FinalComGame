using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    public class Player : Character
    {
        public PlayerBullet Bullet;
        public Keys Left, Right, Fire, Jump, Attack, Dash, Crouch, Climb, Interact, Item1, Item2;
        
        public float MaxMP;
        public float MP;
        public int Life;
        public float AbsorptionHealth;

        public Item[] HoldItem;

        public SoulParticle _particle;

        public int CrouchSpeed;
        public int ClimbSpeed;

        private bool _isClimbing;
        private bool _isCrouching;
        private bool _isDropping;
        private bool _isGliding;

        private TileType _overlappedTile;
        //Jump
        public float CoyoteTime;
        private float _coyoteTimeCounter;
        public float JumpBufferTime;
        private float _jumpBufferCounter;
        // Dash 
        private bool _isDashing;
        public float DashSpeed;
        public float DashDuration;
        public float DashCooldown;
        private float _dashTimer;
        private float _dashCooldownTimer;
        public float DashMP;
        // Glide
        public float GlideGravityScale;
        public float GlideMaxFallSpeed;        
        public float GlideMP;
        // Charge Shot properties
        private bool _isCharging;
        private float _chargeTime;
        public float MaxChargeTime;
        public float MinChargePower;
        public float MaxChargePower;
        public float ChargeMPCost;

        private bool _isFist;

        //SFX
        public SoundEffect JumpSound;

        //Animation
        // private Animation _meleeAttackAnimation;
        // private Animation _jumpAnimation;
        // private Animation _dashAnimation;
        // private Animation _glideAnimation;
        // private Animation _fallAnimation;
        // private Animation _chargeAnimation;

        public Player(Texture2D texture, Texture2D paticleTexture) : base(texture)
        {

            Animation = new Animation(texture, 48, 64, new Vector2(48*16 , 64*8), 24);

            Animation.AddAnimation("idle", new Vector2(0,0), 16);
            Animation.AddAnimation("run", new Vector2(0,1), 8);
            Animation.AddAnimation("melee", new Vector2(0,2), 8);

            Animation.AddAnimation("jump_start", new Vector2(0,3), 4);
            Animation.AddAnimation("jump_up", new Vector2(5,3), 4);
            Animation.AddAnimation("jump_forward", new Vector2(10,3), 4);

            Animation.AddAnimation("fall_start", new Vector2(0,4), 3);
            Animation.AddAnimation("fall_down", new Vector2(4,4), 4);
            Animation.AddAnimation("fall_forward", new Vector2(9,4), 4);

            Animation.AddAnimation("dash", new Vector2(0,5), 4);

            Animation.AddAnimation("crouch", new Vector2(0,6), 16);
            Animation.AddAnimation("crawl", new Vector2(0,7), 8);

            //TODO : Add sword attack animation
            Animation.AddAnimation("sword", new Vector2(0,7), 8);

            Animation.ChangeAnimation(_currentAnimation);

            paticleTexture.SetData([new Color(193, 255, 219)]);
            _particle = new SoulParticle(10, Position, paticleTexture);
        }

        public override void Reset()
        {
            HoldItem = new Item[2];

            Direction = 1; // Reset direction to right

            Health = MaxHealth - 50; //REMOVE LATER FOR DEBUG
            MP = MaxMP;
            AbsorptionHealth = 0;

            ChangeToFistAttack();

            _overlappedTile = TileType.None;

            _isFist = true;
            
            _isClimbing = false;
            _isCrouching = false;
            _isDropping = false;
            _isGliding = false;
            _isAttacking = false;
            _isCharging = false;
            _isDashing = false;
            _isJumping = false;

            _coyoteTimeCounter = 0f;
            _jumpBufferCounter = 0f;
            _dashTimer = 0f;
            _dashCooldownTimer = 0f;
            _chargeTime = 0f;
            _invincibilityTimer = 0f;
            _attackTimer = 0f;
            _attackCooldownTimer = 0f;
            
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleInput(deltaTime, gameObjects);
            RegenerateMP(deltaTime);
            ActiveItemPassiveAbility();
            UpdateInvincibilityTimer(deltaTime);
            UpdateCoyoteTime(deltaTime);
            CheckAndJump();

            if (!_isClimbing && !_isDashing) 
                ApplyGravity(deltaTime);
                
            UpdateDash(deltaTime);
            UpdateGlide();
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            UpdateTileInteraction(tileMap);
            UpdateAttackHitbox();
            CheckAttackHit(gameObjects);
            UpdateAnimation(deltaTime);
            if (!_isDashing) Velocity.X = 0;

            Console.WriteLine(AttackDamage);
            
            base.Update(gameTime, gameObjects, tileMap);
            _particle.Update(Position);    
        }

        private bool IsGameOver()
        {
            return Life <= 0;
        }

        private void RegenerateMP(float deltaTime)
        {
            if(!_isDashing && !_isGliding)
                MP += 5 * deltaTime;
            if(MP >= MaxMP) 
                MP = MaxMP;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _particle.Draw(spriteBatch);
            base.Draw(spriteBatch);
            //DrawDebug(spriteBatch);
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            string animation = "idle";

            if (_isCharging);
                //base.Animation = _chargeAnimation;
            else if(_isAttacking)
                if(_isFist)
                    animation = "melee";
                else
                    animation = "sword";
            else if (_isDashing)
                animation = "dash";
            else if (_isGliding);
                //animation = "fall_down";
            else if (Velocity.Y > 0)
                animation = "fall_start";
            else if (_isJumping && Velocity.Y < 0)
                animation = "jump_start";
            else if (Velocity.X != 0)
            {
                if(_isCrouching)
                    animation = "crawl";
                else
                    animation = "run";

                //Animation.SetFPS(Math.Abs(Velocity.X / WalkSpeed * 24));
            }
            else
            {
                if(_isCrouching)
                    animation = "crouch";
                else
                    animation = "idle";
            }

            if(_currentAnimation != animation){
                _currentAnimation = animation;
                switch (animation)
                {
                    case "jump_start" :
                        Animation.ChangeTransitionAnimation(_currentAnimation, "jump_up");
                        break;
                    case "fall_start" :
                        Animation.ChangeTransitionAnimation(_currentAnimation, "fall_down");
                        break;
                    default:
                        Animation.ChangeAnimation(_currentAnimation);
                        break;
                }    
            }

            // Console.WriteLine(Animation._currentFrameIndex);
            base.UpdateAnimation(deltaTime);
        }

        private void HandleInput(float deltaTime, List<GameObject> gameObjects)
        {
            if (Singleton.Instance.IsKeyPressed(Left))
            {
                if (!_isDashing) 
                {
                    Direction = -1;
                    Velocity.X = -WalkSpeed;
                }
            }
            if (Singleton.Instance.IsKeyPressed(Right))
            {
                if (!_isDashing) 
                {
                    Direction = 1;
                    Velocity.X = WalkSpeed;
                }
            }

            if (Singleton.Instance.IsKeyJustPressed(Attack))
                StartAttack();

            if (_isAttacking)
            {
                _attackTimer -= deltaTime;
                if (_attackTimer <= 0)
                    _isAttacking = false;
            }
            else
            {
                _attackCooldownTimer -= deltaTime;
            }

            // Handle Fire button (charge shot)
            if (Singleton.Instance.IsKeyJustPressed(Fire))
            {
                // Start charging
                StartCharging();
            }
            else if (Singleton.Instance.IsKeyPressed(Fire))
            {
                // Continue charging
                ContinueCharging(deltaTime);
            }
            else if (Singleton.Instance.IsKeyJustReleased(Fire))
            {
                // Release shot
                ReleaseChargedShot(gameObjects);
            }

            if (Singleton.Instance.IsKeyJustPressed(Jump) && !Singleton.Instance.IsKeyPressed(Crouch) && !_isDashing && _coyoteTimeCounter > 0)
                _jumpBufferCounter = JumpBufferTime;
            else
                _jumpBufferCounter -= deltaTime; // Decrease over time

            // Gliding - activate when holding Jump while in air and not climbing or dashing
            if (Singleton.Instance.IsKeyPressed(Jump) && !IsOnGround() && !_isJumping && !_isClimbing && !_isDashing && MP > 0)
            {
                _isGliding = true;
                Animation.Reset(); // Reset glide animation when starting to glide
            }
            else
            {
                _isGliding = false;
            }

            if (Singleton.Instance.IsKeyPressed(Crouch) && !_isJumping && !_isClimbing && Velocity.Y == 0)
            {
                if (!_isCrouching) Position.Y += 16;
                Viewport.Height = 16;
                WalkSpeed = CrouchSpeed;
                _isCrouching = true;
            }
            else
            {
                if (_isCrouching) Position.Y -= 16;
                Viewport.Height = 32;
                WalkSpeed = 200;
                _isCrouching = false;
            }

            if (Singleton.Instance.IsKeyPressed(Crouch) && Singleton.Instance.IsKeyJustPressed(Jump)){
                _isDropping = true;
            }
            else
            {
                _isDropping = false;
            }

            if ((Singleton.Instance.IsKeyPressed(Climb) || Singleton.Instance.IsKeyPressed(Crouch)) && _overlappedTile == TileType.Ladder && !_isClimbing && !_isCrouching && !_isDashing)
            {
                _isClimbing = true;
                _isJumping = false;
                Velocity.Y = 0;
            }

            if (_isClimbing)
            {
                if (Singleton.Instance.IsKeyPressed(Climb))
                {
                    Velocity.Y = -ClimbSpeed;
                }

                else if (Singleton.Instance.IsKeyPressed(Crouch))
                {
                    Velocity.Y = ClimbSpeed;
                }

                else Velocity.Y = 0;
                
                if (Singleton.Instance.IsKeyJustPressed(Jump) || _overlappedTile == TileType.None)
                {
                    _isClimbing = false;
                }

            }

            if (Singleton.Instance.IsKeyJustPressed(Dash))
                StartDash();
            
            if (Singleton.Instance.IsKeyJustPressed(Interact))
                CheckInteraction(gameObjects);

            if (Singleton.Instance.IsKeyJustPressed(Item1))
                UseItem(0);
            if (Singleton.Instance.IsKeyJustPressed(Item2))
                UseItem(1);
        }

        private void ActiveItemPassiveAbility()
        {
            for (int i = 0; i < 2; i++)
            {
                if (HoldItem[i] == null) continue;

                if(!HoldItem[i].IsConsumable)
                {
                    HoldItem[i].ActiveAbility();
                }
            }
        }

        public void BoostSpeed(float speedModifier)
        {
            if(_isDashing) 
                return;
            Velocity.X *= (1 + speedModifier);
        }

        private void UseItem(int itemSlotIndex)
        {
            if(HoldItem[itemSlotIndex] == null) return;

            HoldItem[itemSlotIndex].Use(itemSlotIndex);
        }

        private void CheckInteraction(List<GameObject> gameObjects)
        {
            foreach (var item in gameObjects.OfType<Item>())
            {
                if (item.InPickupRadius() && !item.IsPickedUp)
                {
                    // Check if player has empty slot
                    if (HoldItem[0] == null)
                    {
                        item.OnPickup(0);
                        break;
                        // You could add a pickup sound or effect here
                    }
                    else if (HoldItem[1] == null)
                    {
                        item.OnPickup(1);
                        break;
                        // You could add a pickup sound or effect here
                    }
                    else
                    {
                        // Both slots are full
                        Console.WriteLine("Inventory full, cannot pick up " + item.Name);
                        // Maybe show a UI message to the player
                    }
                }
            }
        }

        private void StartAttack()
        {
            if (_attackCooldownTimer <= 0 && !_isAttacking)
            {
                Animation.Reset();
                _isAttacking = true;
                _attackTimer = AttackDuration;
                _attackCooldownTimer = AttackCooldown;

                // Set attack hitbox in front of the player
                int attackWidth = 20; // Adjust the size of the attack area
                int attackHeight = 32;
                int offsetX = Direction == 1 ? Rectangle.Width : -attackWidth;

                attackHitbox = new Rectangle((int)Position.X + offsetX, (int)Position.Y, attackWidth, attackHeight);

                // TODO: Detect enemies within this hitbox
            }
        }

        private void UpdateAttackHitbox()
        {
            if (_isAttacking)
            {
                int attackWidth = 20; // Adjust as needed
                int attackHeight = 32;
                int offsetX = Direction == 1 ? Rectangle.Width : -attackWidth;

                attackHitbox = new Rectangle((int)Position.X + offsetX, (int)Position.Y, attackWidth, attackHeight);
            }
        }
        
        private void CheckAttackHit(List<GameObject> gameObjects)
        {
            if (!_isAttacking) return;

            foreach (var enemy in gameObjects.OfType<BaseEnemy>())
            {
                enemy.CheckHit(attackHitbox, AttackDamage);
            }
        }

        private void StartDash()
        {
            if (_dashCooldownTimer <= 0 && !_isDashing && MP >= DashMP)
            {
                _isDashing = true;
                _isGliding = false;
                _dashTimer = DashDuration;
                _dashCooldownTimer = DashCooldown;
                Velocity.Y = 0;
                Velocity.X = DashSpeed * Direction;
                UseMP(DashMP);
            }
        }

        private void UpdateDash(float deltaTime)
        {
            if (_isDashing)
            {
                _dashTimer -= deltaTime;
                if (_dashTimer <= 0)
                {
                    _isDashing = false;
                    Velocity.X = 0;
                }
            }
            else
            {
                _dashCooldownTimer -= deltaTime;
            }
        }

        // New method to update glide state
        private void UpdateGlide()
        {
            // Stop gliding if we hit the ground
            if (IsOnGround())
            {
                _isGliding = false;
                return;
            }
            
            // // Generate particles if gliding
            // if (isGliding)
            // {
            //     // You could add special particles here for gliding effect
            //     _particle.Update(Position);
            // }
        }

        private void UpdateCoyoteTime(float deltaTime)
        {
            // Apply coyote time: Reset if on ground
            if (IsOnGround())
            {
                _coyoteTimeCounter = CoyoteTime; // Reset coyote time when on ground
            }
            else
            {
                _coyoteTimeCounter -= deltaTime; // Decrease coyote time when falling
            }
        }

        private void CheckAndJump()
        {
            // Jumping logic with Coyote Time and Jump Buffer
            if (_jumpBufferCounter > 0 && _coyoteTimeCounter > 0)
            {
                Velocity.Y = -JumpStrength;
                _jumpBufferCounter = 0; // Prevent multiple jumps
                _coyoteTimeCounter = 0; // Consume coyote time
                _isJumping = true;
                JumpSound.Play();
            }

            // Jump Modulation 
            if (Singleton.Instance.IsKeyJustReleased(Jump) && _isJumping)
            {
                Velocity.Y *= 0.5f; // Reduce upwards velocity to shorten jump
                _isJumping = false;
            }
        }

        protected void UpdateTileInteraction (TileMap tileMap){

            _overlappedTile = TileType.None;
            int radius = 5;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    Vector2 newPosition = new(Position.X + i * Singleton.BLOCK_SIZE, Position.Y + j * Singleton.BLOCK_SIZE);
                    Tile tile = tileMap.GetTileAtWorldPostion(newPosition);
                    if(tile != null)
                    {
                        if (tile.Type == TileType.Ladder || tile.Type == TileType.Platform_Ladder)
                        {
                            if (IsTouching(tile)){
                                _overlappedTile = TileType.Ladder;
                            }
                        }

                        if (tile.Type == TileType.Platform|| tile.Type == TileType.Platform_Ladder)
                        {
                            if (tile.Position.Y < Position.Y + Viewport.Height || _isDropping){
                                tile.IsSolid = false;
                            }

                            else{
                                tile.IsSolid = true;
                            }

                        }
                    }
                }
            }
        }
       // Charge shot methods
        private void StartCharging()
        {
            if (MP > 0)
            {
                _isCharging = true;
                _chargeTime = 0f;
                // chargeColor = Color.White;
                
                // // Play charge start sound if available
                // if (ChargeSound != null)
                //     ChargeSound.Play();
                
                Animation.Reset();
            }
        }
        
        private void ContinueCharging(float deltaTime)
        {
            if (!_isCharging || MP <= 0)
                return;
                
            // Increment charge time
            _chargeTime += deltaTime;
            
            // Clamp to max charge time
            if (_chargeTime > MaxChargeTime)
                _chargeTime = MaxChargeTime;
                
            // Update charge color (from white to red as charge increases)
            // float chargeRatio = chargeTime / maxChargeTime;
            // chargeColor = new Color(
            //     1.0f, // Red always max
            //     1.0f - (chargeRatio * (1.0f - chargeMinColorG)), // Green decreases
            //     1.0f - (chargeRatio * (1.0f - chargeMinColorG)), // Blue decreases
            //     1.0f); // Alpha always max
                
            // Drain MP while charging (more MP for longer charge)
            DrainMP(ChargeMPCost / MaxChargeTime, deltaTime);
        }
        
        private void ReleaseChargedShot(List<GameObject> gameObjects)
        {
            if (!_isCharging)
                return;
                
            // Calculate charge power (linear scaling from min to max)
            float chargeRatio = Math.Min(_chargeTime / MaxChargeTime, 1.0f);
            float chargePower = MinChargePower + chargeRatio * (MaxChargePower - MinChargePower);
            
            // Create and configure the bullet
            Vector2 direction = new Vector2(Direction, 0);
            PlayerBullet newBullet = Bullet.Clone() as PlayerBullet;
            newBullet.DamageAmount *= chargePower; // Increase damage
            newBullet.Shoot(Position, direction);
            gameObjects.Add(newBullet);
            
            // // Scale bullet size with charge (optional)
            // float sizeMultiplier = 1.0f + chargeRatio;
            // newBullet.Rectangle.Width = (int)(newBullet.Rectangle.Width * sizeMultiplier);
            // newBullet.Rectangle.Height = (int)(newBullet.Rectangle.Height * sizeMultiplier);
            
            // Change color based on charge (optional)
            // newBullet.Color = chargeColor;
;
            
            // // Play charged shot sound if available
            // if (ChargeShotSound != null && chargeRatio > 0.5f)
            //     ChargeShotSound.Play();
            
            // Reset charging state
            _isCharging = false;
            _chargeTime = 0f;
        }

        public override void OnHitByProjectile(GameObject gameObject,float damageAmount)
        {
            OnHit(damageAmount);
        }
        
        public override void OnHit(float damageAmount)
        {
            if (_invincibilityTimer > 0) 
                return; // If i-frames are active, ignore damage

            // Calculate damage to absorption health
            if (AbsorptionHealth > 0)
            {
                // If absorption health can fully block the damage
                if (AbsorptionHealth >= damageAmount)
                {
                    AbsorptionHealth -= damageAmount;
                    damageAmount = 0;
                }
                // If absorption health is partially depleted
                else
                {
                    float remainingDamage = damageAmount - AbsorptionHealth;
                    AbsorptionHealth = 0;
                    Health -= remainingDamage;
                }
            }
            else
            {
                // If no absorption health, damage goes directly to HP
                Health -= damageAmount;
            }

            StartInvincibility();
            Console.WriteLine("Damage " + damageAmount + "CurHP" + Health);
            if (Health <= 0)
            {
                OnDead();
            }
        }

        public override void OnCollideNPC(Character npc, float damageAmount)
        {
            OnHit(damageAmount);
            //player.takeKnockback(npc.knockback);
            base.OnCollideNPC(npc, damageAmount);
        }

        public override void OnDead()
        {
            Life--;
            
            if(IsGameOver()) 
                Singleton.Instance.CurrentGameState = Singleton.GameState.GameOver;
            else  
                Singleton.Instance.CurrentGameState = Singleton.GameState.InitializingStage;  
        }

        private void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 40);
            string directionText = Direction != 1 ? "Left" : "Right";
            string displayText = $"Velocity {Velocity}";
            
            // Add charge info if charging
            if (_isCharging)
            {
                float chargePercent = (_chargeTime / MaxChargeTime) * 100;
                displayText += $"\nCharge {chargePercent:F0}%";
            }
            spriteBatch.DrawString(Singleton.Instance.Debug_Font, displayText, textPosition, Color.White);
            spriteBatch.DrawString(Singleton.Instance.Debug_Font, ".", this.GetPlayerCenter(), Color.Red);
        }

                // New method to apply gravity depending on glide state
        protected override void ApplyGravity(float deltaTime)
        {
            if (_isGliding)
            {
                // Apply reduced gravity while gliding
                Velocity.Y += Singleton.GRAVITY * GlideGravityScale * deltaTime;
                
                // Cap fall speed while gliding
                if (Velocity.Y > GlideMaxFallSpeed)
                    Velocity.Y = GlideMaxFallSpeed;
                
                DrainMP(GlideMP, deltaTime);
            }
            else
            {
                // Normal gravity
                base.ApplyGravity(deltaTime);
            }
        }

        private void UseMP(float MPAmount)
        {
            MP -= MPAmount;
            ClampMP();
        }

        private void DrainMP(float MPAmount, float deltaTime)
        {
            // Consume MP while gliding
            MP -= MPAmount * deltaTime;
            ClampMP();
        }

        private void ClampMP()
        {
            if (MP <= 0)
            {
                MP = 0;
                _isGliding = false; // Stop gliding when MP is depleted
            }
        }
        public Vector2 GetPlayerCenter(){
            Vector2 Center = this.Position;
            Center.X += this.Viewport.Width/2;
            Center.Y += this.Viewport.Height/4; //idk why it need /4 instead of /2
            return Center;
        }

        public void ChangeToSwordAttack(float attackDamage)
        {
            _isFist = false;
            AttackDamage = attackDamage;
        }

        public void ChangeToFistAttack()
        {
            _isFist = true;
            AttackDamage = BaseAttackDamage;
        }
    }
}