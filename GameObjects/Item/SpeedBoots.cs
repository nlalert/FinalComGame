using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class SpeedBoots : Item
    {
        private float _speedModifier;

        public SpeedBoots(Texture2D texture, ItemType type, string description, Vector2 Position, float speedModifier = 1.5f)
            : base(texture, description, Position, type)
        {
            this._speedModifier = speedModifier;
        }
        
        // // Method to be overridden by specific item types
        public override void ActiveAbility(float deltaTime, int slot)
        {
            Singleton.Instance.Player.BoostSpeed(_speedModifier);
            base.ActiveAbility(deltaTime, slot);
        }
    }
}
