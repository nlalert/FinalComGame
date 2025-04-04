using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FinalComGame;

public class PauseMenu : Scene
{
    private Texture2D _button;
    private TextUI _pauseTitle; // can change to ImageUI later
    private Button _resumeButton;
    private Button _restartButton;
    private Button _settingsButton;
    private Button _mainmenuButton;

    public override void LoadContent(SpriteBatch spriteBatch)
    {
        base.LoadContent(spriteBatch);
        _button = _content.Load<Texture2D>("ItemSlot"); //Change Later
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
    
    protected override void SetupHUD()
    {
        int TextWidth = Singleton.SCREEN_WIDTH / 2;
        int TextHeight = 80;
        // Static text
        _pauseTitle = new TextUI(
            new Rectangle((Singleton.SCREEN_WIDTH - TextWidth) / 2 , (Singleton.SCREEN_HEIGHT - TextHeight) / 6, TextWidth, TextHeight),
            "Pause",  
            Color.White, 
            TextUI.TextAlignment.Center
        );

        int ButtonWidth = Singleton.SCREEN_WIDTH / 3;
        int ButtonHeight = 80;

        Texture2D ResumeButtonTexture = _content.Load<Texture2D>("ResumeButtonStatic");
        Texture2D RestartButtonTexture = _content.Load<Texture2D>("RestartButtonStatic");
        Texture2D SoundSettingButtonTexture = _content.Load<Texture2D>("SoundSettingButtonStatic");
        Texture2D MainmenuButtonTexture = _content.Load<Texture2D>("MainmenuButtonStatic");
        _resumeButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 2 / 6, ButtonWidth, ButtonHeight),
            ResumeButtonTexture,
            "",
            Color.Wheat
        );
        _resumeButton.OnClick += ResumeButton_OnClick;

        _restartButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 3 / 6, ButtonWidth, ButtonHeight),
            RestartButtonTexture,
            "",
            Color.Wheat
        );
        _restartButton.OnClick += RestartButton_OnClick;

        _settingsButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 4 / 6, ButtonWidth, ButtonHeight),
            SoundSettingButtonTexture,
            "",
            Color.Wheat
        );
        _settingsButton.OnClick += SettingButton_OnClick;

        _mainmenuButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 5 / 6, ButtonWidth, ButtonHeight),
            MainmenuButtonTexture,
            "",
            Color.Wheat
        );
        _mainmenuButton.OnClick += MainMenuButton_OnClick;

        _ui.AddHUDElement(_pauseTitle);
        _ui.AddHUDElement(_resumeButton);
        _ui.AddHUDElement(_restartButton);
        _ui.AddHUDElement(_settingsButton);
        _ui.AddHUDElement(_mainmenuButton);
    }
}
