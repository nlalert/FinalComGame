using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Sword : Item
    {
        private float _attackDamage;
        private int _playerInventorySlot;

        public Sword(Texture2D texture, string description, Vector2 Position, float attackDamage = 30f)
            : base(texture, description, Position)
        {
            this._attackDamage = attackDamage;
            IsConsumable = false;
        }

        // // Method to be overridden by specific item types
        public override void OnDrop(Vector2 position)
        {
            Singleton.Instance.Player.ChangeToFistAttack();
            base.OnDrop(position);
        }

        public override void ActiveAbility(int slot)
        {
            _playerInventorySlot = slot;
            Singleton.Instance.Player.ChangeToSwordAttack(_attackDamage);
            base.ActiveAbility(_playerInventorySlot);
        }
    }
}
