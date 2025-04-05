using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FinalComGame;

// Class to handle prompt elements with timers, extending UIElement base class
public class Prompt : UIElement
{
    public string Message;
    public Color TextColor;
    public bool IsExpired;
    
    public int Width { get { return _bounds.Width; } }
    public int Height { get { return _bounds.Height; } }
    
    // Set position via the bounds
    public Vector2 Position
    {
        get { return new Vector2(_bounds.X, _bounds.Y); }
        set { _bounds.X = (int)value.X; _bounds.Y = (int)value.Y; }
    }
    
    private float _timer;
    private float _alpha;
    
    // Background color and opacity
    private Color _backgroundColor = new Color(0, 0, 0, 180); // Semi-transparent black
    
    public Prompt(Rectangle bounds, string message, Color textColor, float duration)
        : base(bounds)
    {
        Message = message;
        TextColor = textColor;
        _timer = duration;
        _alpha = 1.0f;
        IsExpired = false;
        
        // Adjust bounds based on text
        if (Singleton.Instance.GameFont != null)
        {
            Vector2 textSize = Singleton.Instance.GameFont.MeasureString(Message);
            _bounds.Width = Math.Max(_bounds.Width, (int)textSize.X + 40); // Padding
            _bounds.Height = Math.Max(_bounds.Height, (int)textSize.Y + 20); // Padding
        }
    }
    
    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Decrease timer
        _timer -= deltaTime;
        
        // Fade out when timer is below 1 second
        if (_timer < 1.0f)
        {
            _alpha = Math.Max(0, _timer / 1.0f);
        }
        
        // Mark as expired when timer reaches zero
        if (_timer <= 0)
        {
            IsExpired = true;
        }
        
        base.Update(gameTime);
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        if (IsExpired)
            return;
        
        // Draw background with current alpha
        Color bgColor = new Color(
            _backgroundColor.R, 
            _backgroundColor.G, 
            _backgroundColor.B, 
            (byte)(_backgroundColor.A * _alpha)
        );
        
        spriteBatch.Draw(Singleton.Instance.PixelTexture, _bounds, bgColor);
        
        // Draw text with current alpha
        Color textColorWithAlpha = new Color(
            TextColor.R,
            TextColor.G,
            TextColor.B,
            (byte)(TextColor.A * _alpha)
        );
        
        // Center text in the background
        Vector2 textSize = Singleton.Instance.GameFont.MeasureString(Message);
        Vector2 textPosition = new Vector2(
            _bounds.X + (_bounds.Width - textSize.X) / 2,
            _bounds.Y + (_bounds.Height - textSize.Y) / 2
        );
        
        spriteBatch.DrawString(
            Singleton.Instance.GameFont,
            Message,
            textPosition,
            textColorWithAlpha
        );
        
        base.Draw(spriteBatch);
    }
}
