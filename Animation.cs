using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FinalComGame
{
    public class Animation
    {

        private Texture2D _spriteSheet;
        private List<Rectangle> _frames;
        private float _fps;  // Time per frame
        private float _timer;      // Tracks time elapsed
        private string _nextAnimation;
        private int _currentFrameIndex;
        private int _currentFrameStart;
        private int _currentFrameCount;

        public List<string> _animationName = new List<string>();
        private List<int> _frameIndex = new List<int>();
        private List<int> _frameCount = new List<int>();
        private int _frameIndexWidth;
        private int _frameIndexHeight;

        private Vector2 _frameSize;

        public bool IsLooping { get; set; } = true;
        public bool IsFinished { get; private set; } = false;
        public bool IsTransition { get; set; } = false;

        public Animation(Texture2D spriteSheet, int frameWidth, int frameHeight, Vector2 spriteSize, float fps)
        {
            _spriteSheet = spriteSheet;
            _frames = new List<Rectangle>();
            _fps = fps;
            _timer = 0f;
            _currentFrameIndex = 0;

            _frameIndexWidth = (int)(spriteSize.X / frameWidth);
            _frameIndexHeight = (int)(spriteSize.Y / frameHeight);
            int frameCount = _frameIndexWidth * _frameIndexHeight;
            

            for (int i = 0; i < frameCount; i++)
            {
                _frames.Add(new Rectangle(i % _frameIndexWidth * frameWidth, i / _frameIndexWidth * frameHeight, frameWidth, frameHeight));
            }

            Console.WriteLine(_frames.Count);

            _frameSize = new Vector2(frameWidth, frameHeight);
        }

        public void Update(float deltaTime)
        {
            if (IsFinished) return;

            _timer += deltaTime;

            if (_timer >= 1 / _fps)
            {
                _timer = 0f;
                _currentFrameIndex++;

                if (_currentFrameIndex >= _currentFrameStart + _currentFrameCount)
                {
                    if (IsLooping)
                        _currentFrameIndex = _currentFrameStart;
                    else
                    {
                        if(IsTransition){
                            IsLooping = true;
                            ChangeAnimation(_nextAnimation);
                        }
                        else
                        {  
                            _currentFrameIndex = _currentFrameCount - 1;  // Stay on the last frame
                            IsFinished = true;
                        }
                    }
                }
            }

            //Console.WriteLine(_currentFrameIndex);
        }

        public void Reset()
        {
            _currentFrameIndex = 0;
            _timer = 0f;
            IsFinished = false;
        }

        public Rectangle GetCurrentFrame()
        {
            return _frames[_currentFrameIndex];
        }

        public Texture2D GetTexture()
        {
            return _spriteSheet;
        }

        public Vector2 GetFrameSize()
        {
            return _frameSize;
        }

        public void SetFPS(float fps)
        {
            _fps = fps;
        }

        public void AddAnimation(string name, Vector2 index, int count)
        {
            _animationName.Add(name);
            _frameIndex.Add((int)index.X + (int)(index.Y * _frameIndexWidth));
            _frameCount.Add(count);
        }

        public void ChangeAnimation(string name)
        {
            _currentFrameIndex = _frameIndex[_animationName.IndexOf(name)];
            _currentFrameStart = _currentFrameIndex;
            _currentFrameCount = _frameCount[_animationName.IndexOf(name)];
            IsTransition = false;
            IsLooping = true;
            IsFinished = false;
        }

        public void ChangeTransitionAnimation(string currentName, string nextName)
        {
            _currentFrameIndex = _frameIndex[_animationName.IndexOf(currentName)];
            _currentFrameStart = _currentFrameIndex;
            _currentFrameCount = _frameCount[_animationName.IndexOf(currentName)];
            _nextAnimation = nextName;
            IsTransition = true;
            IsLooping = false;
            IsFinished = false;
        }
    }
}
