using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class ParallaxLayer
{
    public Texture2D Texture;
    public float ScrollSpeed;
    public Vector2 Position;
    public bool IsLooping;
    public Rectangle SourceRectangle;
    public float Scale;

    public ParallaxLayer(Texture2D texture, float scrollSpeed, bool isLooping = true, float scale = 1.0f)
    {
        Texture = texture;
        ScrollSpeed = scrollSpeed;
        Position = Vector2.Zero;
        IsLooping = isLooping;
        SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
        Scale = scale;
    }

    public void Update(Vector2 cameraMovement)
    {
        // Move the layer based on camera movement and layer's scroll speed
        Position -= cameraMovement * ScrollSpeed;
        
        // If looping is enabled, handle wrapping
        if (IsLooping)
        {
            // Wrap position horizontally
            if (Position.X > 0)
                Position.X %= SourceRectangle.Width;
            else if (Position.X < -SourceRectangle.Width)
                Position.X = Position.X % SourceRectangle.Width;
            
            // Wrap position vertically if needed
            if (Position.Y > 0)
                Position.Y %= SourceRectangle.Height;
            else if (Position.Y < -SourceRectangle.Height)
                Position.Y = Position.Y % SourceRectangle.Height;
        }
    }
}
