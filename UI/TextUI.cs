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
    
    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }

    // Constructor for static text
    public TextUI(Rectangle bounds, string text, Color color = default, 
        TextAlignment alignment = TextAlignment.Left) 
        : base(bounds)
    {
        _text = text;
        _color = color == default ? Color.White : color;
        _alignment = alignment;
        UpdateTextPosition();
    }

    // Constructor for dynamic text (using a function)
    public TextUI(Rectangle bounds, Func<string> textProvider, Color color = default, 
        TextAlignment alignment = TextAlignment.Left)
        : base(bounds)
    {
        _dynamicTextProvider = textProvider;
        _text = textProvider();
        _color = color == default ? Color.White : color;
        _alignment = alignment;
        UpdateTextPosition();
    }

    // Set a new static text
    public void SetText(string text)
    {
        _text = text;
        _dynamicTextProvider = null;
        UpdateTextPosition();
    }

    // Set a dynamic text provider function
    public void SetDynamicTextProvider(Func<string> textProvider)
    {
        _dynamicTextProvider = textProvider;
        UpdateTextPosition();
    }

    // Set the text color
    public void SetColor(Color color)
    {
        _color = color;
    }

    // Set the text alignment
    public void SetAlignment(TextAlignment alignment)
    {
        _alignment = alignment;
        UpdateTextPosition();
    }


    // Calculate the position of the text based on alignment
    private void UpdateTextPosition()
    {
        string textToMeasure = _text ?? (_dynamicTextProvider != null ? _dynamicTextProvider() : "");
        Vector2 textSize = Singleton.Instance.GameFont.MeasureString(textToMeasure);

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
            spriteBatch.DrawString(Singleton.Instance.GameFont, _text, _textPosition, _color);
        }
        
        base.Draw(spriteBatch);
    }
}
