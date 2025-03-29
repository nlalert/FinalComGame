using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Staff : RangeWeapon
    {
        public Staff(Texture2D texture, ItemType type, string description, Vector2 Position, float attackDamage = 100f, int ammoAmount = 15)
            : base(texture, type, description, Position, attackDamage, ammoAmount)
        {
            //TODO : Add Staff Bullet Texture
        }
    }
}
