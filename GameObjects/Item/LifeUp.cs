using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class LifeUp : Item
    {
        public LifeUp(Texture2D texture, ItemType type)
            : base(texture, type)
        {
        }
        
        public override void OnPickup(int slot)
        {
            base.OnPickup(slot);
            Use(slot);
        }

        public override void ActiveAbility(float deltaTime, int slot,List<GameObject> gameObjects)
        {
            Singleton.Instance.Player.Life++;
            RemoveItem();
        }
    }
}
