using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    public class ImageUI : HUDElement
    {
        private Texture2D _texture;
        private Rectangle _viewport;
        public ImageUI(Texture2D texture, Rectangle bounds, Rectangle viewport) : base(bounds)
        {
            _texture = texture;
            _viewport = viewport;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the texture to fill the bounds
            spriteBatch.Draw(
                _texture,          
                _bounds,           
                _viewport,  
                Color.White             
            );
        }
    }
}
