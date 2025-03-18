using FinalComGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FinalComGame {
    abstract class BaseEnemy : Character
    {
        // Enemy States
        public enum EnemyState
        {
            Idle,
            Patrol,
            Chase,
            Attack,
            Cooldown,
            Dying,
            Dead
        }

        // Enemy Properties
        public Rectangle Hitbox { get; protected set; }
        public EnemyState CurrentState { get; protected set; }
        
        // Movement Properties
        protected float chaseSpeed = 3f;
        protected float patrolBoundaryLeft;
        protected float patrolBoundaryRight;

        // Combat Properties
        protected float detectionRange = 200f;
        protected float attackRange = 50f;

        // Reference to player for tracking
        protected Player player;

        // Spawn and Death Tracking
        public bool CanCollideTile {get;set;} =false;
        public bool HasSpawned { get; protected set; } = false;
        public bool IsDead() => CurrentState == EnemyState.Dead;
        
        protected SpriteFont _DebugFont;
        public BaseEnemy(Texture2D texture,SpriteFont font){
            _DebugFont = font;

            _idleAnimation = new Animation(texture, 16, 32, 1, 24); // 24 fps\
            Animation = _idleAnimation;

            //remove later
            _texture = texture;
        }
        
        // Spawn method with optional spawn effects
        public virtual void Spawn(float x, float y, List<GameObject> gameObjects)
        {
            BaseEnemy newEnemy = (BaseEnemy)this.Clone(); // self clone 
            newEnemy.Position = new Vector2(x, y);
            newEnemy.patrolBoundaryLeft = x - 100f;
            newEnemy.patrolBoundaryRight = x + 100f;
            newEnemy.Health = maxHealth;
            newEnemy.HasSpawned = true;
            newEnemy.IsActive =true;
            gameObjects.Add(newEnemy);
            newEnemy.OnSpawn();
        }

        public virtual bool CanBeHitByPlayer()
        {
            // Determines if enemy can be hit by player
            // Can be overridden for specific enemy types
            return CurrentState != EnemyState.Dead && 
                CurrentState != EnemyState.Dying;
        }

        public override void OnHit(GameObject projectile,float damageAmount)
        {
            //TODO: deal with projectile later
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
                CurrentState = EnemyState.Dying;
                OnDead();
            }
        }

        public virtual void OnHitPlayer()
        {
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap){
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(HasSpawned == false)
                    return;
            if(CurrentState == EnemyState.Dead || CurrentState == EnemyState.Dying){
                this.IsActive = false;
            }
            
            UpdateInvincibilityTimer(deltaTime);

            if(CanCollideTile){
                ResolveTileCollision(deltaTime,gameObjects,tileMap);
            }
            UpdateAnimation(deltaTime);
            base.Update(gameTime,gameObjects, tileMap);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(HasSpawned == false)
                return;
            spriteBatch.Draw(_texture, Position, Viewport, Color.White);
            base.Draw(spriteBatch);
        }

        // Protected helper methods
        public override void Reset()
        {
            base.Reset();
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            //TODO : add more animation
            Animation = _idleAnimation;
            base.UpdateAnimation(deltaTime);
        }
        
        private void ResolveTileCollision(float deltaTime, List<GameObject> gameObjects, TileMap tileMap){
            float newX = Position.X + Velocity.X * deltaTime;
            float newY = Position.Y + Velocity.Y * deltaTime;
            // Store original position 
            Vector2 originalPosition = Position;
            // Temporarily update position to check for future collisions
            Position = new Vector2(newX, newY);
            bool collisionDetected = false;
            // Check collisions at the new position
            foreach (Tile t in tileMap.tiles)
            {
                if (t.IsSolid)
                {
                    // Check ground collision (bottom of NPC touching top of tile)
                    if (IsTouchingTop(t))
                    {
                        collisionDetected = true;
                        Velocity = new Vector2(Velocity.X, 0); // Stop vertical movement
                        newY = t.Rectangle.Top - this.Rectangle.Height; // Position right on top of the tile
                        break;
                    }
                    
                    // You could add other collision checks here if needed:
                    // Check ceiling collision (top of NPC touching bottom of tile)
                    if (IsTouchingBottom(t))
                    {
                        collisionDetected = true;
                        Velocity = new Vector2(Velocity.X, 0); // Stop vertical movement
                        newY = t.Rectangle.Bottom; // Position right below the tile
                        break;
                    }
                    
                    // Check left wall collision
                    if (IsTouchingLeft(t))
                    {
                        collisionDetected = true;
                        Velocity = new Vector2(0, Velocity.Y); // Stop horizontal movement
                        newX = t.Rectangle.Left - this.Rectangle.Width; // Position to the left of the tile
                        break;
                    }
                    
                    // Check right wall collision
                    if (IsTouchingRight(t))
                    {
                        collisionDetected = true;
                        Velocity = new Vector2(0, Velocity.Y); // Stop horizontal movement
                        newX = t.Rectangle.Right; // Position to the right of the tile
                        break;
                    }
                }
            }

            // Restore original position (collision check was just a simulation)
            Position = originalPosition;
        
            // Now actually move to the valid position
            Position = new Vector2(newX, newY);
        }

        public virtual void CheckHit(Rectangle attackHitbox, float damageAmount)
        {
            if(IsTouching(attackHitbox))
            {
                OnHit(damageAmount);
            }
        }
    }
}
