using System;
using System.Collections.Generic;
using FinalComGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MidtermComGame;

public class GameManager : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // Main Menu Variables
    

    // Game Scene (MainScene)
    private PlayScene _playScene;
    private MainMenu _mainMenu;
    private PauseMenu _pauseMenu;
    private Settings _settings;
    private GameOverScene _gameOverScene;
    private GameClearScene _gameClearScene;

    public GameManager()
    {
        Window.Title = "Unfinished History";
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = Singleton.SCREEN_WIDTH;
        _graphics.PreferredBackBufferHeight = Singleton.SCREEN_HEIGHT;

        Singleton.Instance.CurrentGameState = Singleton.GameState.MainMenu;
    }

    protected override void LoadContent()
    {
        Singleton.Instance.GameFont = Content.Load<SpriteFont>("GameFont");

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _playScene = new PlayScene();
        _playScene.Initialize(GraphicsDevice, _graphics, Content);
        _playScene.LoadContent(_spriteBatch);
                
        _mainMenu = new MainMenu();
        _mainMenu.Initialize(GraphicsDevice, _graphics, Content);
        _mainMenu.LoadContent(_spriteBatch);

        _pauseMenu = new PauseMenu();
        _pauseMenu.Initialize(GraphicsDevice, _graphics, Content);
        _pauseMenu.LoadContent(_spriteBatch);

        _settings = new Settings();
        _settings.Initialize(GraphicsDevice, _graphics, Content);
        _settings.LoadContent(_spriteBatch);

        _gameOverScene = new GameOverScene();
        _gameOverScene.Initialize(GraphicsDevice, _graphics, Content);
        _gameOverScene.LoadContent(_spriteBatch);

        _gameClearScene = new GameClearScene();
        _gameClearScene.Initialize(GraphicsDevice, _graphics, Content);
        _gameClearScene.LoadContent(_spriteBatch);
    }

    protected override void Update(GameTime gameTime)
    {
        Singleton.Instance.UpdateCurrentInput();

        switch (Singleton.Instance.CurrentGameState)
        {
            case Singleton.GameState.MainMenu:
                _mainMenu.Update(gameTime);
                break;
            case Singleton.GameState.Pause:
                _pauseMenu.Update(gameTime);
                break;
            case Singleton.GameState.Settings:
                _settings.Update(gameTime);
                break;
            case Singleton.GameState.GameOver:
                _gameOverScene.Update(gameTime);
                break;
            case Singleton.GameState.GameWon:
                _gameClearScene.Update(gameTime);
                break;
            case Singleton.GameState.Exit:
                Exit();
                break;
            default:
                _playScene.Update(gameTime);
                break;
        }

        Singleton.Instance.CurrentUI.Update(gameTime);

        Singleton.Instance.UpdatePreviousInput();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        switch (Singleton.Instance.CurrentGameState)
        {
            case Singleton.GameState.MainMenu:
                // _mainScene.Draw(gameTime);
                _mainMenu.Draw(gameTime);
                break;
            case Singleton.GameState.Pause:
                _playScene.Draw(gameTime);
                _pauseMenu.Draw(gameTime);
                break;
            case Singleton.GameState.Settings:
                _settings.Draw(gameTime);
                break;
            case Singleton.GameState.GameOver:
                _gameOverScene.Draw(gameTime);
                break;
            case Singleton.GameState.GameWon:
                _gameClearScene.Draw(gameTime);
                break;
            default:
                _playScene.Draw(gameTime);
                break;
        }
                
        Singleton.Instance.CurrentUI.DrawWorldSpaceUI(_spriteBatch);
        Singleton.Instance.CurrentUI.DrawHUD(_spriteBatch);
        base.Draw(gameTime);
    }
}