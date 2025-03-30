using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class RangeWeapon : Item
    {
        public float AttackDamage;
        private int _ammoAmount;

        public RangeWeapon(Texture2D texture, ItemType type, Vector2 Position, float attackDamage, int ammoAmount)
            : base(texture, Position, type)
        {
            AttackDamage = attackDamage;
            _ammoAmount = ammoAmount;
        }

        public void DecreaseAmmo()
        {
            _ammoAmount--;
            Console.WriteLine("Ammo "+ _ammoAmount);
            if(_ammoAmount <= 0)
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

        public override void ActiveAbility(float deltaTime, int slot)
        {
            Singleton.Instance.Player.ChangeToRangeWeaponAttack(AttackDamage);
            base.ActiveAbility(deltaTime, slot);
        }

        public override string GetDisplayProperties()
        {
            Console.WriteLine(AttackDamage);
            return $"\nDamage: {AttackDamage}";
        }
    }
}
