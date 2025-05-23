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
    private TextUI _gameClearDescription; // can change to ImageUI later
    private Button _restartButton;
    private Button _exitButton;
    private Texture2D _UITexture;
    private Rectangle _buttonRectangle;
    public override void LoadContent(SpriteBatch spriteBatch)
    {
        _UITexture = _content.Load<Texture2D>("UI");
        _buttonRectangle = ViewportManager.Get("Button");
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
            new Rectangle((Singleton.SCREEN_WIDTH - TextWidth) / 2 , (Singleton.SCREEN_HEIGHT - TextHeight) / 6, TextWidth, TextHeight),
            "Congratulation!",  
            6,
            Color.LimeGreen, 
            TextUI.TextAlignment.Center
        );

        _gameClearDescription = new TextUI(
            new Rectangle((Singleton.SCREEN_WIDTH - TextWidth) / 2 , (int)((Singleton.SCREEN_HEIGHT - TextHeight) * 2.5f / 6), TextWidth, TextHeight),
            "You escaped and go back to living world\nand deleted your browser history!",  
            3,
            Color.White, 
            TextUI.TextAlignment.Center
        );

        int ButtonWidth = Singleton.SCREEN_WIDTH / 2;
        int ButtonHeight = 80;
        
        _restartButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - ButtonHeight) * 4 / 6, ButtonWidth, ButtonHeight),
            _UITexture,
            "Restart",
            Color.Wheat,
            _buttonRectangle,
            2
        );
        _restartButton.OnClick += RestartButton_OnClick;

        _exitButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - ButtonWidth) / 2, (Singleton.SCREEN_HEIGHT - ButtonHeight) * 5 / 6, ButtonWidth, ButtonHeight),
            _UITexture,
            "Exit to Desktop",
            Color.Wheat,
            _buttonRectangle,
            2
        );
        _exitButton.OnClick += ExitGameButton_OnClick;

        _ui.AddHUDElement(_gameClearTitle);
        _ui.AddHUDElement(_gameClearDescription);
        _ui.AddHUDElement(_restartButton);
        _ui.AddHUDElement(_exitButton);
    }
}
