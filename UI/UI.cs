using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FinalComGame;

public class UI
{
    private List<HUDElement> hudElements;
    private List<WorldSpaceUIElement> worldSpaceElements;
    
    public UI()
    {
        hudElements = new List<HUDElement>();
        worldSpaceElements = new List<WorldSpaceUIElement>();
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
    }
    
    public void Update(GameTime gameTime)
    {
        // Update HUD elements (screen space)
        foreach (var element in hudElements)
        {
            element.Update(gameTime);
        }
        
        // Update world space elements
        foreach (var element in worldSpaceElements)
        {
            element.Update(gameTime);
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
}

