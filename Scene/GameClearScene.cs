using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FinalComGame;

public class GameClearScene : Scene
{
    private TextUI _gameClearTitle; // can change to ImageUI later
    private Button _restartButton;
    private Button _exitButton;

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
        _gameClearTitle = new TextUI(
            new Rectangle((Singleton.SCREEN_WIDTH - TextWidth) / 2 , (Singleton.SCREEN_HEIGHT - TextHeight) / 5, TextWidth, TextHeight),
            "Congratulation!",  
            Color.White, 
            TextUI.TextAlignment.Center
        );

        Texture2D Button = _content.Load<Texture2D>("ItemSlot"); //Change Later
        int ButtonWidth = Singleton.SCREEN_WIDTH / 2;
        int ButtonHeight = 80;

        _restartButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 2 / 5, ButtonWidth, ButtonHeight),
            Button,
            "Restart",
            Color.Wheat
        );
        _restartButton.OnClick += RestartButton_OnClick;

        _exitButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2, (Singleton.SCREEN_HEIGHT - ButtonHeight) * 4 / 5, ButtonWidth, ButtonHeight),
            Button,
            "Exit to Desktop",
            Color.Wheat
        );
        _exitButton.OnClick += ExitGameButton_OnClick;

        _ui.AddHUDElement(_gameClearTitle);
        _ui.AddHUDElement(_restartButton);
        _ui.AddHUDElement(_exitButton);
    }
}
