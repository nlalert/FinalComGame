using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Barrier : Item
    {
        private int _damageAbsorptionAmount;

        public Barrier(Texture2D texture, ItemType type, int damageAbsorptionAmount = 40)
            : base(texture, type)
        {
            _damageAbsorptionAmount = damageAbsorptionAmount;
        }
        
        public override void OnPickup(int slot)
        {
            base.OnPickup(slot);
            Use(slot);
        }

        public override void ActiveAbility(float deltaTime, int slot,List<GameObject> gameObjects)
        {
            Singleton.Instance.Player.AbsorptionHealth = _damageAbsorptionAmount;
            RemoveItem();
        }
    }
}
