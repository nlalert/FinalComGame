using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FinalComGame;

public class PauseMenu : Scene
{
    private TextUI _pauseTitle; // can change to ImageUI later
    private Button _resumeButton;
    private Button _restartButton;
    private Button _settingsButton;
    private Button _mainmenuButton;
    private Button _backButton;

    private int buttonGap;

    public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager content)
    {
        base.Initialize(graphicsDevice, graphicsDeviceManager, content);

        // a gap between each button
        buttonGap = 5;
    }

    public override void LoadContent(SpriteBatch spriteBatch)
    {
        base.LoadContent(spriteBatch);

        SetupHUD();
    }

    protected override void SetupHUD()
    {
        int TextWidth = Singleton.SCREEN_WIDTH / 2;
        int TextHeight = 80;
        // Static text
        _pauseTitle = new TextUI(
            new Rectangle((Singleton.SCREEN_WIDTH - TextWidth) / 2 , (Singleton.SCREEN_HEIGHT - TextHeight) / 7, TextWidth, TextHeight),
            "Pause",  
            Color.White, 
            TextUI.TextAlignment.Center
        );

        Texture2D Button = _content.Load<Texture2D>("ItemSlot"); //Change Later
        int ButtonWidth = Singleton.SCREEN_WIDTH / 2;
        int ButtonHeight = 80;

        _resumeButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 2 / 7, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "Resume Button",
            Color.Wheat
        );
        _resumeButton.OnClick += ResumeButton_OnClick;

        _restartButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 3 / 7, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "_restartButton Button",
            Color.Wheat
        );
        _restartButton.OnClick += RestartButton_OnClick;

        _settingsButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 4 / 7, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "_settingsButton Button",
            Color.Wheat
        );
        _settingsButton.OnClick += SettingButton_OnClick;

        _mainmenuButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 5 / 7, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "_mainmenuButton Button",
            Color.Wheat
        );
        _mainmenuButton.OnClick += MainMenuButton_OnClick;

        _backButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 6 / 7, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "_backButton Button",
            Color.Wheat
        );
        _backButton.OnClick += BackButton_OnClick;

        _ui.AddHUDElement(_pauseTitle);
        _ui.AddHUDElement(_resumeButton);
        _ui.AddHUDElement(_restartButton);
        _ui.AddHUDElement(_settingsButton);
        _ui.AddHUDElement(_mainmenuButton);
        _ui.AddHUDElement(_backButton);
    }

    private void BackButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Playing;
    }

    private void MainMenuButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.MainMenu;
    }

    private void SettingButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Settings;
    }

    private void RestartButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.StartingGame;
    }

    private void ResumeButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Playing;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        PauseSong();
    }
}
