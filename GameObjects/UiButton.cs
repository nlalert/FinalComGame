using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace FinalComGame
{
    public class UiButton : HUDElement  // Change UIElement to HUDElement
    {
        private Texture2D _normalTexture;
        private Texture2D _hoverTexture;
        private string _label;
        private Color _textColor;

        public Action<object, EventArgs> OnClick { get; set; }

        public UiButton(Rectangle bounds, Texture2D normalTexture, Texture2D hoverTexture, string label, Color textColor)
            : base(bounds) // Assuming HUDElement takes bounds as a parameter
        {
            _bounds = bounds;
            _normalTexture = normalTexture;
            _hoverTexture = hoverTexture;
            _label = label;
            _textColor = textColor;
        }

        public void SetTexture(Texture2D texture)
        {
            if (texture == _normalTexture)
            {
                _normalTexture = texture;
            }
            else if (texture == _hoverTexture)
            {
                _hoverTexture = texture;
            }
        }

        // Method to check which texture to display
        private Texture2D GetCurrentTexture()
        {
            MouseState mouseState = Mouse.GetState();
            if (_bounds.Contains(mouseState.X, mouseState.Y)) // Check hover
            {
                return _hoverTexture;
            }
            return _normalTexture; // Default normal texture
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GetCurrentTexture(), _bounds, Color.White); // Draw the button

            // Optionally, draw the button label/text
            spriteBatch.DrawString(Singleton.Instance.GameFont, _label, new Vector2(_bounds.X + 10, _bounds.Y + 10), _textColor);
        }

        public void Update()
        {
            MouseState mouseState = Mouse.GetState();
            // Check if the mouse click is within the button's _bounds
            if (_bounds.Contains(mouseState.X, mouseState.Y) && mouseState.LeftButton == ButtonState.Pressed)
            {
                OnClick?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

