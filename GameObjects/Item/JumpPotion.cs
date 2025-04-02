using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class JumpPotion : Item
    {
        private float _jumpStrengthModifier;
        private float _jumpBoostDuration;
        private float _jumpBoostTimer;

        public JumpPotion(Texture2D texture, ItemType type, float jumpModifier = 1.5f, float jumpBoostDuration = 5.0f)
            : base(texture, type)
        {
            _jumpStrengthModifier = jumpModifier;
            _jumpBoostDuration = jumpBoostDuration;
            _jumpBoostTimer = 0;
        }

        public override void ActiveAbility(float deltaTime, int slot,List<GameObject> gameObjects)
        {
            if(_jumpBoostTimer > 0)
            {
                _jumpBoostTimer -= deltaTime;
                Singleton.Instance.Player.BoostJump(_jumpStrengthModifier);
            }
            else if(_jumpBoostTimer < 0){
                Singleton.Instance.Player.ResetJumpStrength();
                RemoveItem();
            }
        }
        // // Method to be overridden by specific item types
        public override void Use(int slot)
        {
            _jumpBoostTimer = _jumpBoostDuration;
            base.Use(slot);
        }
    }
}
