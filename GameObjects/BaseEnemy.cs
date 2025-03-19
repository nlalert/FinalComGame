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

        // Spawn and Death Tracking
        public Player player {get; set;}
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
        public override void OnHitByProjectile(GameObject projectile,float damageAmount)
        {
            //we have 0 projectiles
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
        /// <summary>
        /// This npc physically contact with Player
        /// </summary>
        /// <param name="player">Player Character</param>
        public virtual void OnCollidePlayer(Player player)
        {
            player.OnCollideNPC(this,this.attackDamage);
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
            if(HasSpawned == false)
                return;
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
                foreach (Tile tile in tileMap.tiles)
                {
                    if(ResolveHorizontalCollision(tile)){
                        OnCollisionHorizon();
                    }
                }
            }
        }
        public virtual bool CheckContactPlayer(){
            if(this.IsTouching(player)){
                OnCollidePlayer(player);
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
    }
}
