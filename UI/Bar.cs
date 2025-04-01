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
            // 1x1 pixel texture 
            Texture2D dotTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            dotTexture.SetData(new[] { Color.White });

            // Draw background
            spriteBatch.Draw(dotTexture, bounds, backgroundColor);

            // Calculate bar width
            float percentage = GetPercentage();
            Rectangle healthRect = new Rectangle(
                bounds.X, 
                bounds.Y, 
                (int)(bounds.Width * percentage), 
                bounds.Height);

            // Draw the bar
            spriteBatch.Draw(dotTexture, healthRect, foregroundColor);

            // Draw border if enabled
            if (showBorder)
            {
                // Top border
                spriteBatch.Draw(
                    dotTexture, 
                    new Rectangle(bounds.X, bounds.Y, bounds.Width, borderThickness), 
                    borderColor);
                
                // Bottom border
                spriteBatch.Draw(
                    dotTexture, 
                    new Rectangle(bounds.X, bounds.Y + bounds.Height - borderThickness, bounds.Width, borderThickness), 
                    borderColor);
                
                // Left border
                spriteBatch.Draw(
                    dotTexture, 
                    new Rectangle(bounds.X, bounds.Y, borderThickness, bounds.Height), 
                    borderColor);
                
                // Right border
                spriteBatch.Draw(
                    dotTexture, 
                    new Rectangle(bounds.X + bounds.Width - borderThickness, bounds.Y, borderThickness, bounds.Height), 
                    borderColor);
            }
        }
    }
}