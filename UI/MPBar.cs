using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    public class MPBar : Bar
    {
        public MPBar(Rectangle bounds, Player player, Color foregroundColor, Color backgroundColor, bool showBorder = true, Color? borderColor = null, int borderThickness = 2) 
            : base(bounds, player, foregroundColor, backgroundColor, showBorder,  borderColor, borderThickness)
        {
        }

        public override float GetPercentage()
        {
            return player.MP / player.maxMP;
        }
    }
}