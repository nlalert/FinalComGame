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
    private SpriteFont _font;
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
        _font = _content.Load<SpriteFont>("GameFont");

        SetupUI();

        // Reset();
        Console.WriteLine("Content Loaded");
    }

    protected override void SetupUI()
    {
        Texture2D Button = _content.Load<Texture2D>("ItemSlot"); //Change Later
        _StartButton = new Button(
            new Rectangle(0,0,200,200),
            Button,
            Button,
            _font,
            "Start Button",
            Color.Wheat
        );
        _StartButton.OnClick += StartGameButton_OnClick;

        _ExitButton = new Button(
            new Rectangle(0,300,200,200),
            Button,
            Button,
            _font,
            "Exit Button",
            Color.Wheat
        );
        _ExitButton.OnClick += ExitGameButton_OnClick;

        _ui.AddElement(_StartButton);
        _ui.AddElement(_ExitButton);
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