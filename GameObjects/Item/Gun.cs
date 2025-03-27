using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Gun : Item
    {
        private float _attackDamage;
        private int _ammoAmount;
        private int _playerInventorySlot;

        public Gun(Texture2D texture, string description, Vector2 Position, float attackDamage = 100f, int ammoAmount = 6)
            : base(texture, description, Position)
        {
            _attackDamage = attackDamage;
            _ammoAmount = ammoAmount;
            IsConsumable = false;
        }

        public void DecreaseAmmo()
        {
            _ammoAmount--;
            Console.WriteLine("Ammo "+ _ammoAmount);
            if(_ammoAmount <= 0)
            {
                Singleton.Instance.Player.RemoveItem(_playerInventorySlot);
                Singleton.Instance.Player.ChangeToSoulBulletAttack();
            }
        }
        
        public override void OnDrop(Vector2 position)
        {
            Singleton.Instance.Player.ChangeToSoulBulletAttack();
            base.OnDrop(position);
        }

        public override void ActiveAbility(int slot)
        {
            _playerInventorySlot = slot;
            Singleton.Instance.Player.ChangeToGunAttack(_attackDamage, _playerInventorySlot);
            base.ActiveAbility(_playerInventorySlot);
        }
    }
}
