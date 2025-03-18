using System.Collections.Generic;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Tile : GameObject
    {
        public bool IsSolid;
        public string Type;

        public Tile(Texture2D texture)
        {
            _texture = texture;
        }
        
        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _texture,
                Position,
                Viewport,
                Color.White,
                Rotation, 
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f
            );
            base.Draw(spriteBatch);
        }
    }
}