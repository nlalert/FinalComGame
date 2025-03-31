using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public interface IItemDisplayable
{
    string GetDisplayProperties();
}

public class Item : GameObject, IItemDisplayable
{
    public static Texture2D TooltipBackgroundTexture;
    public static SoundEffect PickUpSound;
    
    // Item properties
    public string Description;
    public bool IsPickedUp;

    public Rectangle spriteViewport = new Rectangle(0, 0, 32,32);
    public ItemType Type;
    
    // Visual indicator for pickup range
    public float pickupRadius = 40f;
    protected Vector2 originalPosition;
    
    // Tooltip related properties
    private bool isPlayerNearby = false;
    private ItemTooltip tooltip;
    private const float TOOLTIP_FADE_SPEED = 5f; // Fade speed per second
    
    // Constructor
    public Item(Texture2D texture, Vector2 Position, ItemType type)
        : base(texture)
    {
        IsPickedUp = false;
        Type = type;

        this.Position = Position;
        originalPosition = Position;
        
        // Create tooltip
        tooltip = new ItemTooltip(this, TooltipBackgroundTexture);
    }
    
    // // Method to be overridden by specific item types
    public virtual void Use(int slot)
    {
        Singleton.Instance.Player.AddUsingItem(slot);
        Console.WriteLine("Using Item");      
    }
    
    public virtual void ActiveAbility(float deltaTime, int slot)
    {    
    }
    
    // Called when item is picked up
    public virtual void OnPickup(int slot)
    {
        Singleton.Instance.Player.ItemSlot[slot] = this;
        PickUpSound.Play();
        IsPickedUp = true;
    }
    
    // Called when item is dropped
    public virtual void OnDrop(Vector2 position)
    {
        IsPickedUp = false;
        Position = position;
        originalPosition = position;
    }

    public virtual string GetDisplayProperties()
    {
        return ""; // Default is no additional properties
    }
    
    public override void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
    {
        if (IsPickedUp) return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        ApplyGravity(deltaTime);
        UpdateVerticalMovement(deltaTime, gameObjects, tileMap);
        
        // Check if player is in pickup radius
        bool wasPlayerNearby = isPlayerNearby;
        isPlayerNearby = InPickupRadius();
        
        // // Update tooltip fade
        // if (isPlayerNearby)
        // {
        //     tooltipFadeIn = Math.Min(1f, tooltipFadeIn + TOOLTIP_FADE_SPEED * deltaTime);
        // }
        // else
        // {
        //     tooltipFadeIn = Math.Max(0f, tooltipFadeIn - TOOLTIP_FADE_SPEED * deltaTime);
        // }
        
        // Update tooltip
        tooltip.Update(gameTime);
        
        base.Update(gameTime, gameObjects, tileMap);
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        if(!IsPickedUp)
        {
            spriteBatch.Draw(
                _texture,
                Position,
                spriteViewport,
                Color.White,
                0f, 
                Vector2.Zero,
                1f,
                SpriteEffects.None, 
                0f
            );

            if(InPickupRadius())
                tooltip.Draw(spriteBatch); 
        }
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
            spriteViewport,
            Color.White,
            0f, 
            Vector2.Zero,
            SpriteEffects.None, 
            0f
        );
    }

    public bool InPickupRadius()
    {
        float distance = Vector2.Distance(Position, Singleton.Instance.Player.Position);
        return distance < pickupRadius;
    }
}