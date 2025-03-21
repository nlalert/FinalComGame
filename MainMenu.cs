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

public class MainMenu
{
    private Texture2D _SpriteTexture;

    private SpriteBatch _spriteBatch;
    private ContentManager _content;

    private SpriteFont _font;
    private Button _BackButton;
    private Button _StartButton;
    private Button _ScoreBoardButton;
    private Button _ExitButton;

     public void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager content)
    {
        Console.WriteLine("MainMenu Init");
        _content = content;
    }

    public void LoadContent(SpriteBatch spriteBatch)
    {
        Console.WriteLine("Loading Content");
        _spriteBatch = spriteBatch;
        _font = _content.Load<SpriteFont>("GameFont");
        // _SpriteTexture = _content.Load<Texture2D>("Sprite_Sheet");

        SetupUI();

        // Reset();
        Console.WriteLine("Content Loaded");
    }

    private void SetupUI()
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

        Singleton.Instance.UI.AddElement(_StartButton);
        Singleton.Instance.UI.AddElement(_ExitButton);

        // _BackButton = new Button(_SpriteTexture)
        // {
        //     Name = "BackButton",
        //     Viewport = ViewportManager.Get("Small_Button"),
        //     HighlightedViewPort = ViewportManager.Get("Small_Button_Highlighted"),
        //     Position = new Vector2(Singleton.SCREEN_WIDTH /2 - ViewportManager.Get("Small_Button").Width/2,
        //             Singleton.SCREEN_HEIGHT - ViewportManager.Get("Small_Button").Height / 2 - 50),
        //     LabelViewPort = ViewportManager.Get("Back_Label"),
        //     HighlightedLabelViewPort = ViewportManager.Get("Back_Label_Highlighted"),
        //     LabelPosition = new Vector2((Singleton.SCREEN_WIDTH - ViewportManager.Get("Back_Label").Width) / 2,
        //             Singleton.SCREEN_HEIGHT - ViewportManager.Get("Back_Label").Height / 2 - 50),
        //     IsActive = true
            
        // };
    }

    public void Update(GameTime gameTime)
    {
    }   

    public void Draw(GameTime gameTime)
    {
    }   

    private void ExitGameButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Exit;
    }

    private void StartGameButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.StartingGame;
    }

    protected void Reset()
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.MainMenu;
    }
}