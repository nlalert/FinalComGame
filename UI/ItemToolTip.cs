// First, let's create a new UIElement for displaying tooltips
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;
public class ItemTooltip : WorldSpaceUIElement
{
    private Item item;
    private Texture2D backgroundTexture;
    private Color textColor = Color.White;
    private string _displayText;
    private int padding = 10;
    private float fontScale = 0.5f; // Half the original font size
    
    public ItemTooltip(Item item, Texture2D backgroundTexture) 
        : base(Rectangle.Empty, new Vector2(item.Position.X, item.Position.Y))
    {
        this.item = item;
        this.backgroundTexture = backgroundTexture;
        UpdateBounds();
    }
    
    private void UpdateBounds()
    {
        // Split the display text into title and content
        string itemName = item.Name;
        string itemDetails = "\n\n" + item.Description + item.GetDisplayProperties() + "\n\nPress " + Singleton.Instance.Player.Interact + " to pick up";
        
        // Build full display text for size measurement
        _displayText = itemName + itemDetails;
        
        // Measure the text with scaling applied
        Vector2 textSize = Singleton.Instance.GameFont.MeasureString(_displayText) * fontScale;
        
        // Calculate tooltip position (centered above the item)
        Vector2 position = new Vector2(
            item.Position.X + item.Rectangle.Width / 2 - textSize.X / 2,
            item.Position.Y - textSize.Y - 20
        );
        
        // Set bounds with padding
        bounds = new Rectangle(
            (int)position.X - padding,
            (int)position.Y - padding,
            (int)textSize.X + padding * 2,
            (int)textSize.Y + padding * 2
        );
    }
    public override void Update(GameTime gameTime)
    {
        // Update position if item moves
        UpdateBounds();
        base.Update(gameTime);
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        if(_displayText == null) return;

        // Draw background
        spriteBatch.Draw(
            backgroundTexture,
            bounds,
            Color.White
        );
        
        // Split the display text into title and content
        string itemName = item.Name;
        string itemDetails = "\n" + item.Description + item.GetDisplayProperties() + "\n\nPress " + Singleton.Instance.Player.Interact + " to pick up";
        
        // Measure the item name and full content
        Vector2 nameSize = Singleton.Instance.GameFont.MeasureString(itemName) * fontScale;
        
        // Calculate positions
        Vector2 namePosition = new Vector2(
            bounds.X + bounds.Width / 2 - nameSize.X / 2, // Center horizontally
            bounds.Y + padding // Keep at top
        );
        
        Vector2 detailsPosition = new Vector2(
            bounds.X + padding, // Left align
            bounds.Y + padding + nameSize.Y // Position below the title
        );
        
        // Draw item name (centered)
        spriteBatch.DrawString(
            Singleton.Instance.GameFont,
            itemName,
            namePosition,
            textColor,
            0f,
            Vector2.Zero,
            fontScale,
            SpriteEffects.None,
            0f
        );
        
        // Draw item details (left-aligned)
        spriteBatch.DrawString(
            Singleton.Instance.GameFont,
            itemDetails,
            detailsPosition,
            textColor,
            0f,
            Vector2.Zero,
            fontScale,
            SpriteEffects.None,
            0f
        );
    }
}