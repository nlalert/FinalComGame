using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    public class HealthBar : Bar
    {
        public Character Target;
        private Color absorptionColor;
        
        public HealthBar(Character target, Rectangle bounds, Color foregroundColor, Color backgroundColor, bool showBorder = true, Color? borderColor = null, int borderThickness = 2) 
            : base(bounds, foregroundColor, backgroundColor, showBorder, borderColor, borderThickness)
        {
            Target = target;
            absorptionColor = new Color(255, 192, 0);
        }

        public override float GetPercentage()
        {
            return Target.Health / Target.MaxHealth;
        }

        private float GetAbsorptionPercentage()
        {
            if(Target is Player)
            {
                if (Singleton.Instance.Player.AbsorptionHealth > 0)
                {
                    return Singleton.Instance.Player.AbsorptionHealth / Singleton.Instance.Player.MaxHealth;
                }
            }
            return 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // 1x1 pixel texture 
            Texture2D dotTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            dotTexture.SetData(new[] { Color.White });

            // Draw background
            spriteBatch.Draw(dotTexture, _bounds, backgroundColor);

            // Calculate normal health bar width
            float healthPercentage = GetPercentage();
            Rectangle healthRect = new Rectangle(
                _bounds.X, 
                _bounds.Y, 
                (int)(_bounds.Width * healthPercentage), 
                _bounds.Height);

            // Draw the normal health bar
            spriteBatch.Draw(dotTexture, healthRect, foregroundColor);
            // Draw absorption health if it exists
            float absorptionPercentage = GetAbsorptionPercentage();
            if (absorptionPercentage > 0)
            {
                // Absorption health appears as an extension of the normal health bar
                Rectangle absorptionRect = new Rectangle(
                    _bounds.X + (int)(_bounds.Width * healthPercentage), 
                    _bounds.Y, 
                    (int)(_bounds.Width * absorptionPercentage), 
                    _bounds.Height);

                spriteBatch.Draw(dotTexture, absorptionRect, absorptionColor);
            }

            // Draw border if enabled
            if (showBorder)
            {
                // Top border
                spriteBatch.Draw(
                    dotTexture, 
                    new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, borderThickness), 
                    borderColor);
                
                // Bottom border
                spriteBatch.Draw(
                    dotTexture, 
                    new Rectangle(_bounds.X, _bounds.Y + _bounds.Height - borderThickness, _bounds.Width, borderThickness), 
                    borderColor);
                
                // Left border
                spriteBatch.Draw(
                    dotTexture, 
                    new Rectangle(_bounds.X, _bounds.Y, borderThickness, _bounds.Height), 
                    borderColor);
                
                // Right border
                spriteBatch.Draw(
                    dotTexture, 
                    new Rectangle(_bounds.X + _bounds.Width - borderThickness, _bounds.Y, borderThickness, _bounds.Height), 
                    borderColor);
            }
        }
    }
}