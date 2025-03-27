using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class PlayerBullet : Projectile
    {
        public float BaseDamageAmount;
        public PlayerBullet(Texture2D texture) : base(texture)
        {
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var enemy in gameObjects.OfType<BaseEnemy>())
            {
                if(IsTouching(enemy))
                {
                    OnProjectileHit(enemy);
                    enemy.OnHitByProjectile(this, DamageAmount);
                        
                    IsActive = false;
                }
            }
            foreach (var bullet in gameObjects.OfType<Projectile>())
            {
                if(IsTouching(bullet))
                {
                    if(bullet is not PlayerBullet)
                    {
                        IsActive = false;
                    }
                } 
            }

            // Check collision with tiles
            if(CanCollideTile){
                int radius = 5;
                for (int i = -radius; i <= radius; i++)
                {
                    for (int j = -radius; j <= radius; j++)
                    {
                        Vector2 newPosition = new(Position.X + i * Singleton.BLOCK_SIZE, Position.Y + j * Singleton.BLOCK_SIZE);
                        Tile tile = tileMap.GetTileAtWorldPostion(newPosition);
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
}
