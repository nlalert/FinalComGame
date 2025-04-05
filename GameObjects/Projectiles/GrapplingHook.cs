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
        private float _lifetime = 1.0f;
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
                foreach (Vector2 offset in _collisionOffsets)
                {
                    Vector2 checkPosition = new Vector2(Position.X + offset.X, Position.Y + offset.Y);
                    Tile tile = tileMap.GetTileAtWorldPostion(checkPosition);
                    if(tile != null)
                    {
                        if (IsTouching(tile) && tile.Type == TileType.Grappling_Tile)
                        {
                            Hooked = true;
                            _lifetime = 1.5f;
                            HookedPosition = Position;
                            Velocity = Vector2.Zero;
                            break;
                        }
                    }
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {

            Vector2 playerPos = Singleton.Instance.Player.GetPlayerCenter();

            float hookAngle = MathF.Atan2(_direction.Y, _direction.X) + MathF.PI / 2f;

            //bottom end of a hook
            Vector2 hookOrigin = new Vector2(Viewport.Width/2, Viewport.Height/2);
            Vector2 hookPosition = Position + hookOrigin;
            Vector2 hookLocalBottomCenter = new Vector2(0, Viewport.Height/2);
            Vector2 hookRotatedOffset = hookOrigin + RotateVector(hookLocalBottomCenter, hookAngle);
            Vector2 hookEndPosition = Position + hookRotatedOffset;

            // Correct the angle calculation by remove MathF.PI / 2 for proper alignment
            float ropeAngle = MathF.Atan2(hookEndPosition.Y - playerPos.Y, hookEndPosition.X - playerPos.X) - MathF.PI / 2;
            Vector2 ropeOrigin = new Vector2(ViewportManager.Get("Hook_Rope").Width/2, 0); // Origin at the top of the rope texture
            Vector2 ropePosition = playerPos + ropeOrigin; // Start the rope at the player position

            float hookDistance = Vector2.Distance(playerPos, Position + hookRotatedOffset);

            // Draw the rope texture stretched to the distance
            spriteBatch.Draw(_texture, ropePosition, ViewportManager.Get("Hook_Rope"), Color.White, ropeAngle, ropeOrigin, 
            new Vector2(1.0f, hookDistance / ViewportManager.Get("Hook_Rope").Height), SpriteEffects.None, 0f);

            // Draw the grappling hook (the projectile itself)
            spriteBatch.Draw(_texture, hookPosition, Viewport, Color.White, hookAngle, hookOrigin, Vector2.One, SpriteEffects.None, 0f);

            // Optional: Draw debug information (if needed)
            //DrawDebug(spriteBatch);
        }
        private void DrawDebug(SpriteBatch spriteBatch){
            Vector2 textPosition = new Vector2(Position.X, Position.Y - 40);
            string displayText = $"StarPos : {Position} \ntimer {_lifetime} ";
            spriteBatch.DrawString(Singleton.Instance.GameFont, displayText, textPosition , Color.White);
        }

        Vector2 RotateVector(Vector2 v, float angle)
        {
            //Calculate the Bottom End Position After Rotated
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            return new Vector2( v.X * cos - v.Y * sin, v.X * sin + v.Y * cos );
        }
    }
}
