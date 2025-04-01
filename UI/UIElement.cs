
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame;

// Abstract base class for all UI elements
public class UIElement
{
    protected Rectangle _bounds;
    protected bool _isHovered;
    protected bool _isPressed;
    
    public UIElement(Rectangle bounds)
    {
        this._bounds = bounds;
    }
    
    public virtual void Update(GameTime gameTime)
    {
    }
    
    public virtual void Draw(SpriteBatch spriteBatch)
    {
    }
}