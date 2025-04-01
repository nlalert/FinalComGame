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
        public GrenadeProjectile(Texture2D texture, Texture2D explosionTexture, SoundEffect explosionSound) : base(texture)
        {
            CanCollideTile = true;
            BaseExplosion = new Explosion(explosionTexture, explosionSound);
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Velocity * deltaTime;
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

        public void StartExplosion(List<GameObject> gameObjects)
        {
            IsActive = false; // Remove Grenade and left with only explosion

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