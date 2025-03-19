using FinalComGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FinalComGame {
    public class Character : GameObject
    {
        public float Health { get; protected set; }
        public float maxHealth = 100f;
        public int WalkSpeed;
        protected int direction = 1; // 1 = Right, -1 = Left

        // i-frame
        protected float invincibilityDuration = 0.5f; // 0.5 seconds of i-frames
        protected float invincibilityTimer = 0f;

        //Attack
        protected bool isAttacking = false;
        protected float attackDamage = 10f;
        protected float attackDuration = 0.4f; // How long the attack lasts
        protected float attackCooldown = 0.2f; // Cooldown before attacking again
        protected float attackTimer = 0f;
        protected float attackCooldownTimer = 0f;
        protected Rectangle attackHitbox;
        
        //Jump
        protected bool isJumping = false;
        protected float jumpStrength = 800f;

        //Animation
        protected Animation _idleAnimation;
        protected Animation _runAnimation;

        public override void Draw(SpriteBatch spriteBatch)
        {
            // hitbox debug drawing
            Texture2D debugTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            debugTexture.SetData(new Color[] { Color.Red });

            if (attackTimer > 0)
                spriteBatch.Draw(debugTexture, attackHitbox, Color.Red);
            // end hitbox debug drawing

            SpriteEffects spriteEffect = direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(
                Animation.GetTexture(),
                GetDrawingPosition(),
                Animation.GetCurrentFrame(),
                Color.White,
                0f, 
                Vector2.Zero,
                1f,
                spriteEffect, 
                0f
            );

            base.Draw(spriteBatch);
        }

        protected Vector2 GetDrawingPosition()
        {
            return  new Vector2( Position.X - (Animation.GetFrameSize().X - Viewport.Width) / 2 
                                ,Position.Y - (Animation.GetFrameSize().Y - Viewport.Height) / 2);
        }

        protected void ApplyGravity(float deltaTime)
        {
            Velocity.Y += Singleton.GRAVITY * deltaTime; // gravity

            //TODO: Check and cap terminal velocity of player if want later
        }

        protected virtual void UpdateHorizontalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position.X += Velocity.X * deltaTime;
            foreach (Tile tile in tileMap.tiles)
            {
                if(tile.IsSolid) ResolveHorizontalCollision(tile);
            }
        }

        protected virtual void UpdateVerticalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position.Y += Velocity.Y * deltaTime;
            foreach (Tile tile in tileMap.tiles)
            {
                if(tile.IsSolid) ResolveVerticalCollision(tile);
            }
        }

        protected bool IsOnGround()
        {
            //TODO apex of jump is grounded?
            return Velocity.Y == 0;
        }

        protected virtual void UpdateAnimation(float deltaTime)
        {
            Animation?.Update(deltaTime);
        }

        protected void UpdateInvincibilityTimer(float deltaTime)
        {
            if (invincibilityTimer > 0)
                invincibilityTimer -= deltaTime;
        }
        /// <summary>
        /// Npc collide with any NPC 
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="damageAmount"></param>
        public virtual void OnCollideNPC(Character npc,float damageAmount){
        }
        /// <summary>
        /// Calls this when enemy get hit by any projectiles
        /// </summary>
        public virtual void OnHitByProjectile(GameObject projectile, float damageAmount){
        }

        /// <summary>
        /// This call when Character recive damage
        /// </summary>
        public virtual void OnHit(float damageAmount)
        {
        }
        /// <summary>
        /// Do anything special when spawn
        /// </summary>
        public virtual void OnSpawn()
        {
        }
        /// <summary>
        /// Do anything specail when dead
        /// </summary>
        public virtual void OnDead()
        {
        }

        public void StartInvincibility()
        {
            invincibilityTimer = invincibilityDuration; // Activate i-frames
        }
    }
}
