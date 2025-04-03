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

    public override void LoadContent(SpriteBatch spriteBatch)
    {
        base.LoadContent(spriteBatch);

        SetupHUD();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        PauseSong();
        _gameManager.IsMouseVisible = true;
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

        Texture2D Button = _content.Load<Texture2D>("ItemSlot"); //Change Later
        int ButtonWidth = Singleton.SCREEN_WIDTH / 2;
        int ButtonHeight = 80;

        _resumeButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 2 / 6, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "Resume",
            Color.Wheat
        );
        _resumeButton.OnClick += ResumeButton_OnClick;

        _restartButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 3 / 6, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "Restart Game",
            Color.Wheat
        );
        _restartButton.OnClick += RestartButton_OnClick;

        _settingsButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 4 / 6, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "Settings",
            Color.Wheat
        );
        _settingsButton.OnClick += SettingButton_OnClick;

        _mainmenuButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 5 / 6, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "Back to Main Menu",
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
