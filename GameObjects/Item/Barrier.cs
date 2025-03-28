using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Barrier : Item
    {
        private int _damageAbsorptionAmount;

        public Barrier(Texture2D texture, ItemType type, string description, Vector2 Position, int damageAbsorptionAmount = 50)
            : base(texture, description, Position, type)
        {
            _damageAbsorptionAmount = damageAbsorptionAmount;
        }
        
        public override void OnPickup(int slot)
        {
            base.OnPickup(slot);
            Use(slot);
        }

        public override void Use(int slot)
        {
            Singleton.Instance.Player.AbsorptionHealth = _damageAbsorptionAmount;
            base.Use(slot);
        }
    }
}
