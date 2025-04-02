using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class CursedGauntlet : Item
    {
        private float _MPUsageRateModifier;
        private float _damageModifier;
        private float _speedModifier;
        private bool _isActivated;

        public CursedGauntlet(Texture2D texture, ItemType type, 
            float MPUsageRateModifier = 1.25f, float DamageModifier = 2.0f, float SpeedModifier = 0.7f)
            : base(texture, type)
        {
            _MPUsageRateModifier = MPUsageRateModifier;
            _damageModifier = DamageModifier;
            _speedModifier = SpeedModifier;
            _isActivated = false;
        }

        public override bool OnDrop(Vector2 position)
        {
            return false; //can't drop
        }
        
        // // Method to be overridden by specific item types
        public override void ActiveAbility(float deltaTime, int slot,List<GameObject> gameObjects)
        {
            if(!_isActivated)
            {
                Singleton.Instance.Player.MPRegenRate /= _MPUsageRateModifier;
                Singleton.Instance.Player.DashMP *= _MPUsageRateModifier;

                Singleton.Instance.Player.AttackDamage *= _damageModifier;
                Singleton.Instance.Player.MaxChargePower *= _damageModifier;

                _isActivated = true;
            }

            Singleton.Instance.Player.BoostSpeed(_speedModifier);
            
            base.ActiveAbility(deltaTime, slot, gameObjects);
        }
    }
}
