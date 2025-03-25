using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    /// <summary>
    /// This first mean to be standable block using invis enemy to create tile that follow enemy 
    /// But Cant find a way to attach tile to this enemy nor spawn tile in fixed location at all
    /// so change to jumping block instead might update name later
    /// </summary>
    public class PlatformEnemy : BaseEnemy
    {
        private float moveSpeed = 30f; // Speed of movement
        public PlatformEnemy(Texture2D texture, SpriteFont font): base(texture, font)
        {
            Velocity = new Vector2(moveSpeed, 0);
            CanCollideTile = true;
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

            Velocity.X = Direction *moveSpeed;
            
            // Move player if standing on top
            if (IsTouchingBottom(Singleton.Instance.Player))
            {
                OnPlayerStompHead();
            }
            base.Update(gameTime, gameObjects, tileMap);
        }
        public override void OnSpawn()
        {
            base.OnSpawn();
        }
        public override void OnCollisionHorizon()
        {
            Direction *= -1;
            base.OnCollisionHorizon();
        }
        private void OnPlayerStompHead(){
            if(Singleton.Instance.Player.Velocity.Y > 0)
                Singleton.Instance.Player.Velocity.Y = -1000;
        }
    }
}
