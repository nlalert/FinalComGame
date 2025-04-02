using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace FinalComGame
{
    public class MainMenu : Scene
    {
        private ImageUI _Title;
        private Button _startButton;
        private Button _exitButton;

        private Texture2D _bgTexture;
        private Texture2D _mgTexture;
        private Texture2D _fgTexture;

        private Vector2 _bgPosition;
        private Vector2 _mgPosition;
        private Vector2 _fgPosition;

        private float _bgSpeed = 0.2f;
        private float _mgSpeed = 0.5f;
        private float _fgSpeed = 1.0f;

        private float _scaleX;
        private float _scaleY;
        private float scaleOtOmg;
        private float scaleOtOfg;
        public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager content)
        {
            base.Initialize(graphicsDevice, graphicsDeviceManager, content);
        }

        public override void LoadContent(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;

            // Load parallax layers
            _bgTexture = _content.Load<Texture2D>("Level_1_Parallax_bg");
            _mgTexture = _content.Load<Texture2D>("Level_1_Parallax_mg");
            _fgTexture = _content.Load<Texture2D>("Level_1_Parallax_fg");

            _bgPosition = Vector2.Zero;
            _mgPosition = Vector2.Zero;
            _fgPosition = Vector2.Zero;

            SetupHUD();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move layers at different speeds
            // _bgPosition.X -= _bgSpeed * deltaTime * 100;
            _mgPosition.X -= _mgSpeed * deltaTime * 100;
            _fgPosition.X -= _fgSpeed * deltaTime * 100;
            _scaleX = (float)Singleton.SCREEN_WIDTH / _bgTexture.Width;
            _scaleY = (float)Singleton.SCREEN_HEIGHT / _bgTexture.Height;
            scaleOtOmg = (float)Singleton.SCREEN_HEIGHT / _mgTexture.Height;
            scaleOtOfg = (float)Singleton.SCREEN_HEIGHT / _fgTexture.Height;

            // Wrap background when it moves out of view
            if (_bgPosition.X <= -_bgTexture.Width) _bgPosition.X = 0;
            if (_mgPosition.X <= -_mgTexture.Width) _mgPosition.X = 0;
            if (_fgPosition.X <= -_fgTexture.Width) _fgPosition.X = 0;
        }

        public override void Draw(GameTime gameTime)
        {
            if (_spriteBatch == null)
            {
                Console.WriteLine("SpriteBatch is not initialized in MainMenu.");
                return;
            }

            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            // Draw parallax layers
            // DrawParallaxLayer(_spriteBatch, _bgTexture, _bgPosition);
            _spriteBatch.Draw(_bgTexture, _bgPosition, null, Color.White, 0f, Vector2.Zero, new Vector2(_scaleX, _scaleY), SpriteEffects.None, 0f);
            DrawParallaxLayer(_spriteBatch, _mgTexture, _mgPosition,new Vector2(scaleOtOmg,scaleOtOmg));
            // DrawParallaxLayer(_spriteBatch, _mgTexture, _mgPosition,new Vector2(1,1));
            DrawParallaxLayer(_spriteBatch, _fgTexture, _fgPosition,new Vector2(scaleOtOfg,scaleOtOfg));

            _spriteBatch.End();

            // Draw UI elements (HUD)
            _spriteBatch.Begin();
            base.Draw(gameTime);  // Call the base draw function to maintain UI behavior
            _spriteBatch.End();
        }

        private void DrawParallaxLayer(SpriteBatch spriteBatch, Texture2D texture, Vector2 position,Vector2 Scale)
        {
            spriteBatch.Draw(texture, position,null, Color.White,0f,Vector2.Zero,Scale,SpriteEffects.None,0f);
            spriteBatch.Draw(texture, new Vector2(position.X + texture.Width, position.Y),null, Color.White,0f,Vector2.Zero,Scale,SpriteEffects.None,0f);
            spriteBatch.Draw(texture, new Vector2(position.X + texture.Width*2, position.Y),null, Color.White,0f,Vector2.Zero,Scale,SpriteEffects.None,0f);
            spriteBatch.Draw(texture, new Vector2(position.X + texture.Width*3, position.Y),null, Color.White,0f,Vector2.Zero,Scale,SpriteEffects.None,0f);
        }

        protected override void SetupHUD()
        {
            Texture2D Title = _content.Load<Texture2D>("Gun"); // Change Later TEMP
            int titleWidth = Singleton.SCREEN_WIDTH / 2;
            int titleHeight = 100;
            _Title = new ImageUI(
                Title,
                new Rectangle((Singleton.SCREEN_WIDTH - titleWidth) / 2, (Singleton.SCREEN_HEIGHT - titleHeight) / 4, titleWidth, titleHeight),
                new Rectangle(0, 0, 32, 32)
            );

            Texture2D StartButtonTexture = _content.Load<Texture2D>("StaticStartButton");
            Texture2D StartHoverButtonTexture = _content.Load<Texture2D>("HoverStartButton");
            Texture2D ExitButtonTexture = _content.Load<Texture2D>("StaticExitButton");
            Texture2D ExitHoverButtonTexture = _content.Load<Texture2D>("HoverExitButton");

            int ButtonWidth = Singleton.SCREEN_WIDTH / 2;
            int ButtonHeight = 100;
            _startButton = new Button(
                new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 2 / 4, ButtonWidth, ButtonHeight),
                StartButtonTexture,
                StartHoverButtonTexture,
                "",
                Color.Wheat
            );

            _startButton.OnClick += StartGameButton_OnClick;

            _exitButton = new Button(
                new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 3 / 4, ButtonWidth, ButtonHeight),
                ExitButtonTexture,
                ExitHoverButtonTexture,
                "",
                Color.Wheat
            );
            _exitButton.OnClick += ExitGameButton_OnClick;

            _ui.AddHUDElement(_Title);
            _ui.AddHUDElement(_startButton);
            _ui.AddHUDElement(_exitButton);
        }
    }
}
