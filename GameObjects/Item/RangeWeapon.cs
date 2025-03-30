using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class RangeWeapon : Item
    {
        private float _attackDamage;
        private int _ammoAmount;

        public RangeWeapon(Texture2D texture, ItemType type, Vector2 Position, float attackDamage, int ammoAmount)
            : base(texture, Position, type)
        {
            _attackDamage = attackDamage;
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
            Singleton.Instance.Player.ChangeToRangeWeaponAttack(_attackDamage);
            base.ActiveAbility(deltaTime, slot);
        }
    }
}
