using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class GiantSlime : BaseEnemy
    {
        private int _jumpCounter = 0;       // Counts jumps before high jump
        private float _actionTimer = 0;     // Timer for aim/charge states
        private bool _isEnraged = false;    // Enrage phase flag
        public float Friction ;
        public float FloatDuration = 0.5f;   // Time floating above the player
        public float SlamChargeDuration = 0.5f;  // Charging before slam
        public float SlamSpeed = 1500f;
        public int JumpsBeforeHighJump = 3;   // Number of normal jumps before high jump

        public GiantSlime(Texture2D texture, Texture2D particleTexture, SpriteFont font) : base(texture, font) 
        { 
            // Animation = new Animation(texture, 48, 48, new Vector2(48*8 , 48*6), 12);
            // Animation.AddAnimation("Chase", new Vector2(0, 0), 8);
            // Animation.AddAnimation("charge", new Vector2(0, 1), 6);
            // Animation.AddAnimation("jump", new Vector2(0, 2), 5);
            // Animation.AddAnimation("float", new Vector2(0, 3), 1);
            // Animation.AddAnimation("land", new Vector2(0, 4), 5);
            // Animation.AddAnimation("die", new Vector2(0, 5), 7);
            // Animation.ChangeAnimation(_currentAnimation);
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

            if (!_isEnraged && Health <= MaxHealth * 0.2f) 
            {
                _isEnraged = true;
            }

            switch (CurrentState)
            {
                case EnemyState.Chase:
                    AI_Chase(deltaTime, gameObjects, tileMap);
                    break;
                case EnemyState.Jump:
                    AI_Jump(deltaTime, gameObjects, tileMap);
                    break;
                case EnemyState.Charging:
                    AI_Charging(deltaTime,gameObjects, tileMap);
                    break;
                case EnemyState.Dash:
                    AI_SlamCharge(deltaTime,gameObjects, tileMap);
                    break;
            }

            base.Update(gameTime, gameObjects, tileMap);
        }

        private void AI_Chase(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

            _actionTimer -= deltaTime * (_isEnraged ? 1.5f : 1);
            Velocity.X *= Friction;

            if (_actionTimer <= 0)
            {
                Console.WriteLine(_jumpCounter);
                if (_jumpCounter >= JumpsBeforeHighJump)
                {
                    Console.WriteLine("Going to High Jump");
                    CurrentState = EnemyState.Jump;  // Jump High
                    _actionTimer = 1f;
                }
                else
                {
                    Console.WriteLine("Normal Jump");
                    CurrentState = EnemyState.Jump;
                    _jumpCounter++;
                    _actionTimer = 1f;
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

                if (_jumpCounter >= JumpsBeforeHighJump) 
                {
                    Console.WriteLine(" going to AIM");
                    int horizontalDir = Math.Sign(Singleton.Instance.Player.Position.X - Position.X);
                    Velocity = new Vector2(horizontalDir * JumpStrength * 0.6f, -JumpStrength * 1.8f); // Jump higher
                    CurrentState = EnemyState.Charging;
                    CanCollideTile = false;// ignore all tile
                    _actionTimer = 3.0f;
                }
                else
                {
                    int horizontalDir = Math.Sign(Singleton.Instance.Player.Position.X - Position.X);
                    Velocity = new Vector2(horizontalDir * JumpStrength * 0.6f, -JumpStrength);
                }
            }

        }

        private void AI_Charging(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Console.WriteLine("Aimming");
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            // Console.WriteLine("Slime" + Position.Y + "Player pos " + Singleton.Instance.Player.GetPlayerCenter().Y );
            _actionTimer -= deltaTime * (_isEnraged ? 1.5f : 1);
            if(_actionTimer <=0 && Position.Y <= Singleton.Instance.Player.GetPlayerCenter().Y){
                Console.WriteLine("Do Slam");
                //go back to jump
                CurrentState = EnemyState.Dash;
                _actionTimer = SlamChargeDuration;
                _jumpCounter = 0;
            }else if(_actionTimer <=0){
                Console.WriteLine("Jump not high enough go back to chase state");
                //go back to jump
                CurrentState = EnemyState.Chase;
                CanCollideTile = true;
                _actionTimer =1f;
                _jumpCounter = 0;
            } else if(_actionTimer <=2 || (Position.Y <= Singleton.Instance.Player.GetPlayerCenter().Y -100 && this.Velocity.Y > 0)){
                if(_actionTimer >2)
                    _actionTimer = 2;
                Vector2 _AimTarget = Singleton.Instance.Player.GetPlayerCenter() + new Vector2(-this.Rectangle.Width/2,-100);
                float distance = Vector2.Distance(Position, _AimTarget);
                if (distance <= 10f)
                {
                    Velocity = Vector2.Zero; // Stop movement when close enough
                    return; // Exit this block, no further movement updates
                }
                // Speed based on distance (closer = slower, farther = faster)
                float minSpeed = 100f;  // Minimum speed when very close
                float maxSpeed = 600f;  // Maximum speed when far away
                float speed = MathHelper.Clamp(distance * 3f, minSpeed, maxSpeed);
                
                Vector2 direction = _AimTarget - Position;
                direction.Normalize(); // Convert to unit vector

                Velocity = direction * speed;
            } 
            else{
                ApplyGravity(deltaTime);
                Velocity.X *=Friction;
            }
        }

        private void AI_SlamCharge(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            Console.WriteLine("Slamming");
            _actionTimer -= deltaTime * (_isEnraged ? 1.5f : 1);
            CanCollideTile = true;
            if(_actionTimer>0){
                Velocity = new Vector2(0, SlamSpeed);
            }
            _jumpCounter = 0;
        }

        public override void OnLandVerticle()
        {
            _isJumping = false;

            if (CurrentState == EnemyState.Dash)
            {
                CurrentState = EnemyState.Chase;
                // _jumpCounter = 0;
            }
            else
            {
                CurrentState = EnemyState.Chase;
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
            base.Draw(spriteBatch);
            DrawDebug(spriteBatch);
        }

        protected override void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 40);
            Vector2 aim  = Singleton.Instance.Player.GetPlayerCenter() + new Vector2(-this.Rectangle.Width/2,-100);
            string displayText = $"State: {CurrentState}\nJumps: {_jumpCounter}/{JumpsBeforeHighJump}\nHP: {Health}/{MaxHealth}\nEnraged: {_isEnraged}";
            spriteBatch.DrawString(_DebugFont, displayText, textPosition, Color.White);
            spriteBatch.DrawString(_DebugFont, "AIM POS", aim, Color.White);
            
        }

        protected override void ApplyGravity(float deltaTime)
        {
            Velocity.Y += Singleton.GRAVITY * deltaTime * (_isEnraged ? 1.2f : 1.0f);
        }
        public override void OnSpawn()
        {
            CurrentState = EnemyState.Chase;
            base.OnSpawn();
        }

    }
}
