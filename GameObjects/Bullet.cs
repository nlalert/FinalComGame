using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    public class Bullet : GameObject
    {   
        public float DistantMoved;
        public Bullet(Texture2D texture) : base(texture)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Viewport, Color.White);
            base.Draw(spriteBatch);
        }

        public override void Reset()
        {
            DistantMoved = 0;
            base.Reset();
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            DistantMoved += Math.Abs(Velocity.X * gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond);
            Position.X += Velocity.X * gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;

            if (DistantMoved >= Singleton.SCREEN_WIDTH)
                IsActive = false;

            foreach (GameObject s in gameObjects)
            {
                if(Name.Equals("BulletPlayer"))
                {
                    if(IsTouching(s))
                    {
                        if(s.Name.Equals("Enemy") || s.Name.Equals("BulletEnemy"))
                        {
                            // s.IsActive = false;
                            if(s is BaseEnemy enemy){
                                // enemy.OnHit(this,this.damage)
                                enemy.OnHitByProjectile(this,10);
                            } 
                            // if(s is Enemy)
                            // {
                            //     Singleton.Instance.Score += (s as Enemy).Score;
                            //     Singleton.Instance.InvaderLeft--;
                            // }
                            IsActive = false;
                        }
                    } 
                }
                else if(Name.Equals("BulletEnemy"))
                {
                    if(IsTouching(s) && s.Name.Equals("Player"))
                    {
                        s.Reset();
                        IsActive = false;
                        // Singleton.Instance.Life--;
                        // Singleton.Instance.CurrentGameState = Singleton.GameState.StartNewLife;
                    }
                }
            }

            foreach (Tile tile in tileMap.tiles.Values)
            {
                if(Name.Equals("BulletPlayer"))
                {
                    if(IsTouching(tile))
                    {
                        IsActive = false;
                    }
                }
            }

            base.Update(gameTime, gameObjects, tileMap);
        }
    }
}