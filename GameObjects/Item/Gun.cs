using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Gun : Item
    {
        private float _attackDamage;
        // private int _ammoAmount;

        public Gun(Texture2D texture, string description, Vector2 Position, float attackDamage = 100f, int ammoAmount = 6)
            : base(texture, description, Position)
        {
            _attackDamage = attackDamage;
            // _ammoAmount = ammoAmount;
            IsConsumable = false;
        }

        // // Method to be overridden by specific item types
        public override void OnDrop(Vector2 position)
        {
            Singleton.Instance.Player.ChangeToSoulBulletAttack();
            base.OnDrop(position);
        }

        public override void ActiveAbility()
        {
            Singleton.Instance.Player.ChangeToGunAttack(_attackDamage);
            base.ActiveAbility();
        }
    }
}
