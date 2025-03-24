using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class PlayerBullet : Projectile
    {
        public PlayerBullet(Texture2D texture) : base(texture)
        {
            Name = "BulletPlayer";
            DamageAmount = 15f;
            Speed = 300f;
            CanCollideTile = true;
        }
    }
}
