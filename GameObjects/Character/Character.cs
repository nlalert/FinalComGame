using FinalComGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FinalComGame {
    public class Character : GameObject
    {
        public float Health;
        public float MaxHealth;
        public int WalkSpeed;
        public int Direction = 1; // 1 = Right, -1 = Left

        // i-frame
        protected float _invincibilityDuration = 0.5f; // 0.5 seconds of i-frames
        protected float _invincibilityTimer;

        //Attack
        protected bool _isAttacking;
        public float BaseAttackDamage;
        public float AttackDamage;
        public float AttackDuration; // How long the attack lasts
        public float AttackCooldown; // Cooldown before attacking again
        protected float _attackTimer;
        protected float _attackCooldownTimer;
        public Rectangle AttackHitbox;
        public int AttackWidth;
        public int AttackHeight;
        
        //Jump
        public bool _isJumping;
        public float BaseJumpStrength;
        public float JumpStrength;

        //Animation
        protected Animation _idleAnimation;
        protected Animation _runAnimation;

        public SoundEffect HitSound;

        public string _currentAnimation = "idle";

        public Character(Texture2D texture) : base(texture){

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffect = Direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color = IsInvincible() ? Color.Red : Color.White;
            
            spriteBatch.Draw(
                Animation.GetTexture(),
                GetDrawingPosition(),
                Animation.GetCurrentFrame(),
                color,
                0f, 
                Vector2.Zero,
                Scale,
                spriteEffect, 
                0f
            );

            base.Draw(spriteBatch);
        }

        protected virtual void DrawDebug(SpriteBatch spriteBatch)
        {
            // hitbox debug drawing
            Texture2D debugTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            debugTexture.SetData(new Color[] { Color.Red });

            if (_attackTimer > 0)
                spriteBatch.Draw(debugTexture, AttackHitbox, Color.Red);
        }

        protected Vector2 GetDrawingPosition()
        {
            return  new Vector2( Position.X - (Animation.GetFrameSize().X - Viewport.Width) / 2 
                                ,Position.Y - (Animation.GetFrameSize().Y - Viewport.Height) / 2);
        }

        protected virtual bool IsOnGround()
        {
            return Velocity.Y == 0;
        }

        protected virtual void UpdateAnimation(float deltaTime)
        {
            Animation?.Update(deltaTime);
        }

        protected bool IsInvincible()
        {
            return _invincibilityTimer > 0;
        }

        protected void UpdateInvincibilityTimer(float deltaTime)
        {
            if (IsInvincible())
                _invincibilityTimer -= deltaTime;
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
        public virtual void OnHitByProjectile(GameObject projectile, float damageAmount, bool isHeavyAttack){
        }

        /// <summary>
        /// This call when Character recive damage
        /// </summary>
        public virtual void OnHit(float damageAmount, bool IsHeavyAttack)
        {
            StartInvincibility(IsHeavyAttack);
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
        public virtual void OnDead(List<GameObject> gameObjects)
        {
            IsActive = false;
        }

        public void StartInvincibility(bool IsHeavyAttack)
        {
            if (IsHeavyAttack)
                _invincibilityTimer = _invincibilityDuration * 10;
            else
                _invincibilityTimer = _invincibilityDuration; // Activate i-frames
        }

        public void ResetJumpStrength()
        {
            JumpStrength = BaseJumpStrength;
        }
    }
}
