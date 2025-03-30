using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Sword : Item
    {
        private float _attackDamage;
        private int _attackWidth;
        private int _attackHeight;

        public Sword(Texture2D texture, ItemType type, Vector2 Position, float attackDamage = 30f, int attackWidth = 32, int attackHeight = 32)
            : base(texture, Position, type)
        {
            _attackDamage = attackDamage;
            _attackWidth = attackWidth;
            _attackHeight = attackHeight;
        }

        // // Method to be overridden by specific item types
        public override void OnDrop(Vector2 position)
        {
            Singleton.Instance.Player.ChangeToFistAttack();
            base.OnDrop(position);
        }

        public Rectangle GetAttackHitbox()
        {
            int offsetX = Singleton.Instance.Player.Direction == 1 ? Singleton.Instance.Player.Rectangle.Width : -_attackWidth;

            return new Rectangle((int)Singleton.Instance.Player.Position.X + offsetX, (int)Singleton.Instance.Player.Position.Y, _attackWidth, _attackHeight);
        }

        public override void ActiveAbility(float deltaTime, int slot)
        {
            Singleton.Instance.Player.ChangeToSwordAttack(_attackDamage);
            base.ActiveAbility(deltaTime, slot);
        }
    }
}
