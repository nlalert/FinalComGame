using FinalComGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace FinalComGame {
    public abstract class BaseEnemy : Character
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
            Charging,
            Dash,
            Jump,
            Dead,
            Floating
        }

        // Enemy Properties
        public Rectangle Hitbox { get; protected set; }
        public EnemyState CurrentState { get; protected set; }
        
        // Movement Properties
        protected float ChaseSpeed;
        protected float _patrolBoundaryLeft;
        protected float _patrolBoundaryRight;

        // Combat Properties
        public float DetectionRange = 200f;
        public float AttackRange = 50f;

        // Spawn and Death Tracking
        public bool CanCollideTile;
        public bool IsDead() => CurrentState == EnemyState.Dead;
        
        public BaseEnemy(Texture2D texture) : base(texture){
            _idleAnimation = new Animation(texture, 16, 32, new Vector2(1,1), 24); // 24 fps\
            Animation = _idleAnimation;
        }
        
        // Spawn method with optional spawn effects
        public virtual void Spawn(float x, float y, List<GameObject> gameObjects, List<BaseEnemy> spawnedEnemies)
        {
            Vector2 position = new Vector2(x, y);
            Spawn(position, gameObjects, spawnedEnemies);
        }

        public virtual BaseEnemy Spawn(Vector2 position, List<GameObject> gameObjects)
        {
            BaseEnemy newEnemy = (BaseEnemy)this.Clone(); // self clone 
            newEnemy.Position = position;
            newEnemy._patrolBoundaryLeft = position.X - 100f;
            newEnemy._patrolBoundaryRight = position.X + 100f;
            newEnemy.Reset();
            gameObjects.Add(newEnemy);

            newEnemy.OnSpawn();

            return newEnemy;
        }

        public virtual void Spawn(Vector2 position, List<GameObject> gameObjects, List<BaseEnemy> spawnedEnemies)
        {
            BaseEnemy newEnemy = Spawn(position, gameObjects);
            spawnedEnemies.Add(newEnemy);
        }

        public virtual void AddAnimation(){
        }

        public virtual bool CanBeHitByPlayer()
        {
            // Determines if enemy can be hit by player
            // Can be overridden for specific enemy types
            return CurrentState != EnemyState.Dead && 
                CurrentState != EnemyState.Dying &&
                IsInvincible();
        }
        public override void OnHitByProjectile(GameObject projectile,float damageAmount)
        {
            //we have 0 projectiles
            OnHit(damageAmount);
        }
        public override void OnHit(float damageAmount)
        {
            if (CanBeHitByPlayer()) 
                return; // If i-frames are active, ignore damage
            // Generic hit handling
            Health -= damageAmount;
            HitSound?.Play();
            StartInvincibility();
            //Console.WriteLine("Damage " + damageAmount + " CurHP" + Health);
        }

        public virtual void OnCollidePlayer()
        {
            Singleton.Instance.Player.OnCollideNPC(this,this.AttackDamage);
        }
        public override void OnCollideNPC(Character npc, float damageAmount)
        {   
            base.OnCollideNPC(npc, damageAmount);
        }
        public override void OnDead()
        {
            DropItem();
            base.OnDead();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap){
            //if touchting player do contact dmg
            CheckContactPlayer();

            base.Update(gameTime,gameObjects, tileMap);

            if (Health <= 0)
            {
                CurrentState = EnemyState.Dying;
                OnDead();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color color = IsInvincible() ? Color.HotPink : Color.White;
            spriteBatch.Draw(_texture, Position, Viewport, color);

            if (Animation._animationName.Count > 0){
                base.Draw(spriteBatch);
            }
            
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            //TODO : add more animation
            base.UpdateAnimation(deltaTime);
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
                    if(tile != null && (tile.Type == TileType.Barrier || (tile.Type == TileType.Platform && !CanDropThroughPlatform(tile))))
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
                    if(tile != null && (tile.Type == TileType.Barrier || (tile.Type == TileType.Platform && !CanDropThroughPlatform(tile))))
                    {
                        if(ResolveVerticalCollision(tile)){
                            OnLandVerticle();
                        }
                    }
                }
            }
        }

        public virtual bool IsAbovePlayer(){
            return Position.Y < Singleton.Instance.Player.Position.Y;
        }

        public virtual bool IsPlayerAbovePlatform(Tile tile){
            return Position.Y <= tile.Position.Y;
        }

        public virtual bool CanDropThroughPlatform(Tile tile){
            return (IsAbovePlayer() && IsPlayerAbovePlatform(tile)) || Velocity.Y < 0;
        }

        public virtual bool CheckContactPlayer(){
            if(this.IsTouching(Singleton.Instance.Player)){
                OnCollidePlayer();
                // Console.WriteLine("contact Player");
                return true;
            }
            else
                return false;
        }
        public virtual void CheckHit(Rectangle attackHitbox, float damageAmount)
        {
            if(IsTouching(attackHitbox))
            {
                OnHit(damageAmount);
            }
        }
        public virtual void DropItem()
        {
        }
        public virtual void OnCollisionHorizon(){

        }
        public virtual void OnLandVerticle(){

        }
        /// <summary>
        /// Enemy look for player with line of sight
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool HaveLineOfSight(TileMap tileMap){
            if (Singleton.Instance.Player == null) return false;
            
            Vector2 enemyPosition = Position;
            Vector2 playerPosition = Singleton.Instance.Player.GetPlayerCenter();
            
            float step = 16f; // Tile size or step size for checking
            Vector2 direction = Vector2.Normalize(playerPosition - enemyPosition);
            Vector2 checkPosition = enemyPosition;

            while (Vector2.Distance(checkPosition, playerPosition) > step)
            {
                checkPosition += direction * step;
                if (tileMap.IsObstacle(checkPosition))
                {
                    return false; // Blocked by an obstacle
                }
            }
            return true;
        }

        public override void Reset()
        {
            Direction = -1; // Reset direction to left

            Health = MaxHealth;
            AttackDamage = BaseAttackDamage;
            ResetJumpStrength();
            _isAttacking = false;
            _isJumping = false;

            _invincibilityTimer = 0f;
            _attackTimer = 0f;
            _attackCooldownTimer = 0f;

            AddAnimation();
            _currentAnimation = "idle";

            CurrentState = EnemyState.Idle;
            
            base.Reset();
        }
    }
}
