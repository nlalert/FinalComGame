using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    public class Button : HUDElement
    {
        private Texture2D texture;
        private string text;
        private Color textColor;
        private Vector2 textPosition;
        private Rectangle rectangle = new Rectangle(0, 0, 0, 0);
        
        public event EventHandler OnClick;
        private bool wasPressed = false;
        private float fontScale = 2.5f; // Half the original font size
        public Button(Rectangle bounds, Texture2D texture, string text, Color textColor, Rectangle SpriteRectagle) : base(bounds)
        {
            this.texture = texture;
            this.text = text;
            this.textColor = textColor;
            this.rectangle = SpriteRectagle;
            
            // Center text - split into lines if needed
            if (text.Contains("\n"))
            {
                string[] lines = text.Split('\n');
                float totalHeight = 0;
                float maxWidth = 0;
                
                // Calculate total height and max width of all lines
                foreach (string line in lines)
                {
                    Vector2 lineSize = Singleton.Instance.GameFont.MeasureString(line) * fontScale;
                    totalHeight += lineSize.Y;
                    maxWidth = Math.Max(maxWidth, lineSize.X);
                }
                
                // Position first line, other lines will be positioned in Draw method
                textPosition = new Vector2(
                    bounds.X + (bounds.Width - maxWidth) / 2,
                    bounds.Y + (bounds.Height - totalHeight) / 2
                );
            }
            else
            {
                // Single line centering (current approach)
                Vector2 textSize = Singleton.Instance.GameFont.MeasureString(text) * fontScale;
                textPosition = new Vector2(
                    bounds.X + (bounds.Width - textSize.X) / 2,
                    bounds.Y + (bounds.Height - textSize.Y) / 2
                );
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Check for click (pressed and then released)
            if (wasPressed && _isHovered && Singleton.Instance.CurrentMouseState.LeftButton == ButtonState.Released)
            {
                OnClick?.Invoke(this, EventArgs.Empty);
            }
            
            wasPressed = _isPressed;
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, _bounds, rectangle, Color.White);
            
            // Draw hover effect
            if (_isHovered)
            {
                spriteBatch.Draw(texture, _bounds, rectangle, new Color(255, 255, 255, 128));
            }
            
            // Handle multi-line text drawing
            if (text.Contains("\n"))
            {
                string[] lines = text.Split('\n');
                Vector2 currentPosition = textPosition;
                
                foreach (string line in lines)
                {
                    Vector2 lineSize = Singleton.Instance.GameFont.MeasureString(line) * fontScale;
                    float lineX = _bounds.X + (_bounds.Width - lineSize.X) / 2; // Center each line horizontally
                    
                    spriteBatch.DrawString(
                        Singleton.Instance.GameFont,
                        line,
                        new Vector2(lineX, currentPosition.Y),
                        textColor,
                        0f,
                        Vector2.Zero,
                        fontScale,
                        SpriteEffects.None,
                        0f
                    );
                    
                    // Move down for next line
                    currentPosition.Y += lineSize.Y;
                }
            }
            else
            {
                // Single line drawing (current approach)
                spriteBatch.DrawString(
                    Singleton.Instance.GameFont,
                    text,
                    textPosition,
                    textColor,
                    0f,
                    Vector2.Zero,
                    fontScale,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}
