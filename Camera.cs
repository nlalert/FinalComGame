using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class Camera
{
    private Vector2 _position;
    private float _zoom;
    private Viewport _viewport;
    
    // Map boundaries
    private Rectangle _mapBounds;
    
    // Smoothing parameter
    private float _smoothSpeed = 0.1f;

    public Camera(Viewport viewport, Rectangle mapBounds)
    {
        _viewport = viewport;
        _mapBounds = mapBounds;
        _zoom = 2.0f;
        _position = Singleton.Instance.Player.Position;
    }

    public Matrix GetTransformation()
    {
        return Matrix.CreateTranslation(new Vector3(-_position, 0)) *
               Matrix.CreateScale(_zoom);
    }

    public void Follow(GameObject target)
    {
        // Calculate the desired camera center position
        Vector2 targetPosition = new Vector2(
            target.Position.X + target.Viewport.X / 2,
            target.Position.Y + target.Viewport.Y / 2
        );

        // Calculate the camera view size based on zoom
        float viewWidth = _viewport.Width / _zoom;
        float viewHeight = _viewport.Height / _zoom;

        // Smoothly interpolate towards the target
        Vector2 smoothedPosition = Vector2.Lerp(_position, 
            targetPosition - new Vector2(viewWidth / 2, viewHeight / 2), 
            _smoothSpeed);

        // Clamp the camera position to map boundaries
        float clampedX = MathHelper.Clamp(smoothedPosition.X, 
            _mapBounds.Left, 
            _mapBounds.Right - viewWidth);
        
        float clampedY = MathHelper.Clamp(smoothedPosition.Y, 
            _mapBounds.Top, 
            _mapBounds.Bottom - viewHeight);

        _position = new Vector2(clampedX, clampedY);
    }
}