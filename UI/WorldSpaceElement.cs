using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame;
public abstract class WorldSpaceUIElement : UIElement
{
    protected Vector2 worldPosition;
    
    public WorldSpaceUIElement(Rectangle bounds, Vector2 worldPosition) : base(bounds)
    {
        this.worldPosition = worldPosition;
    }
    
    public override void Update(GameTime gameTime)
    {
        // // Convert mouse from screen space to world space for hovering check
        // Microsoft.Xna.Framework.Input.MouseState mouseState = Singleton.Instance.CurrentMouseState;
        // Vector2 screenMousePos = new Vector2(mouseState.X, mouseState.Y);
        
        // // Convert mouse position to world space using inverse camera transform
        // Matrix inverseViewMatrix = Matrix.Invert(Singleton.Instance.Camera.GetTransformation());
        // Vector2 worldMousePos = Vector2.Transform(screenMousePos, inverseViewMatrix);
        
        // // Create a rectangle in world space
        // Rectangle worldBounds = new Rectangle(
        //     (int)worldPosition.X, 
        //     (int)worldPosition.Y,
        //     bounds.Width, 
        //     bounds.Height
        // );
        
        // isHovered = worldBounds.Contains(new Point((int)worldMousePos.X, (int)worldMousePos.Y));
        
        // if (isHovered)
        // {
        //     isPressed = mouseState.LeftButton == ButtonState.Pressed;
        // }
        // else
        // {
        //     isPressed = false;
        // }
    }
}