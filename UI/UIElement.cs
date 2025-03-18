
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame
{
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
        
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}