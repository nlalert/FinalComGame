using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FinalComGame;

public class PauseMenu : Scene
{
    private Texture2D _UITexture;
    private TextUI _pauseTitle; // can change to ImageUI later
    private Button _resumeButton;
    private Button _restartButton;
    private Button _settingsButton;
    private Button _mainmenuButton;
    private Rectangle _buttonRectangle;

    private Rectangle _fullScreenRect;

    public override void LoadContent(SpriteBatch spriteBatch)
    {
        base.LoadContent(spriteBatch);
        _UITexture = _content.Load<Texture2D>("UI");
        _buttonRectangle = ViewportManager.Get("Button");

        _fullScreenRect = new Rectangle(0, 0, Singleton.SCREEN_WIDTH, Singleton.SCREEN_HEIGHT);

        SetupHUD();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        PauseSong();
        _gameManager.IsMouseVisible = true;
        if(Singleton.Instance.IsKeyJustPressed(Keys.Escape))
        {
            Singleton.Instance.CurrentGameState = Singleton.GameState.Playing;
        }
    }
    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();
        _spriteBatch.Draw(Singleton.Instance.PixelTexture, _fullScreenRect, Color.Black * 0.8f); // draw murky background
        _spriteBatch.End();
    }

    protected override void SetupHUD()
    {
        int TextWidth = Singleton.SCREEN_WIDTH / 2;
        int TextHeight = 80;
        // Static text
        _pauseTitle = new TextUI(
            new Rectangle((Singleton.SCREEN_WIDTH - TextWidth) / 2 , (Singleton.SCREEN_HEIGHT - TextHeight) / 6, TextWidth, TextHeight),
            "Pause",  
            4,
            Color.White, 
            TextUI.TextAlignment.Center
        );

        int ButtonWidth = Singleton.SCREEN_WIDTH / 3;
        int ButtonHeight = 80;

        _resumeButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 2 / 6, ButtonWidth, ButtonHeight),
            _UITexture,
            "Resume",
            Color.Wheat,
            _buttonRectangle,
            2
        );
        _resumeButton.OnClick += ResumeButton_OnClick;

        _restartButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 3 / 6, ButtonWidth, ButtonHeight),
            _UITexture,
            "Restart",
            Color.Wheat,
            _buttonRectangle,
            2
        );
        _restartButton.OnClick += RestartButton_OnClick;

        _settingsButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 4 / 6, ButtonWidth, ButtonHeight),
            _UITexture,
            "Settings",
            Color.Wheat,
            _buttonRectangle,
            2
        );
        _settingsButton.OnClick += SettingButton_OnClick;

        _mainmenuButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 5 / 6, ButtonWidth, ButtonHeight),
            _UITexture,
            "Back to Main Menu",
            Color.Wheat,
            _buttonRectangle,
            2
        );
        _mainmenuButton.OnClick += MainMenuButton_OnClick;

        _ui.AddHUDElement(_pauseTitle);
        _ui.AddHUDElement(_resumeButton);
        _ui.AddHUDElement(_restartButton);
        _ui.AddHUDElement(_settingsButton);
        _ui.AddHUDElement(_mainmenuButton);
    }
}
