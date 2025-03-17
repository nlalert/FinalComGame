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
        protected float attackDuration = 0.2f; // How long the attack lasts
        protected float attackCooldown = 0.5f; // Cooldown before attacking again
        protected float attackTimer = 0f;
        protected float attackCooldownTimer = 0f;
        protected Rectangle attackHitbox;

        
        //Jump
        protected bool isJumping = false;
        protected float jumpStrength = 1000f;

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
                Position,
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

        protected void ApplyGravity(float deltaTime)
        {
            Velocity.Y += Singleton.GRAVITY * deltaTime; // gravity

            //TODO: Check and cap terminal velocity of player if want later
        }

        protected bool IsOnGround()
        {
            //TODO apex of jump is grounded?
            return Velocity.Y == 0;
        }

        protected virtual void UpdateAnimation(float deltaTime)
        {
        }

        public virtual void OnHit(GameObject projectile, float damageAmount)
        {
        }

        public virtual void OnHit(float damageAmount)
        {
        }

        public virtual void OnSpawn()
        {
        }

        public virtual void OnDead()
        {
        }
    }
}
