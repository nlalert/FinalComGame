// HUD elements - screen space, fixed position
using System;
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
        _isHovered = _bounds.Contains(Singleton.Instance.GetMousePosition());
        
        if (_isHovered)
        {
            _isPressed = Singleton.Instance.CurrentMouseState.LeftButton == ButtonState.Pressed;
        }
        else
        {
            _isPressed = false;
        }
    }
}