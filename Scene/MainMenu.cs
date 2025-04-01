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
        private UiButton _StartButton;
        private UiButton _ExitButton;

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

            // Wrap background when it moves out of view
            if (_bgPosition.X <= -_bgTexture.Width) _bgPosition.X = 0;
            if (_mgPosition.X <= -_mgTexture.Width) _mgPosition.X = 0;
            if (_fgPosition.X <= -_fgTexture.Width) _fgPosition.X = 0;

            _StartButton.Update();
            _ExitButton.Update();
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
            DrawParallaxLayer(_spriteBatch, _mgTexture, _mgPosition);
            DrawParallaxLayer(_spriteBatch, _fgTexture, _fgPosition);

            _spriteBatch.End();

            // Draw UI elements (HUD)
            _spriteBatch.Begin();
            base.Draw(gameTime);  // Call the base draw function to maintain UI behavior
            _spriteBatch.End();
        }

        private void DrawParallaxLayer(SpriteBatch spriteBatch, Texture2D texture, Vector2 position)
        {
            spriteBatch.Draw(texture, position, Color.White);
            spriteBatch.Draw(texture, new Vector2(position.X + texture.Width, position.Y), Color.White);
            spriteBatch.Draw(texture, new Vector2(position.X + texture.Width*2, position.Y), Color.White);
            spriteBatch.Draw(texture, new Vector2(position.X + texture.Width*3, position.Y), Color.White);
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

            int startButtonWidth = Singleton.SCREEN_WIDTH / 2;
            int startButtonHeight = 100;
            _StartButton = new UiButton(
                new Rectangle((Singleton.SCREEN_WIDTH - startButtonWidth) / 2, (Singleton.SCREEN_HEIGHT - startButtonHeight) * 2 / 4, startButtonWidth, startButtonHeight),
                StartButtonTexture,
                StartHoverButtonTexture,
                "",
                Color.Wheat
            );
            _StartButton.OnClick += StartGameButton_OnClick;

            int exitButtonWidth = Singleton.SCREEN_WIDTH / 2;
            int exitButtonHeight = 100;
            _ExitButton = new UiButton(
                new Rectangle((Singleton.SCREEN_WIDTH - exitButtonWidth) / 2, (Singleton.SCREEN_HEIGHT - exitButtonHeight) * 3 / 4, exitButtonWidth, exitButtonHeight),
                ExitButtonTexture,
                ExitHoverButtonTexture,
                "",
                Color.Wheat
            );
            _ExitButton.OnClick += ExitGameButton_OnClick;

            _ui.AddHUDElement(_Title);
            _ui.AddHUDElement(_StartButton);
            _ui.AddHUDElement(_ExitButton);
        }

        private void ExitGameButton_OnClick(object sender, EventArgs e)
        {
            Singleton.Instance.CurrentGameState = Singleton.GameState.Exit;
        }

        private void StartGameButton_OnClick(object sender, EventArgs e)
        {
            Singleton.Instance.CurrentGameState = Singleton.GameState.StartingGame;
        }
    }
}
