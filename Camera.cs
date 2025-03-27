using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class Camera
{
    private Vector2 _currentPosition;
    private Vector2 _previousPosition;
    private float _zoom;
    private Viewport _viewport;
    
    // Map boundaries
    private Rectangle _mapBounds;
    
    // Smoothing parameter
    private float _smoothSpeed = 0.1f;

    // Screen Shake Variables
    private float _shakeIntensity;
    private float _shakeDuration;

    public Camera(Viewport viewport, Rectangle mapBounds)
    {
        _viewport = viewport;
        _mapBounds = mapBounds;
        _zoom = 2.0f;
        _currentPosition = Singleton.Instance.Player.Position;
    }

    public Vector2 GetMovement()
    {
        return _currentPosition - _previousPosition;
    }

    public Matrix GetTransformation()
    {
        // Apply screen shake offset
        Vector2 shakeOffset = GetScreenShakeOffset();
        
        return Matrix.CreateTranslation(new Vector3(-_currentPosition + shakeOffset, 0)) *
               Matrix.CreateScale(_zoom);
    }

    public void Follow(GameObject target)
    {
        _previousPosition = _currentPosition;

        // Calculate the desired camera center position
        Vector2 targetPosition = new Vector2(
            target.Position.X + target.Viewport.X / 2,
            target.Position.Y + target.Viewport.Y / 2
        );

        // Calculate the camera view size based on zoom
        float viewWidth = _viewport.Width / _zoom;
        float viewHeight = _viewport.Height / _zoom;

        // Smoothly interpolate towards the target
        Vector2 smoothedPosition = Vector2.Lerp(_currentPosition, 
            targetPosition - new Vector2(viewWidth / 2, viewHeight / 2), 
            _smoothSpeed);

        // Clamp the camera position to map boundaries
        float clampedX = MathHelper.Clamp(smoothedPosition.X, 
            _mapBounds.Left, 
            _mapBounds.Right - viewWidth);
        
        float clampedY = MathHelper.Clamp(smoothedPosition.Y, 
            _mapBounds.Top, 
            _mapBounds.Bottom - viewHeight);

        _currentPosition = new Vector2(clampedX, clampedY);

        // Update screen shake
        UpdateScreenShake();
    }

    public void ShakeScreen(float intensity, float duration)
    {
        _shakeIntensity = intensity;
        _shakeDuration = duration;
    }

    private void UpdateScreenShake()
    {
        if (_shakeDuration > 0)
        {
            _shakeDuration -= 0.016f; // Assuming 60 FPS, adjust as needed
        }
        else
        {
            _shakeIntensity = 0;
        }
    }

    private Vector2 GetScreenShakeOffset()
    {
        if (_shakeDuration > 0)
        {
            // Generate random shake offset
            float offsetX = (float)(Singleton.Instance.Random.NextDouble() * 2 - 1) * _shakeIntensity * 10;
            float offsetY = (float)(Singleton.Instance.Random.NextDouble() * 2 - 1) * _shakeIntensity * 10;
            
            return new Vector2(offsetX, offsetY);
        }
        
        return Vector2.Zero;
    }
}