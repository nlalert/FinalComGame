using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Linq;
namespace FinalComGame;

public class MainMenu : Scene
{
    private ImageUI _Title;
    private Button _BackButton;
    private Button _StartButton;
    private Button _ScoreBoardButton;
    private Button _ExitButton;

    public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager content)
    {
        base.Initialize(graphicsDevice, graphicsDeviceManager, content);
    }

    public override void LoadContent(SpriteBatch spriteBatch)
    {
        Console.WriteLine("Loading Content");
        _spriteBatch = spriteBatch;

        SetupHUD();

        // Reset();
        Console.WriteLine("Content Loaded");
    }

    protected override void SetupHUD()
    {
        Texture2D Title = _content.Load<Texture2D>("Gun"); //Change Later TEMP
        int titleWidth = Singleton.SCREEN_WIDTH / 2;
        int titleHeight = 100;
        _Title = new ImageUI(
            Title,
            new Rectangle((Singleton.SCREEN_WIDTH - titleWidth) / 2 , (Singleton.SCREEN_HEIGHT - titleHeight) / 4, titleWidth, titleHeight),
            new Rectangle(0, 0, 32, 32)
        );

        Texture2D Button = _content.Load<Texture2D>("ItemSlot"); //Change Later
        int startButtonWidth = Singleton.SCREEN_WIDTH / 2;
        int startButtonHeight = 100;
        _StartButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - startButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - startButtonHeight) * 2 / 4, startButtonWidth, startButtonHeight),
            Button,
            Button,
            "Start Button",
            Color.Wheat
        );
        _StartButton.OnClick += StartGameButton_OnClick;

        int exitButtonWidth = Singleton.SCREEN_WIDTH / 2;
        int exitButtonHeight = 100;
        _ExitButton = new Button(
            new Rectangle((Singleton.SCREEN_WIDTH - exitButtonWidth) / 2 , (Singleton.SCREEN_HEIGHT - exitButtonHeight) * 3 / 4, exitButtonWidth, exitButtonHeight),
            Button,
            Button,
            "Exit Button",
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