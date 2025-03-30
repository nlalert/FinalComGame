using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Gun : RangeWeapon
    {
        public Gun(Texture2D texture, ItemType type, Vector2 Position, float attackDamage = 100f, int ammoAmount = 6)
            : base(texture, type, Position, attackDamage, ammoAmount)
        {
            //TODO : Add Gun Bullet Texture
        }
    }
}
