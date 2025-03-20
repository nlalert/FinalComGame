using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class Item : GameObject
    {
        // Item properties
        public string Description { get; protected set; }
        public bool IsPickedUp { get; protected set; }
        public bool IsConsumable { get; protected set; }
        
        // Visual indicator for pickup range
        protected float pickupRadius = 40f;
        protected bool isInRange = false;
        
        protected float bobAmount = 4f;
        protected float bobSpeed = 2f;
        protected float bobTimer = 0f;
        protected Vector2 originalPosition;
        
        // Constructor
        public Item(Texture2D texture, string description, bool isConsumable, Vector2 Position)
            : base(texture)
        {
            Description = description;
            IsConsumable = isConsumable;
            IsPickedUp = false;
            
            originalPosition = Position;
        }
        
        // // Method to be overridden by specific item types
        public virtual void Use(Player player)
        {
        }
        
        // Called when item is picked up
        public virtual void OnPickup(Player player)
        {
            IsPickedUp = true;
        }
        
        // Called when item is dropped
        public virtual void OnDrop(Vector2 position)
        {
            IsPickedUp = false;
            IsActive = true;
            Position = position;
            originalPosition = position;
        }
        
        public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            if (IsPickedUp) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Animate item bobbing
            bobTimer += deltaTime * bobSpeed;
            Position = originalPosition + new Vector2(0, (float)Math.Sin(bobTimer) * bobAmount);
            
            base.Update(gameTime, gameObjects, tileMap);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            if(IsPickedUp) return;

            spriteBatch.Draw(
                _texture,
                Position,
                Viewport,
                Color.White,
                0f, 
                Vector2.Zero,
                1f,
                SpriteEffects.None, 
                0f
            );

            // // Draw pickup range indicator when player is nearby
            // if (isInRange)
            // {
            //     // Draw a circle around the item to indicate it can be picked up
            //     // This is a simplified approach - you might want to use a proper circle texture
            //     Texture2D rangeIndicator = Singleton.Instance.Content.Load<Texture2D>("rangeIndicator");
            //     spriteBatch.Draw(
            //         rangeIndicator,
            //         new Rectangle(
            //             (int)(Position.X + Rectangle.Width / 2 - pickupRadius),
            //             (int)(Position.Y + Rectangle.Height / 2 - pickupRadius),
            //             (int)(pickupRadius * 2),
            //             (int)(pickupRadius * 2)
            //         ),
            //         rangeIndicatorColor
            //     );
                
            //     // Draw item name above
            //     Singleton.Instance.DrawText(
            //         spriteBatch,
            //         ItemName,
            //         new Vector2(Position.X + Rectangle.Width / 2, Position.Y - 20),
            //         Color.White,
            //         true
            //     );
            // }
        }

        // Add this method to the Item class
        public void DrawInSlot(SpriteBatch spriteBatch, Rectangle slotBounds, float scale = 1.0f)
        {
            Rectangle destinationRect = new Rectangle(
                slotBounds.X + (int)(slotBounds.Width * (1 - scale) / 2),
                slotBounds.Y + (int)(slotBounds.Height * (1 - scale) / 2),
                (int)(slotBounds.Width * scale),
                (int)(slotBounds.Height * scale)
            );

            spriteBatch.Draw(
                _texture,
                destinationRect,
                Viewport,
                Color.White,
                0f, 
                Vector2.Zero,
                SpriteEffects.None, 
                0f
            );
        }

        public bool InPickupRadius(Player player)
        {
            float distance = Vector2.Distance(Position, player.Position);
            return distance < pickupRadius;
        }
    }
}
