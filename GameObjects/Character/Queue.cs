using FinalComGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data;

namespace FinalComGame
{
    public class Queue : BaseEnemy
    {
        public Queue(Texture2D texture) : base(texture)
        {
            _texture = texture;
            Direction = 1;
        }    

        public override void AddAnimation(){
            Animation = new Animation(_texture, 80, 48, new Vector2(80*4 , 64*4), 24);
            Animation.AddAnimation("idle", new Vector2(0, 0), 16);
            Animation.ChangeAnimation("idle");
            Position.X -= 12;
            Position.Y -= 4;
        }

        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap){
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.UpdateAnimation(deltaTime);
        }



        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Animation.GetTexture(),
                GetDrawingPosition(),
                Animation.GetCurrentFrame(),
                Color.White,
                0f, 
                Vector2.Zero,
                Scale,
                SpriteEffects.None, 
                0f
            );

            ////DrawDebug(spriteBatch);
        }
    }
}
