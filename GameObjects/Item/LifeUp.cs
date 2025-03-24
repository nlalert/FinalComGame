using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class LifeUp : Item
    {
        public LifeUp(Texture2D texture, string description, Vector2 Position)
            : base(texture, description, Position)
        {
            IsConsumable = true;
        }
        public override void OnPickup(int slot)
        {
            base.OnPickup(slot);
            Use(slot);
        }
        // // Method to be overridden by specific item types
        public override void Use(int slot)
        {
            Singleton.Instance.Player.Life++;
            base.Use(slot);
        }
    }
}
