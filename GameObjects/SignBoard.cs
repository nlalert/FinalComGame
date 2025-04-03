using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class SignBoard : GameObject
    {
        private string _text;
        private Color _backgroundColor;
        private Color _borderColor;
        private int _borderThickness;
        private float _textScale;

        public SignBoard(Texture2D backgroundTexture, string text, Vector2 position, int width, int height,
                Color backgroundColor, Color borderColor) : base(backgroundTexture)
        {
            // Basic setup
            _texture = backgroundTexture;
            Position = position;
            Viewport = new Rectangle(0, 0, width, height);
            _text = text;
            
            _backgroundColor = backgroundColor;  // Semi-transparent black
            _borderColor = borderColor;

            // Default appearance settings
            _borderThickness = 4;
            _textScale = 1.0f;
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw border
            if (_borderThickness > 0)
            {
                Rectangle borderRect = new Rectangle(
                    (int)Position.X - _borderThickness,
                    (int)Position.Y - _borderThickness,
                    Viewport.Width + (_borderThickness * 2),
                    Viewport.Height + (_borderThickness * 2)
                );
                spriteBatch.Draw(_texture, borderRect, _borderColor);
            }
            
            // Draw background
            Rectangle bgRect = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                Viewport.Width,
                Viewport.Height
            );
            spriteBatch.Draw(_texture, bgRect, _backgroundColor);
            
            // Word wrap the text to fit the sign
            List<string> lines = WrapText(_text, Viewport.Width - 20);
            float lineHeight = Singleton.Instance.GameFont.MeasureString("Tg").Y * _textScale;
            
            for (int i = 0; i < lines.Count; i++)
            {
                Vector2 textPos = new Vector2(
                    Position.X + 10,
                    Position.Y + 10 + (i * lineHeight)
                );
                
                spriteBatch.DrawString(
                    Singleton.Instance.GameFont,
                    lines[i],
                    textPos,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _textScale,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        
        // Helper method to wrap text to fit within the sign width
        private List<string> WrapText(string text, float maxLineWidth)
        {
            List<string> lines = new List<string>();
            string[] words = text.Split(' ');
            
            string currentLine = "";
            float currentWidth = 0;
            
            foreach (string word in words)
            {
                Vector2 wordSize = Singleton.Instance.GameFont.MeasureString(word + " ") * _textScale;
                
                if (currentWidth + wordSize.X > maxLineWidth)
                {
                    // Line would be too long with this word, start a new line
                    lines.Add(currentLine.Trim());
                    currentLine = word + " ";
                    currentWidth = wordSize.X;
                }
                else
                {
                    // Add word to current line
                    currentLine += word + " ";
                    currentWidth += wordSize.X;
                }
            }
            
            // Add the last line if not empty
            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine.Trim());
            }
            
            return lines;
        }
    }
}