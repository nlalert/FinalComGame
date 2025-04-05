using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    public class Bar : HUDElement
    {
        protected Color backgroundColor;
        protected Color foregroundColor;
        protected Color borderColor;
        protected bool showBorder;
        protected int borderThickness;

        public Bar(Rectangle bounds, Color foregroundColor, Color backgroundColor, bool showBorder = true, Color? borderColor = null, int borderThickness = 2) 
            : base(bounds)
        {
            this.foregroundColor = foregroundColor;
            this.backgroundColor = backgroundColor;
            this.showBorder = showBorder;
            this.borderColor = borderColor ?? Color.Black;
            this.borderThickness = borderThickness;
        }

        public virtual float GetPercentage()
        {
            return 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw background
            spriteBatch.Draw(Singleton.Instance.PixelTexture, _bounds, backgroundColor);

            // Calculate bar width
            float percentage = GetPercentage();
            Rectangle healthRect = new Rectangle(
                _bounds.X, 
                _bounds.Y, 
                (int)(_bounds.Width * percentage), 
                _bounds.Height);

            // Draw the bar
            spriteBatch.Draw(Singleton.Instance.PixelTexture, healthRect, foregroundColor);

            // Draw border if enabled
            if (showBorder)
            {
                // Top border
                spriteBatch.Draw(
                    Singleton.Instance.PixelTexture, 
                    new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, borderThickness), 
                    borderColor);
                
                // Bottom border
                spriteBatch.Draw(
                    Singleton.Instance.PixelTexture, 
                    new Rectangle(_bounds.X, _bounds.Y + _bounds.Height - borderThickness, _bounds.Width, borderThickness), 
                    borderColor);
                
                // Left border
                spriteBatch.Draw(
                    Singleton.Instance.PixelTexture, 
                    new Rectangle(_bounds.X, _bounds.Y, borderThickness, _bounds.Height), 
                    borderColor);
                
                // Right border
                spriteBatch.Draw(
                    Singleton.Instance.PixelTexture, 
                    new Rectangle(_bounds.X + _bounds.Width - borderThickness, _bounds.Y, borderThickness, _bounds.Height), 
                    borderColor);
            }
        }
    }
}