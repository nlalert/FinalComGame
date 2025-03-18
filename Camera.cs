using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class Camera
{
    private Vector2 _position;
    private float _zoom;
    private Viewport _viewport;

    public Camera(Viewport viewport)
    {
        _viewport = viewport;
        _zoom = 2f;
    }

    public Matrix GetTransformation()
    {
        return Matrix.CreateTranslation(new Vector3(-_position, 0)) *
               Matrix.CreateScale(_zoom);
    }

    public void Follow(GameObject target)
    {
        //_position = new Vector2(target.Position.X + target.Viewport.X / (2 * _zoom) - _viewport.Width / (2 * _zoom), 0);
        _position = new Vector2(target.Position.X + target.Viewport.X / (2 * _zoom) - _viewport.Width / (2 * _zoom), 
            target.Position.Y + target.Viewport.Y / (2 * _zoom) - _viewport.Width / (2 * _zoom));
    }
}
