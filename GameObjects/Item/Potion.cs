using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Potion : Item
    {
        private int _healAmount;

        public Potion(Texture2D texture, ItemType type, int healAmount = 30)
            : base(texture, type)
        {
            this._healAmount = healAmount;
        }

        public override void ActiveAbility(float deltaTime, int slot,List<GameObject> gameObjects)
        {
            Singleton.Instance.Player.Health += _healAmount;
            Singleton.Instance.Player.Health = Math.Min(Singleton.Instance.Player.Health, Singleton.Instance.Player.MaxHealth);
            RemoveItem();
        }
    }
}
