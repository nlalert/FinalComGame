using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class GrapplingHook : Projectile
    {
        public bool Hooked;
        public Vector2 HookedPosition;
        private float _lifetime = 0.5f;
        public Texture2D RopeTexture;
        public GrapplingHook(Texture2D texture) : base(texture) // No damage, high speed
        {
            CanCollideTile = true;
            Hooked = false;
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            _lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
             if (_lifetime <= 0f )
            {
                //time out 
                IsActive = false;
                if (Singleton.Instance.Player._grapplingHook == this)
                {
                    Singleton.Instance.Player._grapplingHook = null;
                }
                if(Hooked){
                    Singleton.Instance.Player._isGrappling = false;
                    Singleton.Instance.Player._isJumping = false;
                }
            }
            if (Hooked)
                return; // Stop moving if hooked

            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
           
            // Check collision with solid tiles
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
                                // IsActive = false;
                                Hooked = true;
                                _lifetime = 5f;
                                HookedPosition = Position;
                                Velocity = Vector2.Zero;
                                break;
                            }
                        }
                    }
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 playerPos = Singleton.Instance.Player.GetPlayerCenter();
            float hookDistance = Vector2.Distance(playerPos, this.Position);
            Vector2 projectileCenter = new Vector2(Position.X + _texture.Width/2, Position.Y + _texture.Height/2); 

            // Correct the angle calculation by remove MathF.PI / 2 for proper alignment
            float angle = MathF.Atan2(projectileCenter.Y - playerPos.Y, projectileCenter.X - playerPos.X) - MathF.PI / 2;
            Vector2 ropeOrigin = new Vector2(RopeTexture.Width / 2f, 0); // Origin at the top of the rope texture
            Vector2 ropePosition = playerPos; // Start the rope at the player position

            // Draw the rope texture stretched to the distance
            spriteBatch.Draw(RopeTexture, ropePosition, null, Color.White, angle, ropeOrigin, new Vector2(1.0f, hookDistance / RopeTexture.Height), SpriteEffects.None, 0f);

            // Draw the grappling hook (the projectile itself)
            // spriteBatch.Draw(_texture, Position, Color.White);

            // Optional: Draw debug information (if needed)
            DrawDebug(spriteBatch);
        }
        private void DrawDebug(SpriteBatch spriteBatch){
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 40);
            string displayText = $"StarPos : {Position} \ntimer {_lifetime} ";
            spriteBatch.DrawString(Singleton.Instance.GameFont, displayText, textPosition , Color.White);
        }
    }
}
