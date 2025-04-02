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
        private bool _CanCollideVerticlel;

        public HealthBar HealthBar;

        public GiantSlime(Texture2D texture, Texture2D particleTexture) : base(texture) 
        { 
            _texture = texture;
            CanCollideTile = true;
            _CanCollideVerticlel = true;
        }

        public override void AddAnimation(){
            Animation = new Animation(_texture, 96, 80, new Vector2(96*6 , 80*5), 24);;

            Animation.AddAnimation("idle", new Vector2(0, 0), 4);
            Animation.AddAnimation("charge", new Vector2(0, 1), 3);
            Animation.AddAnimation("jump", new Vector2(0, 2), 5);
            Animation.AddAnimation("float", new Vector2(5, 2), 1);
            Animation.AddAnimation("land", new Vector2(0, 3), 6);
            Animation.AddAnimation("fly", new Vector2(0, 4), 6);

            Animation.ChangeAnimation("idle");
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            string animation = "idle";

            if (_jumpCounter < JumpsBeforeHighJump)
            {
                if (_actionTimer < 0.75 && Velocity.Y == 0){
                    animation = "charge"; 
                }
                else if(Velocity.Y != 0){
                    animation = "jump";
                }
                else if (_actionTimer >= 1)
                    animation = "land"; ; 
            }
            else
            {
                animation = "fly";                
            }

            if(_currentAnimation != animation)
            {
                _currentAnimation = animation;
                switch (animation)
                {
                    case "jump" :
                        Animation.ChangeTransitionAnimation(_currentAnimation, "float");
                        break;
                    case "land" :
                        Animation.ChangeTransitionAnimation(_currentAnimation, "idle");
                        break;
                    case "charge": 
                    case "die":
                        Animation.ChangeAnimation(_currentAnimation);
                        Animation.IsLooping = false;
                        break;
                    default:
                        Animation.ChangeAnimation(_currentAnimation);
                        break;
                }    
            }
            
            base.UpdateAnimation(deltaTime);
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

            UpdateAnimation(deltaTime);
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
                    CanCollideTile = true;// ignore all tile
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
                _CanCollideVerticlel =true;
                _actionTimer =1f;
                _jumpCounter = 0;
            } else if(_actionTimer <=2 || (Position.Y <= Singleton.Instance.Player.GetPlayerCenter().Y -100 && this.Velocity.Y > 0)){
                if(_actionTimer >2)
                    _actionTimer = 2;
                CanCollideTile =true;
                _CanCollideVerticlel =false;
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
            CanCollideTile =true;
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
            Console.WriteLine("Slamming");
            _actionTimer -= deltaTime * (_isEnraged ? 1.5f : 1);
            if(Position.Y + _texture.Height> Singleton.Instance.Player.GetPlayerCenter().Y){
                _CanCollideVerticlel =true;
            }
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
            Color color = IsInvincible() ? Color.Red : Color.White;

            spriteBatch.Draw(
                Animation.GetTexture(),
                GetDrawingPosition(),
                Animation.GetCurrentFrame(),
                color,
                0f, 
                Vector2.Zero,
                Scale,
                SpriteEffects.None, 
                0f
            );
            
            DrawDebug(spriteBatch);
        }

        protected override void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 40);
            Vector2 aim  = Singleton.Instance.Player.GetPlayerCenter() + new Vector2(-this.Rectangle.Width/2,-100);
            string displayText = $"State: {CurrentState}\nJumps: {CanCollideTile}\nVert: {_CanCollideVerticlel}";
            spriteBatch.DrawString(Singleton.Instance.GameFont, displayText, textPosition, Color.White);
            spriteBatch.DrawString(Singleton.Instance.GameFont, "AIM POS", aim, Color.White);
            
        }

        protected override void ApplyGravity(float deltaTime)
        {
            Velocity.Y += Singleton.GRAVITY * deltaTime * (_isEnraged ? 1.2f : 1.0f);
        }
        public override void OnSpawn()
        {
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
        protected override void UpdateHorizontalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position.X += Velocity.X * deltaTime;
            if(!CanCollideTile) 
                return;

            int radius = 5;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    Vector2 newPosition = new(Position.X + i * Singleton.TILE_SIZE, Position.Y + j * Singleton.TILE_SIZE);
                    Tile tile = tileMap.GetTileAtWorldPostion(newPosition);
                    if(tile != null && (tile.Type == TileType.Barrier ) && tile.Type != TileType.Platform)
                    {
                        if(ResolveHorizontalCollision(tile)){
                            OnCollisionHorizon();
                        }
                    }
                }
            }
        }
        protected override void UpdateVerticalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position.Y += Velocity.Y * deltaTime;
            if(!CanCollideTile) 
                return;
            int radius = 5;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    Vector2 newPosition = new(Position.X + i * Singleton.TILE_SIZE, Position.Y + j * Singleton.TILE_SIZE);
                    Tile tile = tileMap.GetTileAtWorldPostion(newPosition);
                    if(tile != null && (tile.Type == TileType.Barrier || (tile.Type == TileType.Platform && !CanDropThroughPlatform(tile)))  && tile.Type != TileType.Platform)
                    {
                        if(_CanCollideVerticlel){
                            float VeloY = Velocity.Y;
                            if(ResolveVerticalCollision(tile)){
                                Console.WriteLine(VeloY);
                                if(!(VeloY <0))//jump up and hit wall will not do anything
                                    OnLandVerticle();
                            }
                        }
                    }
                }
            }
        }

    }
}
