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
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(HasSpawned == false)
                    return;
            if(CurrentState == EnemyState.Dead || CurrentState == EnemyState.Dying){
                this.IsActive = false;
            }

            UpdateInvincibilityTimer(deltaTime);

            if(CanCollideTile){

            }
            switch (CurrentState)
            {
                //donot update Position at all cost 
                case EnemyState.Idle :
                    //move left right in idle state
                    IdlePatrol(deltaTime,gameObjects,tileMap);
                    break;
            }
            
            UpdateAnimation(deltaTime);

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
            base.OnDead();
        }

        public override void DropItem()
        {
            base.DropItem();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            // Position text slightly above the enemy
            DrawDebug(spriteBatch);
        }
        private void DrawDebug(SpriteBatch spriteBatch){
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 20); // 20 pixels above the enemy
            string directionText = direction != 1 ? "Left" : "right";
            string displayText = "Dir " + directionText +  "\n PatrolDis" + (Position.X - _PatrolCenterPoint.X) + "\n CHp" + this.Health; 
            spriteBatch.DrawString(_DebugFont, displayText, textPosition, Color.White);
        }

        private void IdlePatrol(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

            //moing left
            if (Math.Abs(Position.X - _PatrolCenterPoint.X) >= _LimitIdlePatrol)
            {
                direction *= -1; // Switch direction
            }

            Velocity.X = 50f * direction; // Adjust speed as needed
        }
        public override void OnCollisionHorizon()
        {
            if(CurrentState == EnemyState.Idle){
                direction *= -1;
            }
            base.OnCollisionHorizon();
        }
        public override void OnHit(float damageAmount)
        {
            //ADD damge taken sound
            base.OnHit(damageAmount);
        }
        public override void OnCollidePlayer(Player player)
        {
            Console.WriteLine("Skeleton hit player");
            //skeleton have weak bone get hurt it self by confusion
            this.OnHit(Health/10);
            base.OnCollidePlayer(player);
        }

    }      
}
