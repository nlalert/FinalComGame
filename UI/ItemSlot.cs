using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
    public class ItemSlot : UIElement
    {
        protected Player player;
        private Item item;
        private Texture2D slotTexture;
        private Texture2D highlightTexture;
        private SpriteFont font;
        private bool isSelected;
        private float scale;
        private Vector2 itemOffset;

        public ItemSlot(Rectangle bounds, Player player, Texture2D slotTexture, Texture2D highlightTexture, SpriteFont font, float scale = 0.8f) 
            : base(bounds)
        {
            this.slotTexture = slotTexture;
            this.player = player;
            this.highlightTexture = highlightTexture;
            this.font = font;
            this.scale = scale;
            this.isSelected = false;

            // Calculate the offset to center the item in the slot
            float offsetX = bounds.Width * (1 - scale) / 2;
            float offsetY = bounds.Height * (1 - scale) / 2;
            this.itemOffset = new Vector2(offsetX, offsetY);
        }

        public Item GetItem()
        {
            return item;
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
        }

        public bool IsSelected()
        {
            return isSelected;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            item = player.holdItem[0];
            // // Handle clicks
            // MouseState mouseState = Singleton.Instance.CurrentMouseState;
            // MouseState prevMouseState = Singleton.Instance.PreviousMouseState;

            // if (isHovered && mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
            // {
            //     OnClick();
            // }
        }

        protected virtual void OnClick()
        {
            // Override this in derived classes to handle clicks
            // For example, to select this slot in a hotbar
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the slot background
            spriteBatch.Draw(slotTexture, bounds, Color.White);

            // Draw the highlight if selected
            if (isSelected)
            {
                spriteBatch.Draw(highlightTexture, bounds, Color.White);
            }
            else if (isHovered)
            {
                // Draw a hover effect
                spriteBatch.Draw(highlightTexture, bounds, new Color(255, 255, 255, 128));
            }
            if (item != null && item.IsPickedUp)
            {
                item.DrawInSlot(spriteBatch, bounds, scale);
            }
        }
    }
}