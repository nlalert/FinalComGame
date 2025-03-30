using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Rhulk : BaseEnemy
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
        
        private Vector2 _dashAim;
        private Vector2 _dashStart;
        
        private Vector2 _barrierstart;
        private Vector2 _barrierEnd ;
        private Vector2 _barrierEnd1;

        public Rhulk(Texture2D texture, Texture2D particleTexture, SpriteFont font) : base(texture, font) 
        { 
            // Animation = new Animation(texture, 48, 48, new Vector2(48*8 , 48*6), 12);
            // Animation.AddAnimation("Chase", new Vector2(0, 0), 8);
            // Animation.AddAnimation("charge", new Vector2(0, 1), 6);
            // Animation.AddAnimation("jump", new Vector2(0, 2), 5);
            // Animation.AddAnimation("float", new Vector2(0, 3), 1);
            // Animation.AddAnimation("land", new Vector2(0, 4), 5);
            // Animation.AddAnimation("die", new Vector2(0, 5), 7);
            // Animation.ChangeAnimation(_currentAnimation);

            CanCollideTile = true;
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
                    AI_Chase(deltaTime, gameObjects, tileMap);
                    break;
                case EnemyState.Charging:
                    AI_Charging(deltaTime,gameObjects, tileMap);
                    break;
                case EnemyState.Dash:
                    AI_Dash(deltaTime,gameObjects, tileMap);
                    break;
            }

            base.Update(gameTime, gameObjects, tileMap);
        }

        private void AI_Chase(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

            _actionTimer -= deltaTime * (_isEnraged ? 1.5f : 1) * _actionTimeOffset;
            if(Math.Abs((Singleton.Instance.Player.Position - this.Position).X) >20f)
                Direction = Math.Sign( (Singleton.Instance.Player.Position - this.Position).X);
            Velocity.X = 60f * Direction;
            Velocity.X *= Friction;


            if (_actionTimer <= 0)
            {
                // random attack pattern
                // if(false){
                CurrentState = EnemyState.Charging;
                _actionTimer = _chargeTime;
                _dashAim = Singleton.Instance.Player.GetPlayerCenter(); // Lock on to player's position
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
                _barrierstart = Singleton.Instance.Player.GetPlayerCenter() - direction * 100f;
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
            base.OnCollisionHorizon();
        }
        public override void OnLandVerticle()
        {
            _isJumping = false;
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
            base.Draw(spriteBatch);
            DrawDebug(spriteBatch);
        }

        protected override void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 40);
            string displayText = $"State: {CurrentState}\n{Direction}\n HP {Health} \nAT:{_actionTimer}";
            spriteBatch.DrawString(_DebugFont, displayText, textPosition , Color.White);
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

            // Create a 1x1 pixel texture if you don't already have one
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });

            // Draw the line (scaled 1x1 texture)
            spriteBatch.Draw(pixel, start, null, color, (float)Math.Atan2(direction.Y, direction.X), Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        protected override void ApplyGravity(float deltaTime)
        {
            Velocity.Y += Singleton.GRAVITY * deltaTime * (_isEnraged ? 1.2f : 1.0f);
        }

        public override void OnSpawn()
        {
            _actionTimer = 3f;
            CurrentState = EnemyState.Chase;
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
            // Add the new object to the list of game objects
            gameObjects.Add(newObject1);
            gameObjects.Add(newObject2);
        }
    }
}
