using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    class SkeletonEnemy : BaseEnemy
    {
        public SkeletonEnemy (Texture2D texture) : base(texture){

        }
        public override void Reset()
        {
            Console.WriteLine("Reset Skeleton");
            maxHealth = 80f;
            attackDamage = 5f;
            base.Reset();
        }
        public override void Update(GameTime gameTime, List<GameObject> gameObjects)
        {
            // Console.WriteLine(this.Health); //debug ai 
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (CurrentState)
            {
                case EnemyState.Idle :
                    // Console.WriteLine(Position);
                    Velocity.Y += 300*(float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
                // Other state logic similar to previous implementation
            }

            //update position
            float newX = Position.X + Velocity.X * deltaTime;
            float newY = Position.Y + Velocity.Y * deltaTime;
            Position = new Vector2(newX, newY);

            this.Velocity.Y =0;
            this.Velocity.X =0;
            UpdateHitbox();
            base.Update(gameTime, gameObjects);
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
            base.Draw(spriteBatch);
        }
        public override void OnHit(GameObject projectile, float damageAmount)
        {
            Console.WriteLine("Damage " + damageAmount + "CurHP" +this.Health);
            base.OnHit(projectile, damageAmount);
        }
    }
}
