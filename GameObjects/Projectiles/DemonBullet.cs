using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class DemonBullet : Projectile
    {
        public DemonBullet(Texture2D texture) : base(texture)
        {
            Name = "BulletEnemy";
            DamageAmount = 15f;
            Speed = 150f;
            CanCollideTile = true;
            Viewport = new Rectangle(0, 0, 32, 32);
        }
    }
}
