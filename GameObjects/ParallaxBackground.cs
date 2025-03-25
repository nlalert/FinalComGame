using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class ParallaxBackground : GameObject
    {
        private Texture2D _fgTexture, _mgTexture, _bgTexture;
        private Vector2 _fgPosition, _mgPosition, _bgPosition, _startPosition;

        public ParallaxBackground(Texture2D fgTexture,Texture2D mgTexture,Texture2D bgTexture, Vector2 Position)
        {
            _fgTexture = fgTexture;
            _mgTexture = mgTexture;
            _bgTexture = bgTexture;
            _startPosition = Position;

            base.Scale = new Vector2(Math.Max(Singleton.SCREEN_WIDTH / _bgTexture.Width, Singleton.SCREEN_HEIGHT / _bgTexture.Height));
            
        }

        public override void Reset(){
            _fgPosition = new Vector2 (_startPosition.X - _fgTexture.Width/2, _startPosition.Y - _fgTexture.Height/2);
            _mgPosition = new Vector2 (_startPosition.X - _mgTexture.Width/2, _startPosition.Y - _mgTexture.Height/2);
            _bgPosition = new Vector2 (_startPosition.X - _bgTexture.Width/2, _startPosition.Y - _bgTexture.Height/2);
        }
        
        public void Update(){
            _fgPosition = new Vector2 (_startPosition.X - _fgTexture.Width * Scale.X / 2, _startPosition.Y - _fgTexture.Height  * Scale.Y / 2) 
            + new Vector2 ((Singleton.Instance.Player.Position.X - _startPosition.X) * 1/4, 
            Singleton.Instance.Player.Position.Y - _startPosition.Y);
             
            _mgPosition = new Vector2 (_startPosition.X - _mgTexture.Width * Scale.X / 2, _startPosition.Y - _mgTexture.Height * Scale.Y / 2) 
            + new Vector2 ((Singleton.Instance.Player.Position.X - _startPosition.X) * 1/2,
            Singleton.Instance.Player.Position.Y - _startPosition.Y);

            _bgPosition = new Vector2 (_startPosition.X - _bgTexture.Width * Scale.X / 2, _startPosition.Y - _bgTexture.Height * Scale.Y / 2)
            + new Vector2 (Singleton.Instance.Player.Position.X - _startPosition.X,
            Singleton.Instance.Player.Position.Y - _startPosition.Y);

            Console.WriteLine(_fgPosition + " " + _mgPosition + " " + _bgPosition);
        }

        public override void Draw(SpriteBatch spriteBatch){

            spriteBatch.Draw(
                _bgTexture,
                _bgPosition,
                new Rectangle(0, 0, _bgTexture.Width, _bgTexture.Height),
                Color.White,
                0f,
                Vector2.Zero,
                base.Scale,
                SpriteEffects.None, 
                0f
            );

            spriteBatch.Draw(
                _mgTexture,
                _mgPosition,
                new Rectangle(0, 0, _mgTexture.Width, _mgTexture.Height),
                Color.White,
                0f,
                Vector2.Zero,
                base.Scale,
                SpriteEffects.None, 
                0f
            );

            spriteBatch.Draw(
                _fgTexture,
                _fgPosition,
                new Rectangle(0, 0, _fgTexture.Width, _fgTexture.Height),
                Color.White,
                0f,
                Vector2.Zero,
                base.Scale,
                SpriteEffects.None, 
                0f
            );
        }
    }
}