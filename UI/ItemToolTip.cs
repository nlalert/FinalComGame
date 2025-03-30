// First, let's create a new UIElement for displaying tooltips
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class ItemTooltip : UIElement
{
    private Item item;
    private Texture2D backgroundTexture;
    private Color backgroundColor = new Color(0, 0, 0, 180); // Semi-transparent black
    private Color textColor = Color.White;
    private int padding = 5;
    
    public ItemTooltip(Item item, Texture2D backgroundTexture) : base(Rectangle.Empty)
    {
        this.item = item;

        this.backgroundTexture = backgroundTexture;
    }
    
    private void UpdateBounds()
    {
        // Measure the text
        Vector2 textSize = Singleton.Instance.GameFont.MeasureString(item.Description);
        
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
        // Draw background
        spriteBatch.Draw(
            backgroundTexture,
            bounds,
            backgroundColor
        );
        
        // Draw text
        spriteBatch.DrawString(
            Singleton.Instance.GameFont,
            item.Description,
            new Vector2(bounds.X + padding, bounds.Y + padding),
            textColor
        );
    }
}
