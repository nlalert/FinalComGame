// First, let's create a new UIElement for displaying tooltips
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class ItemTooltip : UIElement
{
    private Item item;
    private Texture2D backgroundTexture;
    private Color textColor = Color.White;
    private string _displayText;
    private int padding = 5;
    private float fontScale = 0.5f; // Half the original font size
    
    public ItemTooltip(Item item, Texture2D backgroundTexture) : base(Rectangle.Empty)
    {
        this.item = item;
        this.backgroundTexture = backgroundTexture;
    }
    
    private void UpdateBounds()
    {
        // Build display text using polymorphism
        _displayText = item.Name + "\n" + item.Description + item.GetDisplayProperties();
        
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
        
        // Draw text with scale factor to make it smaller
        spriteBatch.DrawString(
            Singleton.Instance.GameFont,
            _displayText,
            new Vector2(bounds.X + padding, bounds.Y + padding),
            textColor,
            0f,           // rotation
            Vector2.Zero, // origin
            fontScale,    // scale - 0.5f means half size
            SpriteEffects.None,
            0f            // layer depth
        );
    }
}