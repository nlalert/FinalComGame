using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FinalComGame;

public class GameOverScene : Scene
{
    private TextUI _gameOverTitle; // can change to ImageUI later
    private Button _restartButton;
    private Button _mainmenuButton;
    private Button _exitButton;
    
    public override void LoadContent(SpriteBatch spriteBatch)
    {
        base.LoadContent(spriteBatch);

        SetupHUD();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        StopSong();
        _gameManager.IsMouseVisible = true;
    }

    protected override void SetupHUD()
    {
        int TextWidth = Singleton.SCREEN_WIDTH / 2;
        int TextHeight = 80;
        // Static text
        _gameOverTitle = new TextUI(
            new Rectangle((Singleton.SCREEN_WIDTH - TextWidth) / 2 , (Singleton.SCREEN_HEIGHT - TextHeight) / 5, TextWidth, TextHeight),
            "GAME OVER",  
            Color.White, 
            TextUI.TextAlignment.Center
        );

        Texture2D Button = _content.Load<Texture2D>("UI");
        int ButtonWidth = Singleton.SCREEN_WIDTH / 2;
        int ButtonHeight = 80;
        Rectangle buttonRectangle = new Rectangle(0, 48, 304, 48);
        _restartButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 2 / 5, ButtonWidth, ButtonHeight),
            Button,
            "Restart",
            Color.Wheat,
            buttonRectangle
        );
        _restartButton.OnClick += RestartButton_OnClick;

        _mainmenuButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 3 / 5, ButtonWidth, ButtonHeight),
            Button,
            "Back to Main Menu",
            Color.Wheat,
            buttonRectangle
        );
        _mainmenuButton.OnClick += MainMenuButton_OnClick;

        _exitButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2, (Singleton.SCREEN_HEIGHT - ButtonHeight) * 4 / 5, ButtonWidth, ButtonHeight),
            Button,
            "Exit to Desktop",
            Color.Wheat,
            buttonRectangle
        );
        _exitButton.OnClick += ExitGameButton_OnClick;

        _ui.AddHUDElement(_gameOverTitle);
        _ui.AddHUDElement(_restartButton);
        _ui.AddHUDElement(_mainmenuButton);
        _ui.AddHUDElement(_exitButton);
    }
}
