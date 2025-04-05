using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class TextUI : HUDElement
{
    private string _text;
    private Color _color;
    private Func<string> _dynamicTextProvider;
    private Vector2 _textPosition;
    private TextAlignment _alignment;
    private float _textScale;
    
    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }

    // Constructor for static text
    public TextUI(Rectangle bounds, string text, float scale = 1,Color color = default, 
        TextAlignment alignment = TextAlignment.Left) 
        : base(bounds)
    {
        _text = text;
        _color = color == default ? Color.White : color;
        _alignment = alignment;
        _textScale = scale;
        UpdateTextPosition();
    }

    // Constructor for dynamic text (using a function)
    public TextUI(Rectangle bounds, Func<string> textProvider, float scale = 1, Color color = default, 
        TextAlignment alignment = TextAlignment.Left)
        : base(bounds)
    {
        _dynamicTextProvider = textProvider;
        _text = textProvider();
        _color = color == default ? Color.White : color;
        _alignment = alignment;
        _textScale = scale;
        UpdateTextPosition();
    }

    // Calculate the position of the text based on alignment
    private void UpdateTextPosition()
    {
        string textToMeasure = _text ?? (_dynamicTextProvider != null ? _dynamicTextProvider() : "");
        Vector2 textSize = Singleton.Instance.GameFont.MeasureString(textToMeasure) * _textScale;

        switch (_alignment)
        {
            case TextAlignment.Left:
                _textPosition = new Vector2(_bounds.X, _bounds.Y);
                break;
            case TextAlignment.Center:
                _textPosition = new Vector2(
                    _bounds.X + (_bounds.Width - textSize.X) / 2,
                    _bounds.Y + (_bounds.Height - textSize.Y) / 2);
                break;
            case TextAlignment.Right:
                _textPosition = new Vector2(
                    _bounds.X + _bounds.Width - textSize.X,
                    _bounds.Y);
                break;
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Update the text if using a dynamic provider
        if (_dynamicTextProvider != null)
        {
            string newText = _dynamicTextProvider();
            if (newText != _text)
            {
                _text = newText;
                UpdateTextPosition();
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (_text != null)
        {
            spriteBatch.DrawString(
                Singleton.Instance.GameFont, 
                _text, 
                _textPosition, 
                _color,
                0f,
                Vector2.Zero,
                _textScale,
                SpriteEffects.None,
                0f
            );
        }
        
        base.Draw(spriteBatch);
    }
}