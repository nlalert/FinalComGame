using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class PlatformEnemy : BaseEnemy
    {
        public PlatformEnemy(Texture2D texture): base(texture)
        {
            _texture = texture;
            WalkSpeed = 75;
            Velocity = new Vector2(WalkSpeed, 0);
            CanCollideTile = true;
        }

        public override void AddAnimation()
        {
            Animation = new Animation(_texture, 80, 64, new Vector2(80*8, 64*4), 24);

            Animation.AddAnimation("idle", new Vector2(0, 0), 16);
            Animation.AddAnimation("walk_right", new Vector2(0, 2), 8);
            Animation.AddAnimation("walk_left", new Vector2(0, 3), 8);

            Animation.ChangeAnimation("idle");
        }

        protected override void UpdateAnimation(float deltaTime)
        {
            string animation = "idle";

            if (Velocity.X != 0){
                if (Direction == 1)
                    animation = "walk_right";
                else
                    animation = "walk_left";
            }
                
            if(_currentAnimation != animation && !Animation.IsTransition)
            {
                _currentAnimation = animation;
                switch (animation)
                {
                    case "walk_right":
                    case "walk_left":
                        Animation.ChangeAnimationAndKeepFrame(_currentAnimation);
                        break;
                    default:
                        Animation.ChangeAnimation(_currentAnimation);
                        break;
                }    
            }
     
            base.UpdateAnimation(deltaTime);
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

            Velocity.X = Direction * WalkSpeed;
            
            base.Update(gameTime, gameObjects, tileMap);
            UpdateAnimation(deltaTime);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Animation.GetTexture(),
                GetDrawingPosition(),
                Animation.GetCurrentFrame(),
                Color.White,
                0f, 
                Vector2.Zero,
                Scale,
                SpriteEffects.None, 
                0f
            );

            DrawDebug(spriteBatch);
        }

        public override void OnCollisionHorizon()
        {
            Direction *= -1;
            base.OnCollisionHorizon();
        }

        protected override void UpdateHorizontalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position.X += Velocity.X * deltaTime;
            if(!CanCollideTile) 
                return;

            foreach (Vector2 offset in _collisionOffsets)
            {
                Vector2 checkPosition = new Vector2(Position.X + offset.X, Position.Y + offset.Y);
                Tile tile = tileMap.GetTileAtWorldPostion(checkPosition);

                if(tile != null && (tile.Type == TileType.Barrier || tile.Type == TileType.AmbushBarrier ||tile.Type == TileType.Platform))
                {
                    if(ResolveHorizontalCollision(tile)){
                        OnCollisionHorizon();
                    }
                }
            }
        }

        public override void CheckContactPlayer(){
            Position.Y -= Singleton.TILE_SIZE / 2;

            if(IsTouchingBottom(Singleton.Instance.Player))
            {
                Singleton.Instance.Player.Velocity.X += Velocity.X;
            }

            Position.Y += Singleton.TILE_SIZE / 2;
        }
    }
}
