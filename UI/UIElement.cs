
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame;

// Abstract base class for all UI elements
public abstract class UIElement
{
    protected Rectangle bounds;
    protected bool isHovered;
    protected bool isPressed;
    
    public UIElement(Rectangle bounds)
    {
        this.bounds = bounds;
    }
    
    public virtual void Update(GameTime gameTime)
    {
        // Base update code - to be overridden by subclasses
    }
    
    public abstract void Draw(SpriteBatch spriteBatch);
}