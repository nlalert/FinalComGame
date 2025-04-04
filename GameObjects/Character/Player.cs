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
        public GrapplingHook _grapplingHook;

        public Keys Left, Right, Fire, Jump, Attack, Dash, Crouch, Climb, Interact, Item1, Item2, Grapple;
        
        public float MaxMP;
        public float MP;
        public int Life;
        public float AbsorptionHealth;

        // Inventory system
        public Inventory Inventory { get; private set; }

        public SoulParticle _particle;

        public int CrouchSpeed;
        public int ClimbSpeed;

        private bool _isClimbing;
        private bool _isCrouching;
        private bool _isDropping;
        private bool _isGliding;

        //Crouch
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
        public AbilityManager Abilities;

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

        // Grappring Hook
        public Texture2D _hookHeadTexture;
        public Texture2D _ropeTexture;
        private Vector2 GrapplingPosition;
        public bool _isGrappling;
        public bool _haveLineOfSight = false;
        public float GrappleMP;

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

        private string _currentWeapon;

        //SFX
        public SoundEffect JumpSound;
        public SoundEffect DashSound;
        public SoundEffect PunchSound;
        public SoundEffect ChargingSound;
        public SoundEffect BulletShotSound;
        private SoundEffectInstance _chargingSoundInstance;
        
        //Animation
        private Animation HandAnimation;
        private string _currentHandAnimation = "idle";

        // Add these to your Player class variables
        private float _item1HoldTime;
        private float _item2HoldTime;
        private bool _isHoldingItem1;
        private bool _isHoldingItem2;
        private const float DROP_ITEM_THRESHOLD = 0.5f; // Half a second hold to drop

        public Player(Texture2D texture, Texture2D paticleTexture) : base(texture)
        {
            // Create inventory instance
            Inventory = new Inventory();

            Animation = new Animation(texture, 80, 64, new Vector2(80*16 , 64*18), 24);

            Animation.AddAnimation("idle", new Vector2(0,0), 16);
            Animation.AddAnimation("idle_charge_1", new Vector2(0,11), 8);
            Animation.AddAnimation("idle_charge_2", new Vector2(12,11), 4);
            Animation.AddAnimation("idle_fire", new Vector2(8,11), 8);

            Animation.AddAnimation("run", new Vector2(0,1), 8);
            Animation.AddAnimation("run_charge_1", new Vector2(8,1), 8);
            Animation.AddAnimation("run_charge_2", new Vector2(8,2), 8);

            Animation.AddAnimation("jump", new Vector2(0,3), 4);
            Animation.AddAnimation("jump_charge_1", new Vector2(0,4), 4);
            Animation.AddAnimation("jump_charge_2", new Vector2(0,5), 4);

            Animation.AddAnimation("fall", new Vector2(4,3), 4);
            Animation.AddAnimation("fall_charge_1", new Vector2(4,4), 4);
            Animation.AddAnimation("fall_charge_2", new Vector2(4,5), 4);    

            Animation.AddAnimation("glide", new Vector2(8,3), 4);
            Animation.AddAnimation("glide_charge", new Vector2(12,3), 4);

            Animation.AddAnimation("dash", new Vector2(0,6), 4);
            Animation.AddAnimation("dash_charge", new Vector2(5,6), 4);

            Animation.AddAnimation("crouch", new Vector2(0,7), 16);
            Animation.AddAnimation("crouch_charge_1", new Vector2(0,9), 4);
            Animation.AddAnimation("crouch_charge_2", new Vector2(0,10), 4);

            Animation.AddAnimation("crawl", new Vector2(0,8), 8);
            Animation.AddAnimation("crawl_charge_1", new Vector2(4,9), 8);
            Animation.AddAnimation("crawl_charge_2", new Vector2(4,10), 8);

            Animation.AddAnimation("roll", new Vector2(8,8), 8);
            Animation.AddAnimation("roll_charge_1", new Vector2(8,5), 8);
            Animation.AddAnimation("roll_charge_2", new Vector2(8,6), 8);

            Animation.AddAnimation("climb_idle_1", new Vector2(0,12), 16);
            Animation.AddAnimation("climb_up_1", new Vector2(0,13), 8);
            Animation.AddAnimation("climb_down_1", new Vector2(8,13), 4);

            Animation.AddAnimation("melee", new Vector2(0,2), 8);
            Animation.AddAnimation("sword", new Vector2(8,4), 8);

            Animation.ChangeAnimation(_currentAnimation);

            HandAnimation = new Animation(texture, 80, 64, new Vector2(80*16 , 64*18), 24);

            HandAnimation.AddAnimation("idle", new Vector2(12,9), 1);

            HandAnimation.AddAnimation("charge_1", new Vector2(3,14), 1);
            HandAnimation.AddAnimation("charge_1_to_2", new Vector2(12,14), 4);
            HandAnimation.AddAnimation("charge_2", new Vector2(3,15), 1);
            HandAnimation.AddAnimation("charge_3", new Vector2(8,14), 4);
            HandAnimation.AddAnimation("charge_4", new Vector2(8,15), 4);

            HandAnimation.AddAnimation("fire_1", new Vector2(0,14), 8);
            HandAnimation.AddAnimation("fire_2", new Vector2(0,15), 8);

            HandAnimation.AddAnimation("gun_1", new Vector2(0,16), 8);
            HandAnimation.AddAnimation("gun_2", new Vector2(8,16), 8);

            HandAnimation.AddAnimation("staff_1", new Vector2(0,17), 8);
            HandAnimation.AddAnimation("staff_2", new Vector2(8,17), 8);

            HandAnimation.ChangeAnimation(_currentHandAnimation);

            paticleTexture.SetData([new Color(193, 255, 219)]);
            _particle = new SoulParticle(10, Position, paticleTexture);
            Abilities = new AbilityManager();
        }

        public override void Reset()
        {
            // Reset inventory
            Inventory.Reset();
            Abilities.Reset();

            Direction = 1; // Reset direction to right

            Health = MaxHealth;
            MP = MaxMP;
            AbsorptionHealth = 0;

            ChangeToFistAttack();
            ChangeToSoulBulletAttack();
            ResetJumpStrength();

            _overlappedTile = TileType.None;
            
            _isHoldingItem1 = false;
            _isHoldingItem2 = false;

            _isClimbing = false;
            _isCrouching = false;
            _isDropping = false;
            _isGliding = false;
            _isAttacking = false;
            _isCharging = false;
            _isDashing = false;
            _isJumping = false;

            _item1HoldTime = 0f;
            _item2HoldTime = 0f;

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
            if (Health <= 0)
            {
                OnDead(gameObjects);
                return;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleInput(deltaTime, gameObjects, tileMap);
            RegenerateMP(deltaTime);
            
            // Update inventory items
            Inventory.UpdateActiveItemPassives(deltaTime, gameObjects);
            Inventory.UpdateActiveConsumables(deltaTime, gameObjects);
            
            UpdateInvincibilityTimer(deltaTime);
            UpdateCoyoteTime(deltaTime);
            CheckAndJump();
            UpdateDash(deltaTime);
            UpdateGlide();

            if (!_isClimbing && !_isDashing && !_isGrappling) 
                ApplyGravity(deltaTime);

            UpdateGrapplingHook(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            UpdateTileInteraction(tileMap);
            UpdateAttackHitbox();
            CheckAttackHit(gameObjects);
            UpdateHandAnimation(deltaTime);
            UpdateAnimation(deltaTime);

            if (!_isDashing) Velocity.X = 0;

            ResetJumpStrength();
            
            base.Update(gameTime, gameObjects, tileMap);
            _particle.Update(Position);    
        }

        private bool IsGameOver()
        {
            return Life <= 0;
        }

        private void RegenerateMP(float deltaTime)
        {
            if(_MPRegenTime < MPRegenCooldown && !_isCharging && !_isGrappling)
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

            color = _haveLineOfSight ? Color.White : Color.DimGray;

            if (!_isGrappling && GrapplingPosition != Vector2.Zero)
            {
                spriteBatch.Draw(
                    _texture,
                    GrapplingPosition - new Vector2 (16, 16),
                    ViewportManager.Get("Hook_Target"),
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
                if (!_isCrouching){
                    if (HandAnimation._currentAnimation == "idle" || 
                        HandAnimation._currentAnimation == "charge_1" || 
                        HandAnimation._currentAnimation == "fire_1")
                        animation = "dash";
                    else
                        animation = "dash_charge"; 
                }
                else
                {
                    if (HandAnimation._currentAnimation == "idle")
                        animation = "roll";
                    else if (HandAnimation._currentAnimation == "charge_1" || 
                            HandAnimation._currentAnimation == "fire_1")
                        animation = "roll_charge_1";
                    else
                        animation = "roll_charge_2";
                }
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

            else if (Velocity.X != 0 && (Singleton.Instance.IsKeyPressed(Left) || Singleton.Instance.IsKeyPressed(Right)))
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
                if(Singleton.Instance.IsKeyJustPressed(Fire) && _currentWeapon != "none"){
                    if(_currentWeapon == "gun")
                        handAnimation = "gun_2";
                    else
                        handAnimation = "staff_2";
                }
                    
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
                if(Singleton.Instance.IsKeyJustPressed(Fire) && _currentWeapon != "none"){
                    if(_currentWeapon == "gun")
                        handAnimation = "gun_1";
                    else
                        handAnimation = "staff_1";
                }

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
                    case "staff_1" :
                    case "staff_2" :
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

        private void HandleInput(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            if (Singleton.Instance.IsKeyPressed(Left))
            {
                if (!_isDashing && !_isAttacking && !_isGrappling) 
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
                if (!_isDashing && !_isAttacking && !_isGrappling)  
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

            if (Singleton.Instance.IsKeyJustPressed(Attack) && !_isClimbing && CanStandUp(tileMap)){

                if (_isCrouching) 
                {
                    Position.Y -= Singleton.TILE_SIZE;
                    Viewport.Height = Singleton.TILE_SIZE * 2;
                    WalkSpeed = 200;
                    _isCrouching = false;
                }

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
                if(_isSoulBullet)
                {
                    // Start charging
                    _currentWeapon = "none";
                    StartCharging();
                }
                else
                {
                    if (Inventory.GetItem(Inventory.RANGE_SLOT) is Staff && MP >= Inventory.GetItem(Inventory.RANGE_SLOT).MPCost)
                       _currentWeapon = "staff";
                    else if (Inventory.GetItem(Inventory.RANGE_SLOT) is Gun)
                        _currentWeapon = "gun";
                    else
                        _currentWeapon = "none";
                    Shoot(gameObjects);
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
                if (Abilities.IsAbilityUnlocked(AbilityType.Dash))
                {
                    if(_isClimbing){
                        Direction *= -1;
                        _isClimbing = false;
                    }
                    StartDash();
                }
            }
            if (Singleton.Instance.IsKeyJustPressed(Jump) && !_isDashing && !_isAttacking && _coyoteTimeCounter > 0)
            {
                if (!(Singleton.Instance.IsKeyPressed(Crouch) && _isClimbing) && 
                !(Singleton.Instance.IsKeyPressed(Crouch) && _isOnPlatform))
                {
                    _isClimbing = false;
                    if (_isCrouching) {
                        Position.Y -= Singleton.TILE_SIZE;
                        Viewport.Height = Singleton.TILE_SIZE * 2;
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
                _isGliding = Abilities.IsAbilityUnlocked(AbilityType.Glide);
            }
            else
            {
                _isGliding = false;
            }

            if (Singleton.Instance.IsKeyPressed(Crouch) && !_isJumping && !_isClimbing && Velocity.Y == 0 && !_isDashing)
            {
                if (!_isCrouching) Position.Y += Singleton.TILE_SIZE;
                Viewport.Height = Singleton.TILE_SIZE;
                WalkSpeed = CrouchSpeed;
                _isCrouching = true;
            }
            else if (_isCrouching && !_isDashing)
            {
                // Only stand up if there's enough room
                if (CanStandUp(tileMap))
                {
                    Position.Y -= Singleton.TILE_SIZE;
                    Viewport.Height = Singleton.TILE_SIZE * 2;
                    WalkSpeed = 200;
                    _isCrouching = false;
                }
                // If we can't stand up, force the player to remain crouched
                else
                {
                    _isCrouching = true;
                }
            }

            if (Singleton.Instance.IsKeyPressed(Crouch) && Singleton.Instance.IsKeyJustPressed(Jump) && _isOnPlatform){
                _isDropping = true;
            }
            else
            {
                _isDropping = false;
            }

            if (((Singleton.Instance.IsKeyPressed(Climb) && Position.Y > (_ladderTopPosition.Y + 8) && IsOnladder() && !_isCrouching) || 
                (Singleton.Instance.IsKeyPressed(Crouch) && _ladderTopPosition.Y != 0)) && 
                (Velocity.Y >= 0) && !_isClimbing && !_isDashing)
            {
                _isClimbing = true;
                _isCharging = false;
                _isJumping = false;
                _chargeTime = 0;
                Velocity.Y = 0;
                Position.X = _overlappedTilePosition.X;
                if (Position.Y < _ladderTopPosition.Y + 8)
                    Position.Y += Singleton.TILE_SIZE;
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
            
            if(Singleton.Instance.IsKeyJustPressed(Grapple) && Abilities.IsAbilityUnlocked(AbilityType.Grapple) && MP >= GrappleMP){
                if (GrapplingPosition != Vector2.Zero && HaveLineOfSight(tileMap))
                    FireGrapplingHook(gameObjects);
            }
            
            if (Singleton.Instance.IsKeyJustPressed(Interact))
                Inventory.CheckForItemPickup(gameObjects);

            if (Singleton.Instance.IsKeyJustPressed(Item1))
            {
                _isHoldingItem1 = true;
                _item1HoldTime = 0f;
            }
            else if (Singleton.Instance.IsKeyPressed(Item1) && _isHoldingItem1)
            {
                _item1HoldTime += deltaTime;
                
                // If held longer than threshold, drop the item
                if (_item1HoldTime >= DROP_ITEM_THRESHOLD)
                {
                    Inventory.DropItem(Inventory.ITEM_SLOT_1);
                    _isHoldingItem1 = false;
                }
            }
            else if (Singleton.Instance.IsKeyJustReleased(Item1))
            {
                if (_isHoldingItem1 && _item1HoldTime < DROP_ITEM_THRESHOLD)
                {
                    // Normal use if released before threshold
                    Inventory.UseItem(Inventory.ITEM_SLOT_1);
                }
                _isHoldingItem1 = false;
            }

            // Similarly for Item2
            if (Singleton.Instance.IsKeyJustPressed(Item2))
            {
                _isHoldingItem2 = true;
                _item2HoldTime = 0f;
            }
            else if (Singleton.Instance.IsKeyPressed(Item2) && _isHoldingItem2)
            {
                _item2HoldTime += deltaTime;
                
                // If held longer than threshold, drop the item
                if (_item2HoldTime >= DROP_ITEM_THRESHOLD)
                {
                    Inventory.DropItem(Inventory.ITEM_SLOT_2);
                    _isHoldingItem2 = false;
                }
            }
            else if (Singleton.Instance.IsKeyJustReleased(Item2))
            {
                if (_isHoldingItem2 && _item2HoldTime < DROP_ITEM_THRESHOLD)
                {
                    // Normal use if released before threshold
                    Inventory.UseItem(Inventory.ITEM_SLOT_2);
                }
                _isHoldingItem2 = false;
            }
            
            if (!_isGrappling && GrapplingPosition != Vector2.Zero)
                HaveLineOfSight(tileMap);
        }

        private bool IsOnladder(){
            return _overlappedTile == TileType.Ladder || _overlappedTile == TileType.Ladder_Platform;
        }

        protected override bool IsOnGround()
        {
            return Velocity.Y == 0 || _isClimbing ;
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
                    Inventory.GetSword().SlashSound.Play();
                }

                UpdateAttackHitbox();
            }
        }

        private void UpdateAttackHitbox()
        {
            if (_isAttacking)
            {
                if(Inventory.HasMeleeWeapon())
                {
                    AttackHitbox = Inventory.GetSword().GetAttackHitbox();
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
                enemy.CheckHit(AttackHitbox, AttackDamage, true);
            }
        }

        private void StartDash()
        {
            if (!Abilities.IsAbilityUnlocked(AbilityType.Dash))
                return;
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
            if (IsOnGround() || !Abilities.IsAbilityUnlocked(AbilityType.Glide))
            {
                _isGliding = false;
            }
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
            GrapplingPosition = Vector2.Zero;

            int radius = 10;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    Vector2 newPosition = new(Position.X + i * Singleton.TILE_SIZE, Position.Y + j * Singleton.TILE_SIZE);
                    Tile tile = tileMap.GetTileAtWorldPostion(newPosition);
                    if(tile != null)
                    {
                        if (tile.Type == TileType.Ladder || tile.Type == TileType.Ladder_Platform)
                        {
                            if (IsTouching(tile)){
                                _overlappedTile = tile.Type;
                                _overlappedTilePosition = tile.Position;
                            }
                        }

                        if (tile.Type == TileType.Ladder_Top && IsTouching(tile)){
                            _ladderTopPosition = tile.Position;
                        }

                        if (tile.Type == TileType.Grappling_Tile)
                        {
                            Vector2 _tempPosition = tile.Position + new Vector2(Singleton.TILE_SIZE/2, Singleton.TILE_SIZE/2);
                            if (GrapplingPosition == Vector2.Zero)
                            {
                                GrapplingPosition = _tempPosition;
                            }
                            else
                            {
                                if (Direction == 1 && _tempPosition.X > GrapplingPosition.X)
                                {
                                    GrapplingPosition = _tempPosition;
                                }
                                else if(Direction == -1 && _tempPosition.X < GrapplingPosition.X)
                                {
                                    GrapplingPosition = _tempPosition;
                                }
                            }
                        }


                        if (tile.Type == TileType.Platform|| tile.Type == TileType.Ladder_Platform)
                        {
                            if(IsTouchingTop(tile))
                                _isOnPlatform = true;
                            if (tile.Position.Y < Position.Y + Viewport.Height || _isDropping)
                                tile.IsSolid = false;
                            else
                                tile.IsSolid = true;
                        }

                        if (tile.Type == TileType.Finish_Line)
                        {
                            if (IsTouching(tile)) Singleton.Instance.CurrentGameState = Singleton.GameState.StageCompleted;
                        }

                    }
                }
            }
        }

        public bool HaveLineOfSight(TileMap tileMap){
            if (GrapplingPosition == Vector2.Zero) return false;
            
            Vector2 targetPosition = GrapplingPosition;
            Vector2 currentPosition = GetPlayerCenter();
            
            float step = Singleton.TILE_SIZE; // Tile size or step size for checking
            Vector2 direction = Vector2.Normalize(currentPosition - targetPosition);
            Vector2 checkPosition = targetPosition;

            while (Vector2.Distance(checkPosition, currentPosition) > step)
            {
                checkPosition += direction * step;
                if (tileMap.IsObstacle(checkPosition))
                {
                    _haveLineOfSight = false;
                    return false; // Blocked by an obstacle
                }
            }
            _haveLineOfSight = true;
            return true;
        }


        private void Shoot(List<GameObject> gameObjects)
        {
            IShootable rangeWeapon = Inventory.GetRangeWeapon();
            
            if (rangeWeapon == null || !rangeWeapon.CanShoot())
                return;
            
            // Get position offset based on player state
            Vector2 bulletPositionOffset = _isCrouching ? new Vector2(Direction * Singleton.TILE_SIZE, 6) : new Vector2(Direction * Singleton.TILE_SIZE, Singleton.TILE_SIZE); 
            Vector2 bulletPosition = Position + bulletPositionOffset;
            
            // Create and configure the projectile using the weapon
            Projectile newProjectile = rangeWeapon.CreateProjectile(bulletPosition, Direction);
            
            // Set sprite viewport based on weapon type
            if (rangeWeapon is Gun)
                newProjectile.Viewport = ViewportManager.Get("Gun_Bullet");
            else if (rangeWeapon is Staff)
                newProjectile.Viewport = ViewportManager.Get("Staff_Bullet");

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

                if(_chargingSoundInstance == null)
                    _chargingSoundInstance = ChargingSound.CreateInstance();
                _chargingSoundInstance.Volume = 1.0f;
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
                
            // Drain MP while charging (more MP for longer charge)
            if (_chargeTime < MaxChargeTime)
                DrainMP(ChargeMPCost / MaxChargeTime, deltaTime);

            if(_chargeTime > 0.3f && _chargingSoundInstance.State != SoundState.Playing)
                _chargingSoundInstance.Play();
        }
        
        private void ReleaseChargedShot(List<GameObject> gameObjects)
        {
            if (!_isCharging)
                return;

            if(_chargingSoundInstance.State == SoundState.Playing)
                _chargingSoundInstance.Volume *= 0.5f;
            
            // Calculate charge power (linear scaling from min to max)
            float chargeRatio = Math.Min(_chargeTime / MaxChargeTime, 1.0f);
            float chargePower = MinChargePower + chargeRatio * (MaxChargePower - MinChargePower);
            
            // Create and configure the bullet
            Vector2 direction = new Vector2(Direction, 0);
            PlayerBullet newBullet = Bullet.Clone() as PlayerBullet;
            newBullet.DamageAmount *= chargePower; // Increase damage

            Vector2 bulletPositionOffset = _isCrouching ? new Vector2(Direction * Singleton.TILE_SIZE, 6) : new Vector2(Direction * Singleton.TILE_SIZE, Singleton.TILE_SIZE); 
            Vector2 bulletPosition = Position + bulletPositionOffset;

            newBullet.Shoot(bulletPosition, direction);
            if (chargeRatio == 1.0)
                newBullet.Viewport = ViewportManager.Get("Charge_Bullet_2");
            else if (chargeRatio >= 0.5)
                newBullet.Viewport = ViewportManager.Get("Charge_Bullet_1");
            else
                newBullet.Viewport = ViewportManager.Get("Charge_Bullet_0");
            
            gameObjects.Add(newBullet);

            BulletShotSound.Play();
            
            // Reset charging state
            _isCharging = false;
            _lastChargeTime = _chargeTime;
            _chargeTime = 0f;
        }

        public override void OnHitByProjectile(GameObject gameObject, float damageAmount, bool isHeavyAttack)
        {
            OnHit(damageAmount, false);
        }
        
        public override void OnHit(float damageAmount, bool IsHeavyAttack)
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

            Singleton.Instance.Camera.ShakeScreen(damageAmount / Health, 0.3f);

            StartInvincibility(false);
            Console.WriteLine("Damage " + damageAmount + "CurHP" + Health);
        }

        public override void OnCollideNPC(Character npc, float damageAmount)
        {
            OnHit(damageAmount, false);
            //player.takeKnockback(npc.knockback);
            base.OnCollideNPC(npc, damageAmount);
        }

        public override void OnDead(List<GameObject> gameObjects)
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
            Inventory.RemoveItem(slot);
        }

        public void AddUsingItem(int slot)
        {
            Inventory.AddActiveConsumable(slot);
        }

        private void FireGrapplingHook(List<GameObject> gameObjects)
        {
            if (!Abilities.IsAbilityUnlocked(AbilityType.Grapple) || _grapplingHook != null)
                return;

            UseMP(GrappleMP);
            Console.WriteLine("shooting grapple");
            Vector2 AimDirection = GrapplingPosition - GetPlayerCenter();
            _grapplingHook = new GrapplingHook(_hookHeadTexture){
                Name = "GrapplingHook",
                BaseDamageAmount = 0f,
                Speed = 400f,
                Viewport = ViewportManager.Get("Grappling_Hook"),
                RopeTexture = _ropeTexture,
            }; // Load a grappling hook texture
            _grapplingHook.Shoot(GetPlayerCenter(), Vector2.Normalize(AimDirection));
            gameObjects.Add(_grapplingHook);
        }
        
        private void UpdateGrapplingHook(float deltaTime){
            if (_grapplingHook != null && _grapplingHook.Hooked)
            {
                _isGrappling = true;
                _isJumping = true;
                Vector2 direction = _grapplingHook.HookedPosition - Position;
                float distance = direction.Length();
                direction.Normalize();
                float pullSpeed = Math.Max(200f*distance,20000f);

                if (distance > 10f) // Move towards hook
                {
                    direction.Normalize();
                    Velocity = direction * pullSpeed * (float)deltaTime; // Pull speed
                }
                else
                {
                    // Reached the hook point
                    _isGrappling = false;
                    _grapplingHook.IsActive = false; // Destroy hook after pulling
                    _grapplingHook = null; 
                    _isJumping = false;
                }
            }
        }
        
        private bool CanStandUp(TileMap tileMap)
        {
            // Create a temporary hitbox to check if standing would cause collision
            Rectangle standingHitbox = new Rectangle(
                (int)Position.X,
                (int)Position.Y - Singleton.TILE_SIZE, // Where the head would be if standing
                Viewport.Width,
                Singleton.TILE_SIZE * 2); // Full height when standing
                
            // Check for collisions with solid tiles
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 0; j++) // Only check above the player
                {
                    Vector2 tilePos = new Vector2(
                        Position.X + i * Singleton.TILE_SIZE,
                        Position.Y - Singleton.TILE_SIZE + j * Singleton.TILE_SIZE);
                        
                    Tile tile = tileMap.GetTileAtWorldPostion(tilePos);
                    if (tile != null && tile.IsSolid && tile.Type != TileType.Platform)
                    {
                        Rectangle tileRect = new Rectangle(
                            (int)tile.Position.X,
                            (int)tile.Position.Y,
                            Singleton.TILE_SIZE,
                            Singleton.TILE_SIZE);
                            
                        if (standingHitbox.Intersects(tileRect))
                        {
                            return false; // Collision detected, can't stand up
                        }
                    }
                }
            }
            
            return true; // No collision, can stand up
        }
    }
}