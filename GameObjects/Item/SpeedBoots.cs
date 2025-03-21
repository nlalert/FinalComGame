using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class SpeedBoots : Item
    {
        private float speedModifier;

        public SpeedBoots(Texture2D texture, string description, Vector2 Position, float speedModifier = 0.5f)
            : base(texture, description, Position)
        {
            this.speedModifier = speedModifier;
            IsConsumable = false;
        }
        
        // // Method to be overridden by specific item types
        public override void ActiveAbility(Player player)
        {
            player.BoostSpeed(speedModifier);
            base.ActiveAbility(player);
        }
    }
}
