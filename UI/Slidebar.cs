using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FinalComGame;

public class SlideBarUI : HUDElement
{
    // UI Components
    private TextUI _labelText;
    private TextUI _valueText;
    private Rectangle _barBounds;
    private Rectangle _handleBounds;
    
    // Textures
    private Texture2D _barTexture;
    private Texture2D _handleTexture;
    
    // Values
    private float _minValue;
    private float _maxValue;
    private float _currentValue;
    private string _labelFormat;
    private string _valueFormat;
    private bool _isDragging;
    private int _handleWidth;
    private Color _barColor;
    private Color _textColor;
    
    // Customization
    private int _barHeight;
    private int _labelWidth;

    public SlideBarUI(
        Rectangle bounds,
        string label,
        Texture2D barTexture,
        Texture2D handleTexture,
        float minValue = 0,
        float maxValue = 100,
        float startValue = 50,
        string valueFormat = "{0:F0}")
        : base(bounds)
    {
        _barTexture = barTexture;
        _handleTexture = handleTexture;
        _minValue = minValue;
        _maxValue = maxValue;
        _currentValue = MathHelper.Clamp(startValue, minValue, maxValue);
        _labelFormat = label;
        _valueFormat = valueFormat;
        _barColor = Color.White;
        _textColor = Color.White;
        _isDragging = false;
        _handleWidth = handleTexture.Width;
        _barHeight = 24;
        _labelWidth = 150;
        
        // Calculate layout
        InitializeLayout();
    }

    private void InitializeLayout()
    {
        // Calculate label and value text positions
        int leftTextWidth = _labelWidth;
        int rightTextWidth = 80;
        int centerWidth = _bounds.Width - leftTextWidth - rightTextWidth;
        
        // Label on left
        Rectangle labelBounds = new Rectangle(
            _bounds.X,
            _bounds.Y,
            leftTextWidth,
            _bounds.Height
        );
        
        // Value on right
        Rectangle valueBounds = new Rectangle(
            _bounds.X + _bounds.Width - rightTextWidth,
            _bounds.Y,
            rightTextWidth,
            _bounds.Height
        );
        
        // Slider bar in center
        _barBounds = new Rectangle(
            _bounds.X + leftTextWidth,
            _bounds.Y + (_bounds.Height - _barHeight) / 2,
            centerWidth,
            _barHeight
        );
        
        // Create text elements
        _labelText = new TextUI(
            labelBounds,
            _labelFormat,
            _textColor,
            TextUI.TextAlignment.Left
        );
        
        _valueText = new TextUI(
            valueBounds,
            () => string.Format(_valueFormat, _currentValue),
            _textColor,
            TextUI.TextAlignment.Right
        );
        
        // Initialize handle position
        UpdateHandlePosition();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        // Update child UI elements
        _labelText.Update(gameTime);
        _valueText.Update(gameTime);
        
        // Check for mouse interactions with handle
        Point mousePos = Singleton.Instance.GetMousePosition();
        bool isMouseOverHandle = _handleBounds.Contains(mousePos);
        
        // Handle mouse click and drag
        if (Singleton.Instance.IsMouseButtonJustPressed(Singleton.MouseButton.Left) && isMouseOverHandle)
        {
            _isDragging = true;
        }
        else if (Singleton.Instance.IsMouseButtonReleased(Singleton.MouseButton.Left))
        {
            _isDragging = false;
        }
        
        // Update value while dragging
        if (_isDragging)
        {
            float relativeX = MathHelper.Clamp(mousePos.X - _barBounds.X, 0, _barBounds.Width - _handleWidth);
            float valuePercent = relativeX / (_barBounds.Width - _handleWidth);
            _currentValue = _minValue + valuePercent * (_maxValue - _minValue);
            UpdateHandlePosition();
        }
    }

    // Update handle position based on current value
    private void UpdateHandlePosition()
    {
        float valueRange = _maxValue - _minValue;
        float valuePercent = (_currentValue - _minValue) / valueRange;
        
        // Calculate handle position
        int handleX = (int)(_barBounds.X + valuePercent * (_barBounds.Width - _handleWidth));
        int handleY = _barBounds.Y + (_barBounds.Height - _handleTexture.Height) / 2;
        
        _handleBounds = new Rectangle(
            handleX,
            handleY,
            _handleWidth,
            _handleTexture.Height
        );
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Draw bar background
        spriteBatch.Draw(_barTexture, _barBounds, _barColor);
        
        // Draw handle
        spriteBatch.Draw(_handleTexture, _handleBounds, Color.White);
        
        // Draw text elements
        _labelText.Draw(spriteBatch);
        _valueText.Draw(spriteBatch);
        
        base.Draw(spriteBatch);
    }

    // Get the current value
    public float GetValue()
    {
        return _currentValue;
    }
}
