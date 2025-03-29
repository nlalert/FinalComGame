using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class SpeedPotion : Item
    {
        private float _speedModifier;
        private float _speedBoostDuration;
        private float _speedBoostTimer;

        public SpeedPotion(Texture2D texture, ItemType type, string description, Vector2 Position, float speedModifier = 0.5f, float speedBoostDuration = 5.0f)
            : base(texture, description, Position, type)
        {
            _speedModifier = speedModifier;
            _speedBoostDuration = speedBoostDuration;
            _speedBoostTimer = 0;
        }

        public override void ActiveAbility(float deltaTime, int slot)
        {
            if(_speedBoostTimer > 0)
            {
                _speedBoostTimer -= deltaTime;
                Singleton.Instance.Player.BoostSpeed(_speedModifier);

            }
            else if(_speedBoostTimer < 0){
                IsActive = false;
            }
        }
        // // Method to be overridden by specific item types
        public override void Use(int slot)
        {
            _speedBoostTimer = _speedBoostDuration;
            base.Use(slot);
        }
    }
}
