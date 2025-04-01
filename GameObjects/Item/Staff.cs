using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class Staff : RangeWeapon
{
    public FireBall FireBall;

    public Staff(Texture2D texture, ItemType type, Vector2 Position, float attackDamage = 100f, int ammoAmount = 15)
        : base(texture, type, Position, attackDamage, ammoAmount)
    {
    }

    public override Projectile CreateProjectile(Vector2 position, int direction)
    {
        // Staff creates fireballs that shoot at an angle
        FireBall newFireball = FireBall.Clone() as FireBall;
        newFireball.DamageAmount = AttackDamage;
        newFireball.Shoot(position, new Vector2(direction, (float)Math.Sin(MathHelper.ToRadians(-45))));
        ShootSound.Play();
        return newFireball;
    }

    public override bool CanShoot()
    {
        // Staff requires both ammo and MP
        return _ammoAmount > 0 && Singleton.Instance.Player.MP >= MPCost;
    }

    public override void OnShoot()
    {
        // Staff consumes MP in addition to ammo
        Singleton.Instance.Player.UseMP(MPCost);
        base.OnShoot(); // Call base to handle ammo
    }

    public override string GetDisplayProperties()
    {
        return $"\nDamage: {AttackDamage}\nAmmo: {_ammoAmount}\nMP Cost: {MPCost}\nType: Staff";
    }
}

