using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class ParallaxLayer
{
    public Texture2D Texture;
    public float ScrollSpeed;
    public Vector2 Position;
    public Rectangle SourceRectangle;
    public float Scale;

    public ParallaxLayer(Texture2D texture, float scrollSpeed, float scale = 1.0f, Vector2 offset = new Vector2())
    {
        Texture = texture;
        ScrollSpeed = scrollSpeed;
        Position = Vector2.Zero + offset;
        SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
        Scale = scale;
    }

    public void Update(Vector2 cameraMovement)
    {
        // Move the layer based on camera movement and layer's scroll speed
        Position -= cameraMovement * ScrollSpeed;
    }
}
