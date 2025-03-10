using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    class SkeletonEnemy : BaseEnemy
    {
        private int _LimitIdlePatrol = 100;
        private Vector2 _PatrolCenterPoint;
        private int _patrolDirection = 1; // 1 = right, -1 = left
        public SkeletonEnemy (Texture2D texture,SpriteFont font) : base(texture,font){

        }
        public override void Reset()
        {
            
            Console.WriteLine("Reset Skeleton");
            maxHealth = 80f;
            attackDamage = 5f;
            _PatrolCenterPoint = Position;
            base.Reset();
        }
        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            // Console.WriteLine(this.Health); //debug ai 
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ApplyGravity(deltaTime);
            
            switch (CurrentState)
            {
                case EnemyState.Idle :
                    //move left right in idle state
                    IdlePatrol();
                    break;
                // Other state logic similar to previous implementation
            }
            UpdateHorizontalMovement(deltaTime,gameObjects,tileMap);
            UpdateVerticalMovement(deltaTime,gameObjects,tileMap);
            //update position
            float newX = Position.X + Velocity.X * deltaTime;
            float newY = Position.Y + Velocity.Y * deltaTime;
            Position = new Vector2(newX, newY);
            base.Update(gameTime, gameObjects, tileMap);
        }


        public override void OnSpawn()
        {
            // can be add spawn effect.. even out of screen
            // but why 
            Console.WriteLine("Skeleton rises from the ground!");
        }

        public override void OnDead()
        {
            Console.WriteLine("Skeleton slowly crumbles to dust...");
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            // Position text slightly above the enemy
            DrawDebug(spriteBatch);
        }
        private void DrawDebug(SpriteBatch spriteBatch){
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 20); // 20 pixels above the enemy
            string direction = _patrolDirection != 1 ? "Left" : "right";
            string displayText = "Dir " + direction+  "\n PatrolDis" + (Position.X - _PatrolCenterPoint.X); 
            spriteBatch.DrawString(_DebugFont, displayText, textPosition, Color.White);
        }
        public override void OnHit(GameObject projectile, float damageAmount)
        {
            base.OnHit(projectile, damageAmount);
            Console.WriteLine("Damage " + damageAmount + "CurHP" +this.Health);
        }

        private void ApplyGravity(float deltaTime)
        {
            Velocity.Y += Singleton.GRAVITY * deltaTime; 

        }
        private void UpdateHorizontalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position.X += Velocity.X * deltaTime;
            foreach (Tile tile in tileMap.tiles)
            {
                ResolveHorizontalCollision(tile);
            }
        }

        private void UpdateVerticalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position.Y += Velocity.Y * deltaTime;
            foreach (Tile tile in tileMap.tiles)
            {
                ResolveVerticalCollision(tile);
            }
        }
        private void IdlePatrol()
        {

            if (Math.Abs(Position.X - _PatrolCenterPoint.X) >= _LimitIdlePatrol)
            {
                _patrolDirection *= -1; // Switch direction
            }

            Velocity.X = 50f * _patrolDirection; // Adjust speed as needed
        }
    }
}
