using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class GrenadeProjectile : Projectile
    {
        public Explosion BaseExplosion;
    
        public float ExplosionDuration;
        public float DetonateDelayDuration;
        public float DetonateTimer;
        public SoundEffect grenade_Explode_sound;
        public GrenadeProjectile(Texture2D texture) : base(texture)
        {
            CanCollideTile = true;
            DetonateTimer = 0f;
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            DetonateTimer -= deltaTime;

            Rotation += Velocity.X * 0.001f;

            if (DetonateTimer <= 0)
            {
                StartExplosion(gameObjects);
                return;
            }

            ApplyGravity(deltaTime);
            UpdateHorizontalMovement(deltaTime, gameObjects, tileMap);
            UpdateVerticalMovement(deltaTime, gameObjects, tileMap);

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
        }

        public override void Shoot(Vector2 position, Vector2 direction)
        {
            // if (player._isCrouching)
            //     position += new Vector2(player.Direction * 20, 6); // Lower when crouching
            // else
            //     position += new Vector2(player.Direction * 20, 16); // At hand level
            DetonateTimer = DetonateDelayDuration;
            base.Shoot(position, direction);
        }

        public void StartExplosion(List<GameObject> gameObjects)
        {
            grenade_Explode_sound.Play();
            IsActive = false; // Remove Grenade and left with only explosion
            Velocity = Vector2.Zero;
            Explosion newExplosion = BaseExplosion.Clone() as Explosion;
            newExplosion.Position = Position;
            newExplosion.Radius = Radius;
            newExplosion.Duration = ExplosionDuration;
            newExplosion.Damage = BaseDamageAmount * 0.8f;
            newExplosion.TriggerExplosion();
            gameObjects.Add(newExplosion);
            
        }

        protected override void UpdateHorizontalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float previousVelocityX = Velocity.X; 
            Position.X += Velocity.X * deltaTime;

            foreach (Vector2 offset in _collisionOffsets)
            {
                Vector2 checkPosition = new Vector2(Position.X + offset.X, Position.Y + offset.Y);
                Tile tile = tileMap.GetTileAtWorldPostion(checkPosition);
                if(tile != null && tile.IsSolid)
                {
                    //Bounce
                    if(ResolveHorizontalCollision(tile))
                    {
                        Velocity.X = -previousVelocityX / 2;
                    }
                }
            }
        }

        protected override void UpdateVerticalMovement(float deltaTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float previousVelocityY = Velocity.Y; 
            Position.Y += Velocity.Y * deltaTime;

            foreach (Vector2 offset in _collisionOffsets)
            {
                Vector2 checkPosition = new Vector2(Position.X + offset.X, Position.Y + offset.Y);
                Tile tile = tileMap.GetTileAtWorldPostion(checkPosition);

                if(tile != null && tile.IsSolid)
                {
                    //Bounce
                    if(ResolveVerticalCollision(tile))
                    {
                        Velocity.Y = -previousVelocityY / 1.2f;
                        Velocity.X /= 1.2f;
                    }
                }
            }
            foreach (var platformEnemy in gameObjects.OfType<PlatformEnemy>())
            {
                //Bounce
                if(ResolveVerticalCollision(platformEnemy))
                {
                    Velocity.Y = -previousVelocityY / 1.2f;
                    Velocity.X /= 1.2f;
                }
            }
        }
    }
} 