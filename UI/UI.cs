using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FinalComGame
{
    public class UI
    {
        private List<UIElement> elements;
        
        public UI()
        {
            elements = new List<UIElement>();
        }
        
        public void AddElement(UIElement element)
        {
            elements.Add(element);
        }
        
        public void Update(GameTime gameTime)
        {
            foreach (var element in elements)
            {
                element.Update(gameTime);
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var element in elements)
            {
                element.Draw(spriteBatch);
            }
        }
    }
}