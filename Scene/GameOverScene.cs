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

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        StopSong();
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

        Texture2D Button = _content.Load<Texture2D>("ItemSlot"); //Change Later
        int ButtonWidth = Singleton.SCREEN_WIDTH / 2;
        int ButtonHeight = 80;

        _restartButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 2 / 5, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "Restart",
            Color.Wheat
        );
        _restartButton.OnClick += RestartButton_OnClick;

        _mainmenuButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 3 / 5, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "Back to Main Menu",
            Color.Wheat
        );
        _mainmenuButton.OnClick += MainMenuButton_OnClick;

        _exitButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2, (Singleton.SCREEN_HEIGHT - ButtonHeight) * 4 / 5, ButtonWidth, ButtonHeight),
            Button,
            Button,
            "Exit to Desktop",
            Color.Wheat
        );
        _exitButton.OnClick += ExitGameButton_OnClick;

        _ui.AddHUDElement(_gameOverTitle);
        _ui.AddHUDElement(_restartButton);
        _ui.AddHUDElement(_mainmenuButton);
        _ui.AddHUDElement(_exitButton);
    }
}
