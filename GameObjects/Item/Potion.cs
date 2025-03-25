using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Potion : Item
    {
        private int _healAmount;

        public Potion(Texture2D texture, string description, Vector2 Position, int healAmount = 30)
            : base(texture, description, Position)
        {
            this._healAmount = healAmount;
            IsConsumable = true;
        }
        
        // // Method to be overridden by specific item types
        public override void Use(int slot)
        {
            Singleton.Instance.Player.Health += _healAmount;
            Singleton.Instance.Player.Health = Math.Min(Singleton.Instance.Player.Health, Singleton.Instance.Player.MaxHealth);
            
            base.Use(slot);
        }
    }
}
