using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class PlayerBullet : Projectile
    {
        public PlayerBullet(Texture2D texture) : base(texture)
        {
            CanCollideTile = true;
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (var enemy in gameObjects.OfType<BaseEnemy>())
            {
                if(IsTouching(enemy))
                {
                    OnProjectileHit(enemy);
                    enemy.OnHitByProjectile(this, DamageAmount, false);
                    IsActive = false;
                }
            }
            foreach (var bullet in gameObjects.OfType<Projectile>())
            {
                if(bullet != this && bullet is not PlayerBullet && IsTouching(bullet))
                {
                    if(bullet is not DemonLaser && bullet is not SoulMinion)
                    {
                        IsActive = false;
                    }
                } 
            }

            // Check collision with tiles
            if(CanCollideTile){
                foreach (Vector2 offset in _collisionOffsets)
                {
                    Vector2 checkPosition = new Vector2(Position.X + offset.X, Position.Y + offset.Y);
                    Tile tile = tileMap.GetTileAtWorldPostion(checkPosition);
                    
                    if(tile != null && tile.IsSolid)
                    {
                        if (IsTouching(tile))
                        {
                            IsActive = false;
                            break;
                        }
                    }
                }
            }
        }
    }
}
