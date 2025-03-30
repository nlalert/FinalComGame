using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Staff : RangeWeapon
    {
        public FireBall FireBall;
        public float MPCost;
        public Staff(Texture2D texture, ItemType type, Vector2 Position, float attackDamage = 100f, int ammoAmount = 15)
            : base(texture, type, Position, attackDamage, ammoAmount)
        {
            //TODO : Add Staff Bullet Texture
        }
    }
}
