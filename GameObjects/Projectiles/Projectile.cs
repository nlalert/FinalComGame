using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public abstract class Projectile : GameObject
    {
        public float BaseDamageAmount;
        public float DamageAmount;
        public float Speed;
        public bool CanCollideTile;
        protected Vector2 StartPosition;
        public Projectile(Texture2D texture, float damage = 15f, float speed = 300f ) : base(texture)
        {
            DamageAmount = damage;
            Speed = speed;
        }

        public virtual void Shoot(Vector2 position, Vector2 direction)
        {
            Position = position;
            Velocity = direction * Speed;
            StartPosition = position;
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(IsTouching(Singleton.Instance.Player))
            {
                OnProjectileHit(Singleton.Instance.Player);
                Singleton.Instance.Player.OnHitByProjectile(this,DamageAmount);
                // s.Reset();//TODO ???? why does it need reset idk
                IsActive = false;
                // Singleton.Instance.Life--;
                // Singleton.Instance.CurrentGameState = Singleton.GameState.StartNewLife;
            }
            
            // Check collision with tiles
            if(CanCollideTile){
                int radius = 5;
                for (int i = -radius; i <= radius; i++)
                {
                    for (int j = -radius; j <= radius; j++)
                    {
                        Vector2 newPosition = new(Position.X + i * Singleton.TILE_SIZE, Position.Y + j * Singleton.TILE_SIZE);
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

            base.Update(gameTime, gameObjects, tileMap);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Viewport, Color.White);
            base.Draw(spriteBatch);
            //DrawDebug(spriteBatch);
        }
        private void DrawDebug(SpriteBatch spriteBatch)
        {
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 20);
            string displayText = $"Damage {DamageAmount}\n Speed {Speed} ";
            spriteBatch.DrawString(Singleton.Instance.GameFont, displayText, textPosition, Color.White);
        }
        /// <summary>
        /// Modify Projectiles data onhit enemy 
        /// Can modify projectiles damage here
        /// Exmaple : adjust dmg base on enemy hp
        /// </summary>
        public virtual void OnProjectileHit(Character character){

        }
    }
}
