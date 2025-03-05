using FinalComGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FinalComGame {
abstract class BaseEnemy : GameObject
{
    // Enemy States
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Cooldown,
        TakeDamage,
        Dying,
        Dead
    }

    // Enemy Properties
    public Rectangle Hitbox { get; protected set; }
    public EnemyState CurrentState { get; protected set; }
    
    // Movement Properties
    protected float patrolSpeed = 2f;
    protected float chaseSpeed = 3f;
    protected bool movingRight = true;
    protected float patrolBoundaryLeft;
    protected float patrolBoundaryRight;

    // Combat Properties
    public float Health { get; protected set; }
    protected float maxHealth = 100f;
    protected float attackDamage = 10f;
    protected float detectionRange = 200f;
    protected float attackRange = 50f;
    protected float attackCooldown = 2f;
    protected float currentCooldown = 0f;

    // Reference to player for tracking
    protected Player player;

    // Spawn and Death Tracking
    public bool HasSpawned { get; protected set; } = false;
    public bool IsDead() => CurrentState == EnemyState.Dead;

    public BaseEnemy(Texture2D texture) : base(texture){

    }
    // Spawn method with optional spawn effects
    public virtual void Spawn(float x, float y)
    {
        Position = new Vector2(x, y);
        patrolBoundaryLeft = x - 100f;
        patrolBoundaryRight = x + 100f;
        Health = maxHealth;
        HasSpawned = true;

        // Call spawn effect method
        OnSpawn();
    }

    // Virtual methods for extensibility
    public virtual void OnSpawn()
    {
        // Override for specific spawn effects (e.g., particle effects, sound)
        Console.WriteLine($"Enemy spawned at {Position}");
    }

    public virtual void OnDead()
    {
        // Override for death effects (e.g., decay animation, particle effects)
        Console.WriteLine($"Enemy died at {Position}");
    }

    public virtual bool CanHitByPlayer()
    {
        // Determines if enemy can be hit by player
        // Can be overridden for specific enemy types
        return CurrentState != EnemyState.Dead && 
               CurrentState != EnemyState.Dying;
    }

    public virtual void OnHit(float damageAmount)
    {
        // Generic hit handling
        Health -= damageAmount;

        if (Health <= 0)
        {
            CurrentState = EnemyState.Dying;
            OnDead();
        }
        else
        {
            CurrentState = EnemyState.TakeDamage;
        }
    }

    public virtual void OnHitPlayer()
    {
        // Check collding in playscene with these code
        // foreach (var enemy in enemies)
        // {
        //     enemy.Update(gameTime);

        //     // Check for player collision
        //     if (IsColliding(enemy.Hitbox, player.Hitbox))
        //     {
        //         // Call OnHitPlayer when enemy and player hitboxes overlap
        //         enemy.OnHitPlayer();
        //     }
        // }
        // Default behavior when enemy touches player
        // Can be overridden for different enemy types
        
        // player.TakeDamage(attackDamage);
    }

    public override void Update(GameTime gameTime, List<GameObject> gameObjects){
        base.Update(gameTime,gameObjects);
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, Viewport, Color.White);
        base.Draw(spriteBatch);
    }

        // Protected helper methods
    protected virtual void UpdateHitbox()
    {
        // Update hitbox based on current position and sprite size
        Hitbox = new Rectangle(
            (int)Position.X, 
            (int)Position.Y, 
            32,  // Assume 32x32 sprite
            32
        );
    }
        public override void Reset()
        {
            base.Reset();
        }
    }
}
