using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public Item[] ItemSlot;
        //slot 0 for melee item
        //slot 1 for range item
        //slot 2-3 for items
        private List<Item> usingConsumableItem;

        public SoulParticle _particle;

        public int CrouchSpeed;
        public int ClimbSpeed;

        private bool _isClimbing;
        private bool _isCrouching;
        private bool _isDropping;
        private bool _isGliding;

        //Crouch
        //private bool _isHeadHitting;
        private bool _isOnPlatform;

        //Climb
        private TileType _overlappedTile;
        private Vector2 _overlappedTilePosition;
        private Vector2 _ladderTopPosition;

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
        private float _lastChargeTime;
        public float MaxChargeTime;
        public float MinChargePower;
        public float MaxChargePower;
        public float ChargeMPCost;
        public float StartChargeMPCost;

        //MP
        public float MPRegenCooldown;
        public float MPRegenRate;
        private float _MPRegenTime;

        private bool _isFist;
        private bool _isSoulBullet;

        private bool _isUsingWeapon;

        //SFX
        public SoundEffect JumpSound;
        public SoundEffect DashSound;
        public SoundEffect PunchSound;

        //Animation
        private Animation HandAnimation;
        private string _currentHandAnimation = "idle";


        public Player(Texture2D texture, Texture2D paticleTexture) : base(texture)
        {

            Animation = new Animation(texture, 80, 64, new Vector2(80*16 , 64*18), 24);

            Animation.AddAnimation("idle", new Vector2(0,0), 16);
            Animation.AddAnimation("idle_charge_1", new Vector2(0,11), 8);
            Animation.AddAnimation("idle_charge_2", new Vector2(12,11), 4);
            Animation.AddAnimation("idle_fire", new Vector2(8,11), 8);

            Animation.AddAnimation("run", new Vector2(0,1), 8);
            Animation.AddAnimation("run_charge_1", new Vector2(8,1), 8);
            Animation.AddAnimation("run_charge_2", new Vector2(8,2), 8);

            Animation.AddAnimation("melee", new Vector2(0,2), 8);

            Animation.AddAnimation("jump", new Vector2(0,3), 4);
            Animation.AddAnimation("jump_charge_1", new Vector2(0,4), 4);
            Animation.AddAnimation("jump_charge_2", new Vector2(0,5), 4);

            Animation.AddAnimation("fall", new Vector2(5,3), 4);
            Animation.AddAnimation("fall_charge_1", new Vector2(5,4), 4);
            Animation.AddAnimation("fall_charge_2", new Vector2(5,5), 4);    

            Animation.AddAnimation("glide", new Vector2(10,3), 4);
            Animation.AddAnimation("glide_charge", new Vector2(10,4), 4);

            Animation.AddAnimation("dash", new Vector2(0,6), 4);
            Animation.AddAnimation("dash_charge", new Vector2(5,6), 4);

            Animation.AddAnimation("crouch", new Vector2(0,7), 16);
            Animation.AddAnimation("crouch_charge_1", new Vector2(0,9), 4);
            Animation.AddAnimation("crouch_charge_2", new Vector2(0,10), 4);

            Animation.AddAnimation("crawl", new Vector2(0,8), 8);
            Animation.AddAnimation("crawl_charge_1", new Vector2(4,9), 8);
            Animation.AddAnimation("crawl_charge_2", new Vector2(4,10), 8);

            Animation.AddAnimation("climb_idle_1", new Vector2(0,12), 16);
            Animation.AddAnimation("climb_up_1", new Vector2(0,13), 8);
            Animation.AddAnimation("climb_down_1", new Vector2(8,13), 4);

            //TODO : Add sword attack animation
            Animation.AddAnimation("sword", new Vector2(8,8), 8);

            Animation.ChangeAnimation(_currentAnimation);

            HandAnimation = new Animation(texture, 80, 64, new Vector2(80*16 , 64*18), 24);

            HandAnimation.AddAnimation("idle", new Vector2(4,3), 1);

            HandAnimation.AddAnimation("charge_1", new Vector2(3,14), 1);
            HandAnimation.AddAnimation("charge_1_to_2", new Vector2(12,14), 4);
            HandAnimation.AddAnimation("charge_2", new Vector2(3,15), 1);
            HandAnimation.AddAnimation("charge_3", new Vector2(8,14), 4);
            HandAnimation.AddAnimation("charge_4", new Vector2(8,15), 4);

            HandAnimation.AddAnimation("fire_1", new Vector2(0,14), 8);
            HandAnimation.AddAnimation("fire_2", new Vector2(0,15), 8);

            HandAnimation.AddAnimation("gun_1", new Vector2(0,16), 8);
            HandAnimation.AddAnimation("gun_2", new Vector2(8,16), 8);

            HandAnimation.ChangeAnimation(_currentHandAnimation);

            paticleTexture.SetData([new Color(193, 255, 219)]);
            _particle = new SoulParticle(10, Position, paticleTexture);
        }

        public override void Reset()
        {
            ItemSlot = new Item[4];
            usingConsumableItem = new List<Item>();

            Direction = 1; // Reset direction to right

            Health = MaxHealth;
            MP = MaxMP;
            AbsorptionHealth = 0;

            ChangeToFistAttack();
            ChangeToSoulBulletAttack();
            ResetJumpStrength();

            _overlappedTile = TileType.None;


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
            _MPRegenTime = 0f;

            Bullet.DamageAmount = Bullet.BaseDamageAmount;
            
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleInput(deltaTime, gameObjects);
            RegenerateMP(deltaTime);
            ActiveItemPassiveAbility(deltaTime);
            UpdateUsingItem(deltaTime);
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
            UpdateHandAnimation(deltaTime);
            UpdateAnimation(deltaTime);

            if (!_isDashing) Velocity.X = 0;

            ResetJumpStrength();

            //Console.WriteLine(AttackDamage);
            
            base.Update(gameTime, gameObjects, tileMap);
            _particle.Update(Position);    
        }

        private bool IsGameOver()
        {
            return Life <= 0;
        }

        private void RegenerateMP(float deltaTime)
        {
            if(_MPRegenTime < MPRegenCooldown)
                _MPRegenTime += deltaTime;

            else if(_MPRegenTime >= MPRegenCooldown){
                _MPRegenTime = MPRegenCooldown;
                MP += MPRegenRate * deltaTime;
            }

            if(MP >= MaxMP) 
                MP = MaxMP;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _particle.Draw(spriteBatch);

            SpriteEffects spriteEffect = Direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 offset = new Vector2(0 , 0);
            Vector2 handOffset = new Vector2(0 , 0);
            if (_isCrouching)
            {
                offset = new Vector2(0 , 8);
                handOffset = new Vector2(0 , 2);
            }

            if (_isClimbing && _overlappedTile == TileType.Ladder)
            {
                if (Direction == 1){
                    offset = new Vector2(6 , 0);
                }
                else if(Direction == -1){
                    offset = new Vector2(-6 , 0);
                }
            }

            Color color = IsInvincible() ? Color.Red : Color.White;

            spriteBatch.Draw(
                Animation.GetTexture(),
                GetDrawingPosition() - offset,
                Animation.GetCurrentFrame(),
                color,
                0f, 
                Vector2.Zero,
                Scale,
                spriteEffect, 
                0f
            );

            if (_currentHandAnimation != "none")
            {
                spriteBatch.Draw(
                    HandAnimation.GetTexture(),
                    GetDrawingPosition() - handOffset,
                    HandAnimation.GetCurrentFrame(),
                    color,
                    0f, 
                    Vector2.Zero,
                    Scale,
                    spriteEffect, 
                    0f
                );
            }

            //DrawDebug(spriteBatch);
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            string animation = "idle";

            if(_isAttacking){
                if(_isFist)
                    animation = "melee";
                else
                    animation = "sword";
            }

            else if (_isDashing){
                if (HandAnimation._currentAnimation == "idle" || 
                    HandAnimation._currentAnimation == "charge_1" || 
                    HandAnimation._currentAnimation == "fire_1")
                    animation = "dash";
                else
                    animation = "dash_charge";
            }

            else if (_isGliding && !_isJumping)
            {
                if (HandAnimation._currentAnimation == "idle")
                    animation = "glide";
                else
                    animation = "glide_charge";
            }
            else if (_isClimbing)
            {
                if (Velocity.Y < 0)
                    animation = "climb_up_1";
                else if(Velocity.Y > 0)
                    animation = "climb_down_1";
                else
                    animation = "climb_idle_1";
            }

            else if (Velocity.Y > 0)
            {
                if (HandAnimation._currentAnimation == "idle")
                    animation = "fall";
                else if (HandAnimation._currentAnimation == "charge_1" || 
                         HandAnimation._currentAnimation == "fire_1")
                    animation = "fall_charge_1";
                else
                    animation = "fall_charge_2";
            }

            else if (_isJumping && Velocity.Y < 0)
            {
                if (HandAnimation._currentAnimation == "idle")
                    animation = "jump";
                else if (HandAnimation._currentAnimation == "charge_1" || 
                         HandAnimation._currentAnimation == "fire_1")
                    animation = "jump_charge_1";
                else
                    animation = "jump_charge_2";
            }

            else if (Velocity.X != 0)
            {
                if(_isCrouching){
                    if (HandAnimation._currentAnimation == "idle")
                        animation = "crawl";
                    else if (HandAnimation._currentAnimation == "charge_1" || 
                             HandAnimation._currentAnimation == "fire_1")
                        animation = "crawl_charge_1";
                    else
                        animation = "crawl_charge_2";
                }

                else
                {
                    if (HandAnimation._currentAnimation == "idle")
                        animation = "run";
                    else if (HandAnimation._currentAnimation == "charge_1" || 
                             HandAnimation._currentAnimation == "fire_1")
                        animation = "run_charge_1";
                    else
                        animation = "run_charge_2";
                }
            }

            else
            {
                if(_isCrouching)
                {
                    if (HandAnimation._currentAnimation == "idle")
                        animation = "crouch";
                    else if (HandAnimation._currentAnimation == "charge_1" || 
                             HandAnimation._currentAnimation == "fire_1")
                        animation = "crouch_charge_1";
                    else
                        animation = "crouch_charge_2";
                }

                else
                {
                    if (HandAnimation._currentAnimation == "idle")
                        animation = "idle";
                    else if (HandAnimation._currentAnimation == "charge_1" || 
                             HandAnimation._currentAnimation == "fire_1")
                        animation = "idle_charge_1";
                    else if(HandAnimation._currentAnimation == "fire_2")
                        animation = "idle_fire";
                    else
                        animation = "idle_charge_2";

                }
            }

            if(_currentAnimation != animation){
                _currentAnimation = animation;
                switch (animation)
                {
                    case "run" :
                    case "run_charge_1" :
                    case "run_charge_2" :
                        Animation.ChangeAnimationAndKeepFrame(_currentAnimation);
                        break;
                    default:
                        Animation.ChangeAnimation(_currentAnimation);
                        break;
                }    
            }

            base.UpdateAnimation(deltaTime);
        }

        protected void UpdateHandAnimation(float deltaTime)
        {
            string handAnimation = "idle";

            if(_isAttacking){
                handAnimation = "idle";
            }

            else if (_isGliding && !_isJumping)
            {
                if(Singleton.Instance.IsKeyJustPressed(Fire) && _isUsingWeapon)
                    handAnimation = "gun_2";
                    
                else if (_lastChargeTime != 0)
                {
                    handAnimation = "fire_1";
                    _lastChargeTime = 0;
                }

                else if (_isCharging){
                    if (_chargeTime == MaxChargeTime)
                        handAnimation = "charge_4"; 
                    else
                        handAnimation = "charge_1";                 
                }
            }

            else
            {
                if(Singleton.Instance.IsKeyJustPressed(Fire) && _isUsingWeapon)
                    handAnimation = "gun_1";

                else if (_lastChargeTime != 0)
                {
                    if (_lastChargeTime >= MaxChargeTime/2)
                        handAnimation = "fire_2";

                    else
                        handAnimation = "fire_1";

                    _lastChargeTime = 0;
                }

                else if (_isCharging)
                {
                    if (_chargeTime == MaxChargeTime)
                        handAnimation = "charge_3";
                    else if (_chargeTime >= MaxChargeTime/2)
                        handAnimation = "charge_1_to_2";
                    else
                        handAnimation = "charge_1";
                }
            }

            if(_currentHandAnimation != handAnimation || _isAttacking){
                _currentHandAnimation = handAnimation;

                switch (handAnimation)
                {
                    case "charge_1_to_2" :
                        HandAnimation.ChangeTransitionAnimation(_currentHandAnimation, "charge_2");
                        break;
                    case "fire_1" :
                    case "fire_2" :
                    case "gun_1" :
                    case "gun_2" :
                        HandAnimation.ChangeTransitionAnimation(_currentHandAnimation, "idle");
                        _currentHandAnimation = "idle";
                        break;
                    default:
                        HandAnimation.ChangeAnimation(_currentHandAnimation);
                        break;
                }  
            }

            HandAnimation.Update(deltaTime);
        }

        private void HandleInput(float deltaTime, List<GameObject> gameObjects)
        {
            if (Singleton.Instance.IsKeyPressed(Left))
            {
                if (!_isDashing && !_isAttacking) 
                {
                    if (_isClimbing && _overlappedTile == TileType.Ladder)
                        Direction = 1;
                    else if(!_isClimbing)
                    {
                        Direction = -1;
                        Velocity.X = -WalkSpeed;
                    }
                }
            }
            if (Singleton.Instance.IsKeyPressed(Right))
            {
                if (!_isDashing && !_isAttacking)  
                {
                    if (_isClimbing && _overlappedTile == TileType.Ladder)
                        Direction = -1;
                    else if(!_isClimbing)
                    {
                        Direction = 1;
                        Velocity.X = WalkSpeed;
                    }
                }
            }

            if (Singleton.Instance.IsKeyJustPressed(Attack)){
                _isCharging = false;
                _chargeTime = 0;
                StartAttack();
            }

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
            if (Singleton.Instance.IsKeyJustPressed(Fire) && !_isClimbing && !_isAttacking)
            {
                if(_isSoulBullet){
                    // Start charging
                    _isUsingWeapon = false;
                    StartCharging();
                }
                else{
                    if ((ItemSlot[1] is Staff && MP >= 0) || ItemSlot[1] is Gun)
                    {
                        _isUsingWeapon = true;
                        Shoot(gameObjects);
                    }
                    else
                    {
                        _isUsingWeapon = false;
                    }
                }
            }
            else if (Singleton.Instance.IsKeyPressed(Fire) && !_isClimbing && !_isAttacking)
            {
                // Continue charging
                ContinueCharging(deltaTime);
            }
            else if (Singleton.Instance.IsKeyJustReleased(Fire) && !_isClimbing && !_isAttacking)
            {
                // Release shot
                ReleaseChargedShot(gameObjects);
            }

            if (Singleton.Instance.IsKeyJustPressed(Dash)){
                if(_isClimbing){
                    Direction *= -1;
                    _isClimbing = false;
                }
                StartDash();
            }

            if (Singleton.Instance.IsKeyJustPressed(Jump) && !_isDashing && _coyoteTimeCounter > 0)
            {

                if (!(Singleton.Instance.IsKeyPressed(Crouch) && _isClimbing) && 
                !(Singleton.Instance.IsKeyPressed(Crouch) && _isOnPlatform))
                {

                    _isClimbing = false;
                    if (_isCrouching) {
                        Position.Y -= 16;
                        Viewport.Height = 32;
                        WalkSpeed = 200;
                        _isCrouching = false;
                    }

                    _jumpBufferCounter = JumpBufferTime;
                }

                _isClimbing = false;

            }
            else
                _jumpBufferCounter -= deltaTime; // Decrease over time

            // Gliding - activate when holding Jump while in air and not climbing or dashing
            if (Singleton.Instance.IsKeyPressed(Jump) && !IsOnGround() && 
            !_isJumping && !_isClimbing && !_isDashing && !_isAttacking && 
            MP > 0 && _coyoteTimeCounter <= 0)
            {
                _isGliding = true;
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

            if (Singleton.Instance.IsKeyPressed(Crouch) && Singleton.Instance.IsKeyJustPressed(Jump) && _isOnPlatform){
                _isDropping = true;
            }
            else
            {
                _isDropping = false;
            }

            if (Singleton.Instance.IsKeyPressed(Climb) && (Velocity.Y >= 0) &&
                !_isClimbing && !_isCrouching && !_isDashing && Position.Y > (_ladderTopPosition.Y + 8) &&
                (_overlappedTile == TileType.Ladder || _overlappedTile == TileType.Ladder_Platform ||
                _overlappedTile == TileType.Ladder_Left || _overlappedTile == TileType.Ladder_Right))
            {
                _isClimbing = true;
                _isCharging = false;
                _isJumping = false;
                _chargeTime = 0;
                Velocity.Y = 0;
                Position.X = _overlappedTilePosition.X;

                if(_overlappedTile == TileType.Ladder_Left)
                    Direction = -1;
                else if(_overlappedTile == TileType.Ladder_Right)
                    Direction = 1;

            }

            if (_isClimbing)
            {
                Position.X = _overlappedTilePosition.X;
                
                if (Singleton.Instance.IsKeyPressed(Climb) && Position.Y >= (_ladderTopPosition.Y + 8))
                {
                    Velocity.Y = -ClimbSpeed;
                }

                else if (Singleton.Instance.IsKeyPressed(Crouch))
                {
                    Velocity.Y = ClimbSpeed;
                }

                else Velocity.Y = 0;
                
                
                if (_overlappedTile == TileType.None)
                    _isClimbing = false;

            }
            
            if (Singleton.Instance.IsKeyJustPressed(Interact))
                CheckInteraction(gameObjects);

            if (Singleton.Instance.IsKeyJustPressed(Item1))
                UseItem(2);
            if (Singleton.Instance.IsKeyJustPressed(Item2))
                UseItem(3);
        }

        protected override bool IsOnGround()
        {
            //TODO apex of jump is grounded?
            return Velocity.Y == 0 || _isClimbing;
        }

        private void ActiveItemPassiveAbility(float deltaTime)
        {
            for (int i = 0; i < 4; i++)
            {
                if (ItemSlot[i] == null || ItemSlot[i].Type == ItemType.Consumable) continue;

                ItemSlot[i].ActiveAbility(deltaTime, i);
                
            }
        }

        private void UpdateUsingItem(float deltaTime)
        {
            for (int i = usingConsumableItem.Count - 1; i >= 0; i--)
            {
                if(!usingConsumableItem[i].IsActive)
                {
                    usingConsumableItem.RemoveAt(i);
                }
                else
                {
                    usingConsumableItem[i].ActiveAbility(deltaTime, i);
                }
            }
        }

        public void BoostSpeed(float speedModifier)
        {
            if(_isDashing) 
                return;
            Velocity.X *= speedModifier;
        }

        public void BoostJump(float jumpStrengthModifier)
        {
            JumpStrength *= jumpStrengthModifier;
        }

        private void UseItem(int itemSlotIndex)
        {
            if(ItemSlot[itemSlotIndex] == null || ItemSlot[itemSlotIndex].Type != ItemType.Consumable) return;

            ItemSlot[itemSlotIndex].Use(itemSlotIndex);
        }

        private void CheckInteraction(List<GameObject> gameObjects)
        {
            foreach (var item in gameObjects.OfType<Item>())
            {
                if (item.InPickupRadius() && !item.IsPickedUp)
                {
                    if(item.Type == ItemType.MeleeWeapon)
                    {
                        ItemSlot[0]?.OnDrop(Position);
                        item.OnPickup(0);
                        break;
                        // You could add a pickup sound or effect here
                    }
                    else if(item.Type == ItemType.RangeWeapon)
                    {
                        ItemSlot[1]?.OnDrop(Position);
                        item.OnPickup(1);
                        break;
                        // You could add a pickup sound or effect here
                    }
                    else if(item.Type != ItemType.MeleeWeapon && item.Type != ItemType.RangeWeapon && ItemSlot[2] == null)
                    {
                        item.OnPickup(2);
                        break;
                        // You could add a pickup sound or effect here
                    }
                    else if(item.Type != ItemType.MeleeWeapon && item.Type != ItemType.RangeWeapon && ItemSlot[3] == null)
                    {
                        item.OnPickup(3);
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

                if(_isFist)
                {
                    PunchSound.Play();
                }
                else
                {
                    (ItemSlot[0] as Sword).SlashSound.Play();
                }

                UpdateAttackHitbox();
            }
        }

        private void UpdateAttackHitbox()
        {
            if (_isAttacking)
            {
                if(ItemSlot[0] != null)
                {
                    AttackHitbox = (ItemSlot[0] as Sword).GetAttackHitbox();
                }
                else
                {
                    int offsetX = Direction == 1 ? Rectangle.Width : -AttackWidth;

                    AttackHitbox = new Rectangle((int)Position.X + offsetX, (int)Position.Y, AttackWidth, AttackHeight);
                }
            }
        }
        
        private void CheckAttackHit(List<GameObject> gameObjects)
        {
            if (!_isAttacking) return;
            foreach (var enemy in gameObjects.OfType<BaseEnemy>())
            {
                enemy.CheckHit(AttackHitbox, AttackDamage);
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
                DashSound.Play();
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

            _isOnPlatform = false;
            _ladderTopPosition = Vector2.Zero;
            _overlappedTile = TileType.None;

            int radius = 5;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    Vector2 newPosition = new(Position.X + i * Singleton.TILE_SIZE, Position.Y + j * Singleton.TILE_SIZE);
                    Tile tile = tileMap.GetTileAtWorldPostion(newPosition);
                    if(tile != null)
                    {
                        if (tile.Type == TileType.Ladder || tile.Type == TileType.Ladder_Left || 
                        tile.Type == TileType.Ladder_Right || tile.Type == TileType.Ladder_Platform)
                        {
                            if (IsTouching(tile)){
                                _overlappedTile = tile.Type;
                                _overlappedTilePosition = tile.Position;
                            }
                        }

                        if (tile.Type == TileType.Ladder_Top && IsTouching(tile))
                            _ladderTopPosition = tile.Position;


                        if (tile.Type == TileType.Platform|| tile.Type == TileType.Ladder_Platform)
                        {
                            if(IsTouchingTop(tile))
                                _isOnPlatform = true;
                            if (tile.Position.Y < Position.Y + Viewport.Height || _isDropping)
                                tile.IsSolid = false;
                            else
                                tile.IsSolid = true;
                        }

                    }
                }
            }
        }


        private void Shoot(List<GameObject> gameObjects)
        {
            IShootable rangeWeapon = ItemSlot[1] as IShootable;
            
            if (rangeWeapon == null || !rangeWeapon.CanShoot())
                return;
            
            // Get position offset based on player state
            Vector2 bulletPositionOffset = _isCrouching ? new Vector2(0, 6) : new Vector2(0, 16); 
            Vector2 bulletPosition = Position + bulletPositionOffset;
            
            // Create and configure the projectile using the weapon
            Projectile newProjectile = rangeWeapon.CreateProjectile(bulletPosition, Direction);
            if (rangeWeapon == ItemSlot[1] as Gun)
                newProjectile.spriteViewport = new Rectangle(48, 0, 16, 16);
            else if (rangeWeapon == ItemSlot[1] as Staff)
                newProjectile.spriteViewport = new Rectangle(32, 16, 16, 16);

            // Handle the effects of shooting
            rangeWeapon.OnShoot();
            
            // Add projectile to game world
            gameObjects.Add(newProjectile);
            Animation.Reset();
        }

       // Charge shot methods
        private void StartCharging()
        {
            if (MP >= StartChargeMPCost)
            {
                MP -= StartChargeMPCost;
                _isCharging = true;
                _chargeTime = 0f;
                // chargeColor = Color.White;
                
                // // Play charge start sound if available
                // if (ChargeSound != null)
                //     ChargeSound.Play();
                
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
            if (_chargeTime < MaxChargeTime)
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

            Vector2 bulletPositionOffset = _isCrouching ? new Vector2(0, 6) : new Vector2(0, 16); 
            Vector2 bulletPosition = Position + bulletPositionOffset;

            newBullet.Shoot(bulletPosition, direction);
            if (chargeRatio == 1.0)
                newBullet.spriteViewport = new Rectangle(32, 0, 16, 16);
            else if (chargeRatio >= 0.5)
                newBullet.spriteViewport = new Rectangle(16, 0, 16, 16);
            else
                newBullet.spriteViewport = new Rectangle(0, 0, 16, 16);
            
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
            _lastChargeTime = _chargeTime;
            _chargeTime = 0f;
        }

        public override void OnHitByProjectile(GameObject gameObject,float damageAmount)
        {
            OnHit(damageAmount);
        }
        
        public override void OnHit(float damageAmount)
        {
            if (IsInvincible()) 
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

            Singleton.Instance.Camera.ShakeScreen(damageAmount * 1.75f / MaxHealth, 0.3f);

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

        protected override void DrawDebug(SpriteBatch spriteBatch)
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
            spriteBatch.DrawString(Singleton.Instance.GameFont, displayText, textPosition, Color.White);
            spriteBatch.DrawString(Singleton.Instance.GameFont, ".", this.GetPlayerCenter(), Color.Red);
            base.DrawDebug(spriteBatch);
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

        public void UseMP(float MPAmount)
        {
            _MPRegenTime = 0;
            MP -= MPAmount;
            ClampMP();
        }

        public void DrainMP(float MPAmount, float deltaTime)
        {
            // Consume MP while gliding
            _MPRegenTime = 0;
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

        public void ChangeToRangeWeaponAttack(float damageAmount)
        {
            _isSoulBullet = false;
            Bullet.DamageAmount = damageAmount;
        }

        public void ChangeToSoulBulletAttack()
        {
            _isSoulBullet = true;
            Bullet.DamageAmount = Bullet.BaseDamageAmount;
        }

        public void RemoveItem(int slot)
        {
            ItemSlot[slot].IsActive = false;
            ItemSlot[slot] = null;
        }

        public void AddUsingItem(int slot)
        {
            usingConsumableItem.Add(ItemSlot[slot]);
            ItemSlot[slot] = null;
        }
    }
}