using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Rhulk : BaseEnemy
    {
        private float _actionTimer = 0;     // Timer for aim/charge states
        private bool _isEnraged = false;    // Enrage phase flag
        public float Friction ;
        private Vector2 _spawnPoint;
        private float _chargeTime = 2.0f;
        private bool _isDashing;
        private float _dashTimer = 0f;
        private float _dashDuration = 2f;
        private float _beamchargingTime = 2.0f;
        private float _beamingTime = 10.0f; 
        private float _rotationAngle = 0f;
        private Vector2 _dashAim;
        private Vector2 _dashStart;
        
        private Vector2 _barrierstart;
        private Vector2 _barrierEnd ;
        private Vector2 _barrierEnd1;
        public DemonLaser Laserproj;

        public HealthBar HealthBar;
        
        public SoundEffect LaserSound;
        public SoundEffect DashSound;
        
        private int _lastSecondMark = -1;
        public Rhulk(Texture2D texture) : base(texture)
        { 
            _texture = texture;
            CanCollideTile = false;
        }
        public override void Reset()
        {
            _actionTimer = 0;     // Timer for aim/charge states
            _isEnraged = false;    // Enrage phase flag
            _isDashing =false ;
            _chargeTime = 2.0f;
            _dashTimer = 0f;
            _dashDuration = 2f;
            _beamchargingTime = 2.0f;
            _beamingTime = 10.0f; 
            _rotationAngle = 0f;
            IsIgnorePlatform = false;
            base.Reset();
        }

        public override void AddAnimation()
        {
            Animation = new Animation(_texture, 64, 96, new Vector2(64*6 , 96*3), 12);
            Animation.AddAnimation("idle", new Vector2(0, 0), 12);
            Animation.AddAnimation("run", new Vector2(0, 2), 6);
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
                case EnemyState.Floating:
                    AI_Floating(deltaTime,gameObjects, tileMap);
                    break;
                case EnemyState.Attack:
                    AI_Attack(deltaTime,gameObjects, tileMap);
                    break;
            }

            UpdateAnimation(deltaTime);
            base.Update(gameTime, gameObjects, tileMap);
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            string animation = "idle";

            if (Velocity.X != 0f)
            {
                animation = "run";
            }

            if(_currentAnimation != animation)
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
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            Vector2 Dir = Singleton.Instance.Player.Position - this.Position;
            _actionTimer -= deltaTime * (_isEnraged ? 1.5f : 1);
            if(Math.Abs(Vector2.Distance(Singleton.Instance.Player.Position , this.Position)) >20f){
               Dir.Normalize();
                Velocity = 50f * Dir;
                Velocity *= Friction;
            }


            if (_actionTimer <= 0)
            {
                // random attack pattern
                if(Singleton.Instance.Random.NextDouble() > 0.5f){
                    // start charging
                    CurrentState = EnemyState.Charging;
                    _actionTimer = _chargeTime;
                    _dashAim = Singleton.Instance.Player.GetPlayerCenter(); // Lock on to player's position
                }else{
                    CurrentState = EnemyState.Floating;
                    _actionTimer = _beamchargingTime;
                    //Teleport to Spawn Pos shoot lazer
                }
            }
        }

        private void AI_Charging(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
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
                DashSound.Play();
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
                    _actionTimer =3f;
                    _isJumping = false;
                }
            }
        }
        private void AI_Floating(float deltaTime,List<GameObject> gameObjects, TileMap tileMap){
            //aka charging beammmm
            //going to middle of arena
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            _actionTimer -= deltaTime * (_isEnraged ? 1.5f : 1);
            if(_actionTimer >0){
                float distance = Vector2.Distance(Position, _spawnPoint);
                if (distance <= 10f)
                {
                    Velocity = Vector2.Zero; // Stop movement when close enough
                    return; // Exit this block, no further movement updates
                }
                // Speed based on distance (closer = slower, farther = faster)
                float minSpeed = 50f;  // Minimum speed when very close
                float maxSpeed = 600f;  // Maximum speed when far away
                float speed = MathHelper.Clamp(distance * 3f, minSpeed, maxSpeed);
                Vector2 direction = _spawnPoint - Position;
                direction.Normalize(); // Convert to unit vector
                Velocity = direction * speed;
            }else{
                CurrentState = EnemyState.Attack;
                _actionTimer = _beamingTime;
                //Spawn Projectiles
                Console.WriteLine("Spawn Laser");
                int numLines = (int)Math.Clamp(MaxHealth/Health,1,3)+3; // Number of lines
                float angleOffset = MathHelper.TwoPi / numLines;
                for (int i = 0; i < numLines; i++){
                    DemonLaser laser = Laserproj.Clone() as DemonLaser;
                    laser.Shoot(Position, new Vector2(_rotationAngle+ (i * angleOffset),400f) );
                    gameObjects.Add(laser);
                }
            }
        }

        private void AI_Attack(float deltaTime,List<GameObject> gameObjects, TileMap tileMap){
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            _actionTimer -= deltaTime * (_isEnraged ? 1.5f : 1);
            int currentSecond = (int)Math.Floor(_actionTimer);
            if(_actionTimer>0){
                //playsound here 
                if (currentSecond != _lastSecondMark && currentSecond >= 0)
                {
                    _lastSecondMark = currentSecond;
                    LaserSound.Play();
                }
                //doing Attack
            }else{
                CurrentState = EnemyState.Chase;
                _actionTimer =3f;
            }
        }
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
            //DrawDebug(spriteBatch);
        }

        protected override void DrawDebug(SpriteBatch spriteBatch)
        {
            // Vector2 textPosition = new Vector2(Position.X, Position.Y - 40);
            // string displayText = $"State: {CurrentState}\n{Direction}\n HP {Health} \nAT:{_actionTimer}";
            // spriteBatch.DrawString(Singleton.Instance.GameFont, displayText, textPosition , Color.White);
            // if(CurrentState == EnemyState.Charging || CurrentState == EnemyState.Dash){
            //     DrawLine(spriteBatch, _dashAim, Position, Color.Green);
            //     // Draw 90-degree line at the Aim position
            //     DrawLine(spriteBatch,_barrierEnd,_barrierEnd1, Color.Blue);
            // }
            // else if(CurrentState == EnemyState.Floating || CurrentState == EnemyState.Attack){
            //     // spriteBatch.DrawString(Singleton.Instance.GameFont, "StartPos", textPosition , Color.White);
            //     // DrawRotatingLines(spriteBatch);
            // }
        }
        private void DrawRotatingLines(SpriteBatch spriteBatch)
        {
            int numLines = 5; // Number of lines
            float radius = 600f; // Length of each line
            float angleOffset = MathHelper.TwoPi / numLines; // 360 degrees / numLines
            Vector2 centerPos = this.Position + new Vector2(this.Rectangle.Width/2,this.Rectangle.Height/2);
            for (int i = 0; i < numLines; i++)
            {
                float angle = _rotationAngle + (i * angleOffset);
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                Vector2 endPosition = centerPos + direction;

                DrawLine(spriteBatch, centerPos, endPosition, Color.Red);
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
            _spawnPoint = this.Position;
            _actionTimer = 3f;
            CurrentState = EnemyState.Chase;
            HealthBar = new HealthBar(
                this,
                new Rectangle((Singleton.SCREEN_WIDTH - 200)/2, Singleton.SCREEN_HEIGHT * 5 / 6, 200, 30),
                Color.Red,
                Color.Gray
            );
            Singleton.Instance.CurrentUI.AddHUDElement(HealthBar);
            base.OnSpawn();
        }
        public override void OnDead(List<GameObject> gameObjects)
        {
            Singleton.Instance.CurrentGameState = Singleton.GameState.StageCompleted;
            Singleton.Instance.CurrentUI.RemoveHUDElement(HealthBar);
            base.OnDead(gameObjects);
        }
    }
}
