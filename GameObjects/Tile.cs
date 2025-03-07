using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    class Tile : GameObject
    {
        public bool IsSolid;

        public Tile(Texture2D texture)
        {
            _texture = texture;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _texture, 
                new Rectangle((int)Position.X, (int)Position.Y, (int)(Viewport.Width * Scale.X), (int)(Viewport.Height * Scale.Y)), 
                Viewport, 
                Color.White);
            base.Draw(spriteBatch);
        }
    }
}