using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class SpeedBoots : Item
    {
        private float _speedModifier;

        public SpeedBoots(Texture2D texture, string description, Vector2 Position, float speedModifier = 0.5f)
            : base(texture, description, Position)
        {
            this._speedModifier = speedModifier;
            IsConsumable = false;
        }
        
        // // Method to be overridden by specific item types
        public override void ActiveAbility(int slot)
        {
            Singleton.Instance.Player.BoostSpeed(_speedModifier);
            base.ActiveAbility(slot);
        }
    }
}
