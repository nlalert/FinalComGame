using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class ParallaxBackground
{
    private List<ParallaxLayer> _layers;
    private Viewport _viewport;

    public ParallaxBackground(Viewport viewport)
    {
        _layers = new List<ParallaxLayer>();
        _viewport = viewport;
    }

    public void AddLayer(ParallaxLayer layer)
    {
        _layers.Add(layer);
    }

    public void AddLayer(Texture2D texture, float scrollSpeed, bool isLooping = true, float scale = 1.0f)
    {
        _layers.Add(new ParallaxLayer(texture, scrollSpeed, isLooping, scale));
    }

    public void Update(GameTime gameTime)
    {
        // Get camera movement since last frame
        Vector2 cameraMovement = Singleton.Instance.Camera.GetMovement();
        
        // Update each layer
        foreach (var layer in _layers)
        {
            layer.Update(cameraMovement);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var layer in _layers)
        {
            // Calculate how many times to draw the texture to fill the screen
            int horizontalCount = (int)Math.Ceiling(_viewport.Width / (layer.SourceRectangle.Width * layer.Scale)) + 2;
            int verticalCount = (int)Math.Ceiling(_viewport.Height / (layer.SourceRectangle.Height * layer.Scale)) + 2;

            // Only draw multiple copies if this layer is set to loop
            if (!layer.IsLooping)
            {
                horizontalCount = 1;
                verticalCount = 1;
            }

            // Draw the layer multiple times to create a seamless effect
            for (int y = 0; y < verticalCount; y++)
            {
                for (int x = 0; x < horizontalCount; x++)
                {
                    // Calculate position for this tile
                    Vector2 drawPosition = new Vector2(
                        layer.Position.X + x * layer.SourceRectangle.Width * layer.Scale,
                        layer.Position.Y + y * layer.SourceRectangle.Height * layer.Scale
                    );

                    // If we're using non-looping layers, just draw at the specified position
                    if (!layer.IsLooping)
                    {
                        drawPosition = layer.Position;
                    }

                    // Draw the texture
                    spriteBatch.Draw(
                        layer.Texture,
                        drawPosition,
                        layer.SourceRectangle,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        layer.Scale,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }
    }
}
