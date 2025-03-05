using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    class SkeletonEnemy : BaseEnemy
    {
        private float decayTimer = 2f; // Time for skeleton to decay after death

        public SkeletonEnemy(Player player) : base(player) 
        {
            maxHealth = 80f;
            attackDamage = 5f;
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects)
        {
            switch (CurrentState)
            {
                case EnemyState.Dying:
                    HandleDecay(gameTime);
                    break;
                // Other state logic similar to previous implementation
            }
            UpdateHitbox();
            base.Update(gameTime, gameObjects);
        }

        private void HandleDecay(GameTime gameTime)
        {
            decayTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (decayTimer <= 0)
            {
                CurrentState = EnemyState.Dead;
            }
        }

        public override void OnSpawn()
        {
            // Skeleton-specific spawn effect
            Console.WriteLine("Skeleton rises from the ground!");
            // Could add particle effects, sound, etc.
        }

        public override void OnDead()
        {
            Console.WriteLine("Skeleton slowly crumbles to dust...");
            // Could add visual decay effect, bone scatter, etc.
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw with potential decay effect
            Color drawColor = Color.White;
            if (CurrentState == EnemyState.Dying)
            {
                byte alphaValue = (byte)(255 * (decayTimer / 2f));
                // drawColor = new Color(255, 255, 255, alphaValue);
                drawColor = new Color(255,255,255,255f);
            }
            
            spriteBatch.Draw(_texture, Position, drawColor);
        }
    }
}
