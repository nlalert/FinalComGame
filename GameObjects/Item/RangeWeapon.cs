using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace FinalComGame;

public interface IShootable
{
    Projectile CreateProjectile(Vector2 position, int direction);
    bool CanShoot();
    void OnShoot();
}

public class RangeWeapon : Item, IShootable
{
    public float AttackDamage;
    protected int _ammoAmount;

    public RangeWeapon(Texture2D texture, ItemType type, Vector2 Position, float attackDamage, int ammoAmount)
        : base(texture, Position, type)
    {
        AttackDamage = attackDamage;
        _ammoAmount = ammoAmount;
    }

    public virtual Projectile CreateProjectile(Vector2 position, int direction)
    {
        // Default implementation - should be overridden by specific weapon types
        Projectile bullet = Singleton.Instance.Player.Bullet.Clone() as PlayerBullet;
        bullet.DamageAmount = AttackDamage;
        bullet.Shoot(position, new Vector2(direction, 0));
        return bullet;
    }

    public virtual bool CanShoot()
    {
        return _ammoAmount > 0;
    }

    public virtual void OnShoot()
    {
        DecreaseAmmo();
    }

    public void DecreaseAmmo()
    {
        _ammoAmount--;
        Console.WriteLine("Ammo " + _ammoAmount);
        if (_ammoAmount <= 0)
        {
            Singleton.Instance.Player.RemoveItem(1);
            Singleton.Instance.Player.ChangeToSoulBulletAttack();
        }
    }
    
    public override void OnDrop(Vector2 position)
    {
        Singleton.Instance.Player.ChangeToSoulBulletAttack();
        base.OnDrop(position);
    }

    public override void ActiveAbility(float deltaTime, int slot,List<GameObject> gameObjects)
    {
        Singleton.Instance.Player.ChangeToRangeWeaponAttack(AttackDamage);
        base.ActiveAbility(deltaTime, slot, gameObjects);
    }

    public override string GetDisplayProperties()
    {
        return $"\nDamage: {AttackDamage}\nAmmo: {_ammoAmount}";
    }
}
