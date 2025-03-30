using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class TowerBullet : Projectile
    {
        public TowerBullet(Texture2D texture) : base(texture)
        {
            CanCollideTile = true;
        }
    }
}
