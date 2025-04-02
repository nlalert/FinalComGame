using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class SoulBullet : Projectile
    {
        public SoulBullet(Texture2D texture) : base(texture)
        {
            CanCollideTile = true;
            CanHitPlayer= false;
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            foreach (var enemy in gameObjects.OfType<BaseEnemy>())
            {
                if(IsTouching(enemy))
                {
                    OnProjectileHit(enemy);
                    enemy.OnHitByProjectile(this, BaseDamageAmount, false);
                        
                    IsActive = false;
                }
            }
            foreach (var bullet in gameObjects.OfType<Projectile>())
            {
                if(IsTouching(bullet))
                {
                    if(bullet is not PlayerBullet)
                    {
                        if(bullet is not DemonLaser && bullet is not SoulMinion)
                        IsActive = false;
                    }
                } 
            }

            // Check collision with tiles
            base.Update(gameTime, gameObjects, tileMap);
        }
    }
}
