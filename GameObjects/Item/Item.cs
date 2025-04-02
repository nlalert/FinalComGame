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
    public SoundEffect UseSound;
    
    // Item properties
    public string Description;
    public bool IsPickedUp;

    public ItemType Type;

    public float MPCost = 0;
    
    // Visual indicator for pickup range
    public float pickupRadius = 40f;
    
    // Tooltip related properties
    private ItemTooltip _toolTip;
    
    // Constructor
    public Item(Texture2D texture, ItemType type)
        : base(texture)
    {
        IsPickedUp = false;
        Type = type;
    }
    
    // // Method to be overridden by specific item types
    public virtual void Use(int slot)
    {
        UseSound?.Play();
        Singleton.Instance.Player.AddUsingItem(slot);
        Console.WriteLine("Using Item");      
    }
    
    public virtual void ActiveAbility(float deltaTime, int slot,List<GameObject> gameObject)
    {    
    }

    public virtual void OnSpawn()
    {
        // Create tooltip
        _toolTip = new ItemTooltip(this, TooltipBackgroundTexture);
        Singleton.Instance.CurrentUI.AddWorldSpaceElement(_toolTip);
    }

    public virtual void OnPickup(int slot)
    {
        Singleton.Instance.Player.Inventory.AddItem(this, slot);
        PickUpSound.Play();
        IsPickedUp = true;
    }

    // Called when item is dropped
    public virtual void OnDrop(Vector2 position)
    {
        IsPickedUp = false;
        Position = position;
    }

    public virtual void RemoveItem()
    {
        IsActive = false;
        Singleton.Instance.CurrentUI.RemoveWorldSpaceElement(_toolTip);
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
           
        base.Update(gameTime, gameObjects, tileMap);
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        if(!IsPickedUp)
        {
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
            Viewport,
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