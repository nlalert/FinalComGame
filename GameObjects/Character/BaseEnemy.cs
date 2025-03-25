using FinalComGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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
            Dead
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
        
        protected SpriteFont _DebugFont;
        public BaseEnemy(Texture2D texture,SpriteFont font) : base(texture){
            _DebugFont = font;

            _idleAnimation = new Animation(texture, 16, 32, 1, 24); // 24 fps\
            Animation = _idleAnimation;
        }
        
        // Spawn method with optional spawn effects
        public virtual void Spawn(float x, float y, List<GameObject> gameObjects)
        {
            BaseEnemy newEnemy = (BaseEnemy)this.Clone(); // self clone 
            newEnemy.Position = new Vector2(x, y);
            newEnemy._patrolBoundaryLeft = x - 100f;
            newEnemy._patrolBoundaryRight = x + 100f;
            gameObjects.Add(newEnemy);
            newEnemy.OnSpawn();
        }
        public virtual void Spawn(Vector2 position, List<GameObject> gameObjects)
        {
            BaseEnemy newEnemy = (BaseEnemy)this.Clone(); // self clone 
            newEnemy.Position = position;
            newEnemy._patrolBoundaryLeft = position.X - 100f;
            newEnemy._patrolBoundaryRight = position.X + 100f;
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
        public override void OnHitByProjectile(GameObject projectile,float damageAmount)
        {
            //we have 0 projectiles
            OnHit(damageAmount);
        }
        public override void OnHit(float damageAmount)
        {
            if (_invincibilityTimer > 0) 
                return; // If i-frames are active, ignore damage
            // Generic hit handling
            Health -= damageAmount;
            StartInvincibility();
            Console.WriteLine("Damage " + damageAmount + " CurHP" + Health);
            if (Health <= 0)
            {
                CurrentState = EnemyState.Dying;
                OnDead();
            }
        }
        /// <summary>
        /// This npc physically contact with Player
        /// </summary>
        /// <param name="player">Player Character</param>
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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Viewport, Color.White);
            base.Draw(spriteBatch);
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            //TODO : add more animation
            Animation = _idleAnimation;
            base.UpdateAnimation(deltaTime);
        }

        protected override void UpdateHorizontalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position.X += Velocity.X * deltaTime;
            if(CanCollideTile)
            {
                foreach (Tile tile in tileMap.tiles.Values)
                {
                    if(tile.IsSolid){
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
            foreach (Tile tile in tileMap.tiles.Values)
            {
                if(tile.IsSolid){
                    if(ResolveVerticalCollision(tile)){
                        OnLandVerticle();
                    }
                } 
            }
        }
        public virtual bool CheckContactPlayer(){
            if(this.IsTouching(Singleton.Instance.Player)){
                OnCollidePlayer();
                Console.WriteLine("contact Player");
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

            _isAttacking = false;
            _isJumping = false;

            _invincibilityTimer = 0f;
            _attackTimer = 0f;
            _attackCooldownTimer = 0f;
            
            base.Reset();
        }
    }
}
