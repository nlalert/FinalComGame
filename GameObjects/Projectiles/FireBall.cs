using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class FireBall : Projectile
    {
        public Explosion BaseExplosion;
        
        public float ExplosionDuration;
        public FireBall(Texture2D texture) : base(texture)
        {
            CanCollideTile = true;
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * deltaTime;
            ApplyGravity(deltaTime);

            // Check enemy collision
            foreach (var enemy in gameObjects.OfType<BaseEnemy>())
            {
                if(IsTouching(enemy))
                {
                    OnProjectileHit(enemy);
                    enemy.OnHitByProjectile(this, DamageAmount, true);
                    StartExplosion(gameObjects);
                    return;
                }
            }

            // Check collision with other projectiles
            foreach (var bullet in gameObjects.OfType<Projectile>())
            {
                if(bullet != this && IsTouching(bullet))
                {
                    if(bullet is not PlayerBullet)
                    {
                        StartExplosion(gameObjects);
                        return;
                    }
                } 
            }

            // Check collision with tiles
            if(CanCollideTile)
            {
                foreach (Vector2 offset in _collisionOffsets)
                {
                    Vector2 checkPosition = new Vector2(Position.X + offset.X, Position.Y + offset.Y);
                    Tile tile = tileMap.GetTileAtWorldPostion(checkPosition);
                    if(tile != null && tile.IsSolid)
                    {
                        if (IsTouching(tile))
                        {
                            StartExplosion(gameObjects);
                            return;
                        }
                    }
                }
            }
        }

        public void StartExplosion(List<GameObject> gameObjects)
        {
            IsActive = false; // Remove Fireball and left with only explosion

            Velocity = Vector2.Zero;
            Explosion newExplosion = BaseExplosion.Clone() as Explosion;
            newExplosion.Position = Position;
            newExplosion.Radius = Radius;
            newExplosion.Duration = ExplosionDuration;
            newExplosion.Damage = DamageAmount * 0.8f;
            newExplosion.TriggerExplosion();
            gameObjects.Add(newExplosion);
        }

    }
} 