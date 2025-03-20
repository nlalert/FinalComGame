using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    class SkeletonEnemy : BaseEnemy
    {
        private int _limitIdlePatrol = 100;
        private Vector2 _patrolCenterPoint;

        public SkeletonEnemy(Texture2D texture, SpriteFont font) : base(texture, font) { }

        public override void Reset()
        {
            Console.WriteLine("Reset Skeleton");
            maxHealth = 80f;
            attackDamage = 5f;
            _patrolCenterPoint = Position;
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!HasSpawned) return;

            if (CurrentState == EnemyState.Dead || CurrentState == EnemyState.Dying)
            {
                IsActive = false;
            }

            UpdateInvincibilityTimer(deltaTime);

            switch (CurrentState)
            {
                case EnemyState.Idle:
                    IdlePatrol(deltaTime, gameObjects, tileMap);
                    break;
            }
            
            UpdateAnimation(deltaTime);
            base.Update(gameTime, gameObjects, tileMap);
        }

        public override void OnSpawn()
        {
            Console.WriteLine("Skeleton rises from the ground!");
        }

        public override void OnDead()
        {
            Console.WriteLine("Skeleton slowly crumbles to dust...");
            base.OnDead();
        }

        public override void DropItem()
        {
            base.DropItem();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            DrawDebug(spriteBatch);
        }

        private void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 20);
            string directionText = direction != 1 ? "Left" : "Right";
            string displayText = $"Dir {directionText}\nPatrolDis {(Position.X - _patrolCenterPoint.X)}\nCHp {Health}";
            spriteBatch.DrawString(_DebugFont, displayText, textPosition, Color.White);
        }

        private void IdlePatrol(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

            if (Math.Abs(Position.X - _patrolCenterPoint.X) >= _limitIdlePatrol)
            {
                direction *= -1;
            }

            Velocity.X = 50f * direction;
            if(this.HaveLineOfSight(player,tileMap) && Vector2.Distance(player.Position,this.Position) <=100)
            {
                Console.WriteLine("Skeleton sees the player! Switching to chase mode.");
                CurrentState = EnemyState.Chase;
            }
        }

        public override void OnCollisionHorizon()
        {
            if (CurrentState == EnemyState.Idle)
            {
                direction *= -1;
            }
            base.OnCollisionHorizon();
        }

        public override void OnHit(float damageAmount)
        {
            base.OnHit(damageAmount);
        }

        public override void OnCollidePlayer(Player player)
        {
            Console.WriteLine("Skeleton hit player");
            this.OnHit(Health / 10);
            base.OnCollidePlayer(player);
        }
    }
}
