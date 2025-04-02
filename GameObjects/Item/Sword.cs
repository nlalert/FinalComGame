using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Sword : Item
    {
        public float AttackDamage;
        private int _attackWidth;
        private int _attackHeight;

        public SoundEffect SlashSound;

        public Sword(Texture2D texture, ItemType type, float attackDamage = 30f, int attackWidth = Singleton.TILE_SIZE * 2, int attackHeight = Singleton.TILE_SIZE * 2)
            : base(texture,  type)
        {
            AttackDamage = attackDamage;
            _attackWidth = attackWidth;
            _attackHeight = attackHeight;
        }

        // // Method to be overridden by specific item types
        public override bool OnDrop(Vector2 position)
        {
            Singleton.Instance.Player.ChangeToFistAttack();
            return base.OnDrop(position);
        }

        public Rectangle GetAttackHitbox()
        {
            int offsetX = Singleton.Instance.Player.Direction == 1 ? Singleton.Instance.Player.Rectangle.Width : -_attackWidth;

            return new Rectangle((int)Singleton.Instance.Player.Position.X + offsetX, (int)Singleton.Instance.Player.Position.Y, _attackWidth, _attackHeight);
        }

        public override void ActiveAbility(float deltaTime, int slot,List<GameObject> gameObjects)
        {
            Singleton.Instance.Player.ChangeToSwordAttack(AttackDamage);
            base.ActiveAbility(deltaTime, slot, gameObjects);
        }

        public override string GetDisplayProperties()
        {
            return $"\nDamage: {AttackDamage}";
        }
    }
}
