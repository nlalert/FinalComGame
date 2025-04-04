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
        private Texture2D _button;
        private ImageUI _title;
        private Button _startButton;
        private Button _exitButton;
        private Button _playTutorialButton;
        private Button _skipTutorialButton;

        private Texture2D _TitleTexture;
        private Texture2D _bgTexture;
        private Texture2D _mgTexture;
        private Texture2D _fgTexture;

        private Vector2 _bgPosition;
        private Vector2 _mgPosition;
        private Vector2 _fgPosition;

        private float _mgSpeed = 0.5f;
        private float _fgSpeed = 1.0f;

        private float _scaleX;
        private float _scaleY;
        private float scaleOtOmg;
        private float scaleOtOfg;

        public override void LoadContent(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _TitleTexture = _content.Load<Texture2D>("Title");
            _button = _content.Load<Texture2D>("ItemSlot"); //Change Later
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

            _gameManager.IsMouseVisible = true;
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
            spriteBatch.Draw(texture, new Vector2(position.X + texture.Width*-1, position.Y),null, Color.White,0f,Vector2.Zero,Scale,SpriteEffects.None,0f);
            spriteBatch.Draw(texture, new Vector2(position.X + texture.Width, position.Y),null, Color.White,0f,Vector2.Zero,Scale,SpriteEffects.None,0f);
            spriteBatch.Draw(texture, new Vector2(position.X + texture.Width*2, position.Y),null, Color.White,0f,Vector2.Zero,Scale,SpriteEffects.None,0f);
            spriteBatch.Draw(texture, new Vector2(position.X + texture.Width*3, position.Y),null, Color.White,0f,Vector2.Zero,Scale,SpriteEffects.None,0f);
        }

        protected override void SetupHUD()
        {
            Texture2D Title = _TitleTexture; 
            int titleWidth = Singleton.SCREEN_WIDTH / 2;
            int titleHeight = 100;
            _title = new ImageUI(
                Title,
                new Rectangle((Singleton.SCREEN_WIDTH - titleWidth) / 2, (Singleton.SCREEN_HEIGHT - titleHeight) / 4, titleWidth, titleHeight),
                new Rectangle(0, 0, 236, 40)
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

            _playTutorialButton = new Button(
                new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth - 100) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight - 50) / 3, ButtonWidth + 100, ButtonHeight + 50),
                _button,
                _button,
                "Play Tutorial",
                Color.Wheat
            );
            _playTutorialButton.OnClick += PlayTutorialButton_OnClick;

            _skipTutorialButton = new Button(
                new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth + 300) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight + 50) * 2 / 3, ButtonWidth - 300, ButtonHeight - 50),
                _button,
                _button,
                "Skip Tutorial",
                Color.Wheat
            );
            _skipTutorialButton.OnClick += SkipTutorialButton_OnClick;

            _ui.AddHUDElement(_title);
            _ui.AddHUDElement(_startButton);
            _ui.AddHUDElement(_exitButton);
        }

        protected void StartGameButton_OnClick(object sender, EventArgs e)
        {
            ShowStartingChoices();
        }

        private void ShowStartingChoices()
        {
            _ui.RemoveHUDElement(_title);
            _ui.RemoveHUDElement(_startButton);
            _ui.RemoveHUDElement(_exitButton);

            _ui.AddHUDElement(_playTutorialButton);
            _ui.AddHUDElement(_skipTutorialButton);
        }

        private void ShowMainMenu()
        {
            _ui.AddHUDElement(_title);
            _ui.AddHUDElement(_startButton);
            _ui.AddHUDElement(_exitButton);

            _ui.RemoveHUDElement(_playTutorialButton);
            _ui.RemoveHUDElement(_skipTutorialButton);
        }

        protected void PlayTutorialButton_OnClick(object sender, EventArgs e)
        {
            Singleton.Instance.CurrentGameState = Singleton.GameState.StartingGame;
            Singleton.Instance.Stage = 0;
            ShowMainMenu();
        }
        
        protected void SkipTutorialButton_OnClick(object sender, EventArgs e)
        {
            Singleton.Instance.CurrentGameState = Singleton.GameState.StartingGame; 
            Singleton.Instance.Stage = 1;
            ShowMainMenu();
        }

    }

}
