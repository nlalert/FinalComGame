using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    public class HealthBar : Bar
    {
        public Character Target;
        public HealthBar(Character target, Rectangle bounds, Color foregroundColor, Color backgroundColor, bool showBorder = true, Color? borderColor = null, int borderThickness = 2) 
            : base(bounds, foregroundColor, backgroundColor, showBorder,  borderColor, borderThickness)
        {
            Target = target;
        }

        public override float GetPercentage()
        {
            return Target.Health / Target.MaxHealth;
        }
    }
}