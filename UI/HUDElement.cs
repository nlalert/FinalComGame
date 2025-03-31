// HUD elements - screen space, fixed position
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame;
public abstract class HUDElement : UIElement
{
    public HUDElement(Rectangle bounds) : base(bounds)
    {
    }
    
    public override void Update(GameTime gameTime)
    {
        // Screen-space mouse position check
        MouseState mouseState = Singleton.Instance.CurrentMouseState;
        Point mousePoint = new Point(mouseState.X, mouseState.Y);
        isHovered = bounds.Contains(mousePoint);
        
        if (isHovered)
        {
            isPressed = mouseState.LeftButton == ButtonState.Pressed;
        }
        else
        {
            isPressed = false;
        }
    }
}