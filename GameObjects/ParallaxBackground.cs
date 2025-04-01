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

    public void AddLayer(Texture2D texture, float scrollSpeed, float scale = 1.0f, Vector2 offset = new Vector2())
    {
        _layers.Add(new ParallaxLayer(texture, scrollSpeed, scale, offset));
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
            // Calculate the scale to fit vertically while maintaining aspect ratio
            float verticalScale = (float)_viewport.Height / layer.SourceRectangle.Height;
            float actualScale = layer.Scale * verticalScale;
            
            // Calculate the width after scaling
            float scaledWidth = layer.SourceRectangle.Width * actualScale;
            
            // Calculate how many copies needed to fill the screen horizontally
            int horizontalCount = (int)Math.Ceiling(_viewport.Width / scaledWidth) + 2;

            // Draw the layer multiple times horizontally to create a seamless effect
            for (int x = 0; x < horizontalCount; x++)
            {
                // Calculate position for this tile
                Vector2 drawPosition = new Vector2(
                    layer.Position.X + x * scaledWidth,
                    layer.Position.Y
                );

                // Draw the texture
                spriteBatch.Draw(
                    layer.Texture,
                    drawPosition,
                    layer.SourceRectangle,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    actualScale,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}
