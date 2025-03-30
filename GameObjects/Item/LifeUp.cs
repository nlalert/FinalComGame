using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class LifeUp : Item
    {
        public LifeUp(Texture2D texture, ItemType type, Vector2 Position)
            : base(texture, Position, type)
        {
        }
        
        public override void OnPickup(int slot)
        {
            base.OnPickup(slot);
            Use(slot);
        }

        public override void ActiveAbility(float deltaTime, int slot)
        {
            Singleton.Instance.Player.Life++;
            IsActive = false;
        }
    }
}
