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
        private float fontScale; // Scale for the font

        // Constructor with explicit scale parameter
        public Button(Rectangle bounds, Texture2D texture, string text, Color textColor, 
                      Rectangle spriteRectangle, float textScale = 1f) : base(bounds)
        {
            this.texture = texture;
            this.text = text;
            this.textColor = textColor;
            this.rectangle = spriteRectangle;
            this.fontScale = textScale;
            
            UpdateTextPosition();
        }
        
        // Add this method to handle text positioning
        private void UpdateTextPosition()
        {
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
                    _bounds.X + (_bounds.Width - maxWidth) / 2,
                    _bounds.Y + (_bounds.Height - totalHeight) / 2
                );
            }
            else
            {
                // Single line centering
                Vector2 textSize = Singleton.Instance.GameFont.MeasureString(text) * fontScale;
                textPosition = new Vector2(
                    _bounds.X + (_bounds.Width - textSize.X) / 2,
                    _bounds.Y + (_bounds.Height - textSize.Y) / 2
                );
            }
        }

        // Method to change the scale after creation
        public void SetTextScale(float scale)
        {
            this.fontScale = scale;
            UpdateTextPosition();
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
                // Single line drawing
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