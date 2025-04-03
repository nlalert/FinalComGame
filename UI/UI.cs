using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace FinalComGame;
public class UI
{
    private List<HUDElement> hudElements;
    private List<WorldSpaceUIElement> worldSpaceElements;

    private List<Prompt> activePrompts;

    public UI()
    {
        hudElements = new List<HUDElement>();
        worldSpaceElements = new List<WorldSpaceUIElement>();
        activePrompts = new List<Prompt>();
    }
    
    public void AddHUDElement(HUDElement element)
    {
        hudElements.Add(element);
    }
    
    public void AddWorldSpaceElement(WorldSpaceUIElement element)
    {
        worldSpaceElements.Add(element);
    }
    
    public void RemoveHUDElement(HUDElement element)
    {
        hudElements.Remove(element);
    }
    
    public void RemoveWorldSpaceElement(WorldSpaceUIElement element)
    {
        worldSpaceElements.Remove(element);
    }
    
    public void ClearHUD()
    {
        hudElements.Clear();
    }
    
    public void ClearWorldSpaceUI()
    {
        worldSpaceElements.Clear();
    }
    
    public void ClearAllUI()
    {
        hudElements.Clear();
        worldSpaceElements.Clear();
        activePrompts.Clear();
    }
    
    public void Update(GameTime gameTime)
    {
        // Update HUD elements (screen space)
        for (int i = 0; i < hudElements.Count; i++)
        {
            hudElements[i].Update(gameTime);
        }

        // Update world space elements
        for (int i = 0; i < worldSpaceElements.Count; i++)
        {
            worldSpaceElements[i].Update(gameTime);
        }
        
        // Update prompts and remove expired ones
        for (int i = activePrompts.Count - 1; i >= 0; i--)
        {
            activePrompts[i].Update(gameTime);
            
            if (activePrompts[i].IsExpired)
            {
                activePrompts.RemoveAt(i);
            }
        }
    }
    
    public void DrawHUD(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        foreach (var element in hudElements)
        {
            element.Draw(spriteBatch);
        }
        
        spriteBatch.End();
    }
    
    public void DrawWorldSpaceUI(SpriteBatch spriteBatch)
    {
        if(Singleton.Instance.Camera == null)
            return;
            
        // Draw world-space UI elements with camera transform applied
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Singleton.Instance.Camera.GetTransformation());
        
        foreach (var element in worldSpaceElements)
        {
            element.Draw(spriteBatch);
        }
        
        spriteBatch.End();
    }

    public void DrawPrompts(SpriteBatch spriteBatch)
    {
        if (activePrompts.Count == 0)
            return;
            
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        // Stack prompts from top to bottom
        int yOffset = 100; // Start from top with some margin
        int safeAreaHeight = Singleton.SCREEN_HEIGHT - 40; // Safe area height with margin
        int promptsToShow = activePrompts.Count;
        
        // Check if prompts will fit within the screen
        int estimatedHeight = 0;
        for (int i = 0; i < activePrompts.Count; i++)
        {
            estimatedHeight += activePrompts[i].Height + 10; // Height + spacing
            
            // If we exceed safe area or max visible prompts, limit how many we show
            if (estimatedHeight > safeAreaHeight || i + 1 > Singleton.MAX_VISIBLE_PROMPTS)
            {
                promptsToShow = i;
                break;
            }
        }
        
        // Draw only the prompts that fit (most recent ones)
        int startIndex = Math.Max(0, activePrompts.Count - promptsToShow);
        for (int i = startIndex; i < activePrompts.Count; i++)
        {
            // Center horizontally and stack vertically
            activePrompts[i].Position = new Vector2(
                (Singleton.SCREEN_WIDTH - activePrompts[i].Width) / 2, 
                yOffset
            );
            
            activePrompts[i].Draw(spriteBatch);
            
            yOffset += activePrompts[i].Height + 10; // Add spacing between prompts
        }
        
        spriteBatch.End();
    }

    public void Prompt(string message, float duration = 2.0f, Color? textColor = null)
    {
        // Default to white if no color provided
        Color color = textColor ?? Color.White;
        
        // Default prompt dimensions
        int promptWidth = Singleton.SCREEN_WIDTH / 4;
        int promptHeight = Singleton.SCREEN_HEIGHT / 15;
        
        // Adjust based on text size if font is available
        if (Singleton.Instance.GameFont != null)
        {
            Vector2 textSize = Singleton.Instance.GameFont.MeasureString(message);
            promptWidth = Math.Max(promptWidth, (int)textSize.X + 40); // Add padding
            promptHeight = Math.Max(promptHeight, (int)textSize.Y + 20); // Add padding
        }
        
        // Initial position (will be adjusted in DrawPrompts)
        Rectangle bounds = new Rectangle(
            (Singleton.SCREEN_WIDTH - promptWidth) / 2,
            20,
            promptWidth,
            promptHeight
        );
        
        // Create a new prompt with timer
        Prompt newPrompt = new Prompt(bounds, message, color, duration);
        
        // If we already have too many prompts, remove the oldest one
        if (activePrompts.Count >= Singleton.MAX_VISIBLE_PROMPTS)
        {
            activePrompts.RemoveAt(0); // Remove oldest prompt
        }
        
        // Add to active prompts
        activePrompts.Add(newPrompt);
    }
}
