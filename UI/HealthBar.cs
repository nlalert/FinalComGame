using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    public class HealthBar : Bar
    {
        public HealthBar(Rectangle bounds, Color foregroundColor, Color backgroundColor, bool showBorder = true, Color? borderColor = null, int borderThickness = 2) 
            : base(bounds, foregroundColor, backgroundColor, showBorder,  borderColor, borderThickness)
        {
        }

        public override float GetPercentage()
        {
            return Singleton.Instance.Player.Health / Singleton.Instance.Player.MaxHealth;
        }
    }
}