using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Potion : Item
    {
        private int healAmount;

        public Potion(Texture2D texture, string description, Vector2 Position, int healAmount = 30)
            : base(texture, description, Position)
        {
            this.healAmount = healAmount;
            IsConsumable = true;
        }
        
        // // Method to be overridden by specific item types
        public override void Use(Player player)
        {
            player.Health += healAmount;
            player.Health = Math.Min(player.Health, player.maxHealth);
            
            base.Use(player);
        }
    }
}
