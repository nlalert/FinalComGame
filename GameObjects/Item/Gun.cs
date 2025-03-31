using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class Gun : RangeWeapon
{
    public Gun(Texture2D texture, ItemType type, Vector2 Position, float attackDamage = 100f, int ammoAmount = 6)
        : base(texture, type, Position, attackDamage, ammoAmount)
    {
        // Gun-specific initialization
    }

    // Gun uses the default RangeWeapon implementation for shooting straight bullets
    public override string GetDisplayProperties()
    {
        return $"\nDamage: {AttackDamage}\nAmmo: {_ammoAmount}\nType: Gun";
    }
}

