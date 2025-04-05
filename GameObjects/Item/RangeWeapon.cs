using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
    public SoundEffect ShootSound;
    public float AttackDamage;
    protected int _ammoAmount;

    public RangeWeapon(Texture2D texture, ItemType type, float attackDamage, int ammoAmount)
        : base(texture, type)
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
        ShootSound.Play();
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
    
    public override bool OnDrop(Vector2 position)
    {
        Singleton.Instance.Player.ChangeToSoulBulletAttack();
        return base.OnDrop(position);
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

    public override void DrawInSlot(SpriteBatch spriteBatch, Rectangle slotBounds, float scale = 1.0f)
    {
        base.DrawInSlot(spriteBatch, slotBounds, scale);

        Rectangle destinationRect = new Rectangle(
            slotBounds.X + (int)(slotBounds.Width * (1 - scale) / 2),
            slotBounds.Y + (int)(slotBounds.Height * (1 - scale) / 2),
            (int)(slotBounds.Width * scale),
            (int)(slotBounds.Height * scale)
        );

        string text = _ammoAmount.ToString();
        Vector2 textSize = Singleton.Instance.GameFont.MeasureString(text);

        Vector2 destinationPosition = new Vector2(
            destinationRect.Right - textSize.X,
            destinationRect.Bottom - textSize.Y
        );

        spriteBatch.DrawString(
            Singleton.Instance.GameFont,
            _ammoAmount.ToString(),
            destinationPosition,
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            0f
        );

    }
}
