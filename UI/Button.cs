using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    public class Button : UIElement
    {
        private SpriteFont font;
        private Texture2D texture;
        private Texture2D hoverTexture;
        private string text;
        private Color textColor;
        private Vector2 textPosition;
        
        public event EventHandler OnClick;
        private bool wasPressed = false;
        
        public Button(Rectangle bounds, Texture2D texture, Texture2D hoverTexture, 
                    SpriteFont font, string text, Color textColor) : base(bounds)
        {
            this.texture = texture;
            this.hoverTexture = hoverTexture;
            this.font = font;
            this.text = text;
            this.textColor = textColor;
            
            // Center text
            Vector2 textSize = font.MeasureString(text);
            textPosition = new Vector2(
                bounds.X + (bounds.Width - textSize.X) / 2,
                bounds.Y + (bounds.Height - textSize.Y) / 2
            );
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MouseState mouseState = Singleton.Instance.CurrentMouseState;
            // Check for click (pressed and then released)
            if (wasPressed && isHovered && mouseState.LeftButton == ButtonState.Released)
            {
                OnClick?.Invoke(this, EventArgs.Empty);
            }
            
            wasPressed = isPressed;
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D currentTexture = isHovered ? hoverTexture : texture;
            spriteBatch.Draw(currentTexture, bounds, Color.White);
            spriteBatch.DrawString(font, text, textPosition, textColor);
        }
    }
}
