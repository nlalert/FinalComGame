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
        public EnemyState CurrentState { get; protected set; }
        
        // Movement Properties
        protected float _patrolBoundaryLeft;
        protected float _patrolBoundaryRight;

        // Combat Properties
        public float DetectionRange = 200f;
        public float AttackRange = 50f;

        // Spawn and Death Tracking
        public bool CanCollideTile;
        public bool IsIgnorePlatform = false;

        public Dictionary<ItemID, float> LootTableChance;

        public BaseEnemy(Texture2D texture) : base(texture){
            _idleAnimation = new Animation(texture, Singleton.TILE_SIZE, Singleton.TILE_SIZE * 2, new Vector2(1,1), 24); // 24 fps\
            Animation = _idleAnimation;
            _invincibilityDuration = 0.05f;
        }
        
        public BaseEnemy Spawn(Vector2 position, List<GameObject> gameObjects)
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
        public override void OnHitByProjectile(GameObject projectile,float damageAmount, bool isHeavyAttack)
        {
            //we have 0 projectiles
            OnHit(damageAmount, isHeavyAttack);
        }
        public override void OnHit(float damageAmount, bool IsHeavyAttack)
        {
            if (CanBeHitByPlayer()) 
                return; // If i-frames are active, ignore damage
            // Generic hit handling
            Health -= damageAmount;
            HitSound?.Play();
            StartInvincibility(IsHeavyAttack);
            //Console.WriteLine("Damage " + damageAmount + " CurHP" + Health);
        }

        public virtual void OnCollidePlayer()
        {
            Singleton.Instance.Player.OnCollideNPC(this, this.AttackDamage);
        }

        public override void OnDead(List<GameObject> gameObjects)
        {
            DropItem(gameObjects);
            base.OnDead(gameObjects);
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap){
            //if touchting player do contact dmg
            CheckContactPlayer();

            base.Update(gameTime,gameObjects, tileMap);

            if (Health <= 0)
            {
                CurrentState = EnemyState.Dying;
                OnDead(gameObjects);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color color = IsInvincible() ? Color.Red : Color.White;
            //spriteBatch.Draw(_texture, Position, Viewport, color);

            if (Animation._animationName.Count > 0){
                base.Draw(spriteBatch);
            }
            
        }

        protected override void UpdateHorizontalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position.X += Velocity.X * deltaTime;
            if(!CanCollideTile) 
                return;

            foreach (Vector2 offset in _collisionOffsets)
            {
                Vector2 checkPosition = new Vector2(Position.X + offset.X, Position.Y + offset.Y);
                Tile tile = tileMap.GetTileAtWorldPostion(checkPosition);
                
                if (tile != null && (tile.Type == TileType.Barrier || tile.Type == TileType.AmbushBarrier 
                    || (tile.Type == TileType.Platform && !CanDropThroughPlatform(tile))))
                {
                    if (ResolveHorizontalCollision(tile))
                    {
                        OnCollisionHorizon();
                    }
                }
            }
        }

        protected override void UpdateVerticalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position.Y += Velocity.Y * deltaTime;
            if(!CanCollideTile) 
                return;

            foreach (Vector2 offset in _collisionOffsets)
            {
                Vector2 checkPosition = new Vector2(Position.X + offset.X, Position.Y + offset.Y);
                Tile tile = tileMap.GetTileAtWorldPostion(checkPosition);
                
                if(tile != null && (tile.Type == TileType.Barrier || (tile.Type == TileType.Platform && !CanDropThroughPlatform(tile))))
                {
                    if(ResolveVerticalCollision(tile)){
                        OnLandVerticle();
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
            return (IsAbovePlayer() && IsPlayerAbovePlatform(tile)) || Velocity.Y < 0 || IsIgnorePlatform;
        }

        public virtual void CheckContactPlayer(){
            if(IsTouching(Singleton.Instance.Player))
                OnCollidePlayer();
        }
        public virtual void CheckHit(Rectangle attackHitbox, float damageAmount, bool isHeavyAttack)
        {
            if(IsTouching(attackHitbox))
            {
                OnHit(damageAmount, isHeavyAttack);
            }
        }
        public virtual void DropItem(List<GameObject> gameObjects)
        {
            ItemManager.RandomSpawnItem(LootTableChance, Position, gameObjects);
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
        protected bool HaveLineOfSightOfPlayer(TileMap tileMap){
            if (!IsWithinDetectionRange())
                return false;
            
            float step = Singleton.TILE_SIZE; // Tile size or step size for checking
            Vector2 checkPosition = Position;
            Vector2 playerPosition = Singleton.Instance.Player.GetPlayerCenter();
            Vector2 direction = Vector2.Normalize(playerPosition - checkPosition);

            while (Vector2.Distance(checkPosition, playerPosition) > step)
            {
                checkPosition += direction * step;
                Tile tile = tileMap.GetTileAtWorldPostion(checkPosition);
                if (tile != null && tile.IsSolid)
                {
                    return false; // Blocked by an obstacle
                }
            }
            return true;
        }

        protected bool IsWithinDetectionRange()
        {
            float distanceToPlayer = Vector2.Distance(Position, Singleton.Instance.Player.GetPlayerCenter());
            
            return distanceToPlayer <= DetectionRange;
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
