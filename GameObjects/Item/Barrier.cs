using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Barrier : Item
    {
        private int _damageAbsorptionAmount;

        public Barrier(Texture2D texture, ItemType type, Vector2 Position, int damageAbsorptionAmount = 50)
            : base(texture, Position, type)
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
