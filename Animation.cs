using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FinalComGame
{
    public class Animation
    {
        private Texture2D _spriteSheet;
        private List<Rectangle> _frames;
        private float _fps;  // Time per frame
        private float _timer;      // Tracks time elapsed
        private int _currentFrame;

        public bool IsLooping { get; set; } = true;
        public bool IsFinished { get; private set; } = false;

        public Animation(Texture2D spriteSheet, int frameWidth, int frameHeight, int frameCount, float fps)
        {
            _spriteSheet = spriteSheet;
            _frames = new List<Rectangle>();
            _fps = fps;
            _timer = 0f;
            _currentFrame = 0;

            for (int i = 0; i < frameCount; i++)
            {
                _frames.Add(new Rectangle(i * frameWidth, 0, frameWidth, frameHeight));
            }
        }

        public void Update(GameTime gameTime)
        {
            if (IsFinished) return;

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer >= 1 / _fps)
            {
                _timer = 0f;
                _currentFrame++;

                if (_currentFrame >= _frames.Count)
                {
                    if (IsLooping)
                        _currentFrame = 0;
                    else
                    {
                        _currentFrame = _frames.Count - 1;  // Stay on the last frame
                        IsFinished = true;
                    }
                }
            }
        }

        public void Reset()
        {
            _currentFrame = 0;
            _timer = 0f;
            IsFinished = false;
        }

        public Rectangle GetCurrentFrame()
        {
            return _frames[_currentFrame];
        }

        public Texture2D GetTexture()
        {
            return _spriteSheet;
        }
    }
}
