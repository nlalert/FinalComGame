using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Cerberus : BaseEnemy
    {
        private float _actionTimer = 0;     // Timer for aim/charge states
        private bool _isEnraged = false;    // Enrage phase flag
        public float Friction ;
        public float FloatDuration = 0.5f;   // Time floating above the player
        public float SlamChargeDuration = 0.5f;  // Charging before slam
        public float SlamSpeed = 1500f;
        public int JumpsBeforeHighJump = 3;   // Number of normal jumps before high jump

        private float _chargeTime = 2.0f;
        private bool _isDashing;
        private float _dashTimer = 0f;
        private float _dashDuration = 2f;
        private bool _isSummoned = false;
        public float _actionTimeOffset = 1f;
        public int CerberusCount = 3;
        
        private Vector2 _dashAim;
        private Vector2 _dashStart;
        
        private Vector2 _barrierstart;
        private Vector2 _barrierEnd ;
        private Vector2 _barrierEnd1;
        public TextUI DisplayNameUI;
        public HealthBar HealthBar;
        public SoundEffect DogSound;


        public Cerberus(Texture2D texture, Texture2D particleTexture) : base(texture) 
        { 
            _texture = texture;
            CanCollideTile = true;
            IsIgnorePlatform = true;
        }
        public override void Reset()
        {
            _actionTimer = 0;     // Timer for aim/charge states
            _isEnraged = false;    // Enrage phase flag
            FloatDuration = 0.5f;   // Time floating above the player
            SlamChargeDuration = 0.5f;  // Charging before slam
            SlamSpeed = 1000f;
            JumpsBeforeHighJump = 3;   // Number of normal jumps before high jump
            _chargeTime = 2.0f;
            _dashTimer = 0f;
            _dashDuration = 2f;
            _isSummoned = false;
            _actionTimeOffset = 1f;
            IsIgnorePlatform = true;
            CerberusCount = 3;
            base.Reset();
        }
        public override void AddAnimation(){
            Animation = new Animation(_texture, 96, 80, new Vector2(96*8, 80*5), 24);

            Animation.AddAnimation("idle", new Vector2(0,0), 8);
            Animation.AddAnimation("charge", new Vector2(0,1), 8);
            Animation.AddAnimation("dash", new Vector2(0,3), 2);
            Animation.AddAnimation("run", new Vector2(0,4), 3);

            Animation.AddAnimation("sd_charge", new Vector2(0,2), 8);
            Animation.AddAnimation("sd_dash", new Vector2(3,3), 2);
            Animation.AddAnimation("sd_run", new Vector2(3,4), 3);

            Animation.ChangeAnimation("idle");
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (CurrentState == EnemyState.Dead || CurrentState == EnemyState.Dying)
            {
                IsActive = false;
                return;
            }

            UpdateInvincibilityTimer(deltaTime);

            if(!_isEnraged && Health <= MaxHealth * 0.5f){
                Split(gameObjects);
            }

            if (!_isEnraged && Health <= MaxHealth * 0.2f) 
            {
                _isEnraged = true;
            }

            switch (CurrentState)
            {
                case EnemyState.Chase:
                    if (Velocity.X > 0)
                        Direction = 1;
                    else if (Velocity.X < 0)
                        Direction = -1;
                    AI_Chase(deltaTime, gameObjects, tileMap);
                    break;
                case EnemyState.Jump:
                    AI_Jump(deltaTime, gameObjects, tileMap);
                    break;
                case EnemyState.Charging:
                    if (Singleton.Instance.Player.GetPlayerCenter().X > Position.X)
                        Direction = 1;
                    else if (Singleton.Instance.Player.GetPlayerCenter().X < Position.X)
                        Direction = -1;
                    AI_Charging(deltaTime,gameObjects, tileMap);
                    break;
                case EnemyState.Dash:
                    if (Singleton.Instance.Player.GetPlayerCenter().X > Position.X)
                        Direction = 1;
                    else if (Singleton.Instance.Player.GetPlayerCenter().X < Position.X)
                        Direction = -1;
                    AI_Dash(deltaTime,gameObjects, tileMap);
                    break;
            }

            UpdateAnimation(deltaTime);
            base.Update(gameTime, gameObjects, tileMap);
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            string animation = "idle";
            Animation.SetFPS(24);

            switch (CurrentState)
            {
                case EnemyState.Charging:
                case EnemyState.Jump:
                    animation = "charge";
                    break;

                case EnemyState.Dash:
                    animation = "dash";
                    break;
                    
                case EnemyState.Chase:
                    Animation.SetFPS(12);
                    animation = "run";
                    break;

                case EnemyState.Idle:
                    animation = "idle";
                    break;
            }
                
            if(_currentAnimation != animation && !Animation.IsTransition)
            {
                _currentAnimation = animation;
                switch (animation)
                {
                    default:
                        Animation.ChangeAnimation(_currentAnimation);
                        break;
                }    
            }
     
            base.UpdateAnimation(deltaTime);
        }

        private void AI_Chase(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

            _actionTimer -= deltaTime * (_isEnraged ? 1.5f : 1) * _actionTimeOffset;
            if(Math.Abs((Singleton.Instance.Player.Position - this.Position).X) >20f)
                base.Direction = Math.Sign( (Singleton.Instance.Player.Position - this.Position).X);

            Velocity.X = 150f * base.Direction;
            Velocity.X *= Friction;

            if (_actionTimer <= 0)
            {
                // random attack pattern
                if(Singleton.Instance.Random.NextDouble() > 0.5f){
                // if(false){
                    CurrentState = EnemyState.Charging;
                    _actionTimer = _chargeTime;
                    _dashAim = Singleton.Instance.Player.GetPlayerCenter(); // Lock on to player's position
                }
                else{
                    Console.WriteLine("Going to High Jump");
                    CurrentState = EnemyState.Jump;  // Jump High
                    _actionTimer = 1f;
                    _dashAim = Singleton.Instance.Player.GetPlayerCenter(); // Lock on to player's position
                }
            }
        }

        private void AI_Jump(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            _actionTimer -= deltaTime * (_isEnraged ? 1.5f : 1);
            if (!_isJumping)
            {
                _isJumping = true;
                Console.WriteLine(" going to AIM");
                int horizontalDir = Math.Sign(Singleton.Instance.Player.Position.X - Position.X);
                Velocity = new Vector2(0, -JumpStrength * 1.8f); // Jump higher
                CurrentState = EnemyState.Charging;
                CanCollideTile = false;// ignore all tile
                _actionTimer = 2.0f;
            }
        }
        private void AI_Charging(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            if(!_isJumping)
                ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            _actionTimer -= deltaTime;

            if(_isJumping && _actionTimer >0.0f ){
                if(Position.Y <= Singleton.Instance.Player.GetPlayerCenter().Y -150){
                    Velocity *= 0.5f;
                }
            }

            if (_actionTimer <= 0 )
            {
                //going to dash
                //Console.WriteLine("Hellhound dashes!");
                DogSound.Play();
                CanCollideTile = false;
                CurrentState = EnemyState.Dash;
                _dashStart = this.Position;
                _isDashing = true;
                _dashTimer = _dashDuration;
                Velocity = (_dashAim - Position);
                Velocity.Normalize();
                Velocity *= 900f;//Dash speed
                
                //finding barrier line to stop
                Vector2 direction = Position - _dashAim;
                direction.Normalize();
                Vector2 perpendicularDirection = new Vector2(-direction.Y, direction.X);
                _barrierstart = Singleton.Instance.Player.GetPlayerCenter() - direction * 100f;// push further from player
                _barrierEnd = _barrierstart - perpendicularDirection * 350;
                _barrierEnd1 = _barrierstart - perpendicularDirection * -350;
            }
            else{
                Velocity.X *= 0.95f;
                _dashAim = Singleton.Instance.Player.GetPlayerCenter();
            }
        }
        private void AI_Dash(float deltaTime,List<GameObject> gameObjects, TileMap tileMap)
        { 
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            

            if (_isDashing)
            {
                _dashTimer -= deltaTime;
                if(Vector2.Distance(_dashAim,Position)<50f){
                    var tile = tileMap.GetTileAtWorldPostion(Position);
                    if(tile!=null &&this.IsTouching(tileMap.GetTileAtWorldPostion(Position))){

                    }else 
                        CanCollideTile = true;
                }
                if (_dashTimer <=0 || IsIntersect(_barrierEnd,_barrierEnd1,_dashStart,Position))
                {
                    //Console.WriteLine("Hellhound finished dashing, switching to chase mode.");
                    _isDashing = false;
                    CurrentState = EnemyState.Chase;
                    _actionTimer =5f;
                    CanCollideTile = true;
                    _isJumping = false;
                }
            }
        }
        // private void OLDAI_SlamCharge(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        // {
        //     ApplyGravity(deltaTime);
        //     UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
        //     UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
        //     Console.WriteLine("Slamming");
        //     _actionTimer -= deltaTime * (_isEnraged ? 1.5f : 1);
        //     CanCollideTile = true;
        //     if(_actionTimer>0){
        //         Velocity = new Vector2(0, SlamSpeed);
        //     }
        //     _jumpCounter = 0;
        // }
        
        //MATH STUFF
        private bool Ccw(Vector2 A, Vector2 B, Vector2 C)
        {
            return (C.Y - A.Y) * (B.X - A.X) > (B.Y - A.Y) * (C.X - A.X);
        }
        private bool IsIntersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
        {
            return Ccw(A, C, D) != Ccw(B, C, D) && Ccw(A, B, C) != Ccw(A, B, D);
        }
        public override void OnCollisionHorizon()
        {
            if(CurrentState == EnemyState.Chase){
                if (!_isJumping){
                    _isJumping = true;
                    Velocity.Y = -600f;
                }
            }
            base.OnCollisionHorizon();
            if(CurrentState == EnemyState.Dash && _isDashing){
                _isDashing = false;
                CurrentState = EnemyState.Chase;
                _actionTimer =5f;
                CanCollideTile = true;
                _isJumping = false;
            }
        }
        public override void OnLandVerticle()
        {
            _isJumping = false;
            if(CurrentState == EnemyState.Dash && _isDashing){
                _isDashing = false;
                CurrentState = EnemyState.Chase;
                _actionTimer =5f;
                CanCollideTile = true;
                _isJumping = false;
            }
        }

        // protected override void UpdateAnimation(float deltaTime)
        // {
        //     string animation = "Chase";

        //     if (CurrentState == EnemyState.Dying)
        //     {
        //         animation = "die";
        //     }
        //     else if (CurrentState == EnemyState.Charging)
        //     {
        //         animation = "float";
        //     }
        //     else if (CurrentState == EnemyState.Dash)
        //     {
        //         animation = "slam";
        //     }
        //     else if (_isJumping)
        //     {
        //         animation = "jump";
        //     }

        //     if (_currentAnimation != animation)
        //     {
        //         _currentAnimation = animation;
        //         Animation.ChangeAnimation(_currentAnimation);
        //     }

        //     base.UpdateAnimation(deltaTime);
        // }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffect = base.Direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color color = IsInvincible() ? Color.Red : Color.White;

            spriteBatch.Draw(
                Animation.GetTexture(),
                GetDrawingPosition(),
                Animation.GetCurrentFrame(),
                color,
                0f, 
                Vector2.Zero,
                Scale,
                spriteEffect, 
                0f
            );

            // //DrawDebug(spriteBatch);
        }
    

        protected override void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 40);
            string displayText = $"State: {CurrentState}\n{base.Direction}\n HP {Health} \nAT:{_actionTimer} \n {CerberusCount}";
            spriteBatch.DrawString(Singleton.Instance.GameFont, displayText, textPosition , Color.White);
            if(CurrentState == EnemyState.Charging || CurrentState == EnemyState.Dash){
                DrawLine(spriteBatch, _dashAim, Position, Color.Green);
                // Draw 90-degree line at the Aim position
                DrawLine(spriteBatch,_barrierEnd,_barrierEnd1, Color.Blue);
            }
        }

        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            // Calculate the length and direction of the line
            float length = Vector2.Distance(start, end);
            Vector2 direction = end - start;
            direction.Normalize();

            // Draw the line (scaled 1x1 texture)
            spriteBatch.Draw(Singleton.Instance.PixelTexture, start, null, color, (float)Math.Atan2(direction.Y, direction.X), Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        protected override void ApplyGravity(float deltaTime)
        {
            Velocity.Y += Singleton.GRAVITY * deltaTime * (_isEnraged ? 1.2f : 1.0f);

            ClampMaxFallVelocity();
        }

        public override void OnSpawn()
        {
            _actionTimer = 3f;
            CurrentState = EnemyState.Chase;
            DisplayNameUI = new TextUI(
                new Rectangle(Singleton.SCREEN_WIDTH/4, (int)(Singleton.SCREEN_HEIGHT * 2.75f / 4), Singleton.SCREEN_WIDTH/2 , 30),
                Name,  
                1.75f,
                Color.White, 
                TextUI.TextAlignment.Center
            );
            HealthBar = new HealthBar(
                this,
                new Rectangle(Singleton.SCREEN_WIDTH/4, Singleton.SCREEN_HEIGHT * 3 / 4, Singleton.SCREEN_WIDTH/2 , 30),
                Color.Red,
                Color.Black
            );
            Singleton.Instance.CurrentUI.AddHUDElement(DisplayNameUI);
            Singleton.Instance.CurrentUI.AddHUDElement(HealthBar);
            base.OnSpawn();
        }
        
        private void Split(List<GameObject> gameObjects){
            if(_isSummoned)
                return;
            this._isSummoned =true;

            this._actionTimeOffset = 0.95f;
            GameObject newObject1 = (GameObject)this.Clone(); // Cloning the current object
            this._actionTimeOffset = 0.90f;
            GameObject newObject2 = (GameObject)this.Clone(); // Cloning the current object
            this._actionTimeOffset = 0.85f;

            newObject1.Position = new Vector2(this.Position.X + 150, this.Position.Y-100);
            newObject2.Position = new Vector2(this.Position.X - 150, this.Position.Y-100); 

            newObject1.Name = "Split Object"; // Give the new object a unique name if necessary
            newObject2.Name = "Split Object"; // Give the new object a unique name if necessary

            newObject1.Animation = new Animation(_texture, 96, 80, new Vector2(96*8, 80*5), 24);
            newObject1.Animation.AddAnimation("charge", new Vector2(0,2), 8);
            newObject1.Animation.AddAnimation("dash", new Vector2(3,3), 2);
            newObject1.Animation.AddAnimation("run", new Vector2(3,4), 3);
            newObject1.Animation.ChangeAnimation("charge");

            newObject2.Animation = new Animation(_texture, 96, 80, new Vector2(96*8, 80*5), 24);
            newObject2.Animation.AddAnimation("charge", new Vector2(0,2), 8);
            newObject2.Animation.AddAnimation("dash", new Vector2(3,3), 2);
            newObject2.Animation.AddAnimation("run", new Vector2(3,4), 3);
            newObject2.Animation.ChangeAnimation("charge");

            // Add the new object to the list of game objects
            gameObjects.Add(newObject1);
            gameObjects.Add(newObject2);
        }

        public override void OnDead(List<GameObject> gameObjects)
        {
            foreach (var cerberus in gameObjects.OfType<Cerberus>())
            {
                cerberus.CerberusCount--;
            }
            if(CerberusCount <= 0){
                Singleton.Instance.CurrentGameState = Singleton.GameState.StageCompleted;
                Singleton.Instance.CurrentUI.RemoveHUDElement(DisplayNameUI);
                Singleton.Instance.CurrentUI.RemoveHUDElement(HealthBar);
            }
            base.OnDead(gameObjects);
        }
    }
}
