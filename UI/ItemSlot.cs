using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    public class ItemSlot : HUDElement
    {
        private int slotNumber;
        private Item item;
        private Texture2D slotTexture;
        private float scale;
        Rectangle _itemSlotRectangle = new Rectangle(192, 112, 32, 32);

        public ItemSlot(int slotNumber, Rectangle bounds, Texture2D slotTexture, float scale = 0.8f) 
            : base(bounds)
        {
            this.slotNumber = slotNumber;
            this.slotTexture = slotTexture;
            this.scale = scale;
        }

        //mark
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            item = Singleton.Instance.Player.Inventory.GetItem(slotNumber);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the slot background
            spriteBatch.Draw(slotTexture, _bounds,_itemSlotRectangle, Color.White);

            if (item != null && item.IsPickedUp)
            {
                item.DrawInSlot(spriteBatch, _bounds, scale);
            }
        }
    }
}