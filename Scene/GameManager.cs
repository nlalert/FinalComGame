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
    }

    protected override void Update(GameTime gameTime)
    {
        Singleton.Instance.UpdateCurrentInput();

        //assume game state as playing
        switch (Singleton.Instance.CurrentGameState)
        {
            case Singleton.GameState.MainMenu:
                // _mainScene.Update(gameTime);
                _mainMenu.Update(gameTime);
                break;
            case Singleton.GameState.Pause:
                // _playScene.Update(gameTime);
                _pauseMenu.Update(gameTime);
                break;
            case Singleton.GameState.Exit:
                Exit();
                break;
            default:
                _playScene.Update(gameTime);
                break;
        //     case Singleton.GameState.MainMenu:
        //         _mainScene.Update(gameTime);
        //         _mainMenu.Update(gameTime);
        //         break;
        //     case Singleton.GameState.Pause:
        //         _mainScene.Update(gameTime);
        //         _pauseMenu.Update(gameTime);
        //         break;
        //     case Singleton.GameState.Exit:
        //         Exit();
        //         break;
        //     default:
        //         _mainScene.Update(gameTime);
        //         break;
        }

        Singleton.Instance.UpdatePreviousInput();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // _spriteBatch.Begin();

       
        switch (Singleton.Instance.CurrentGameState)
        {
            case Singleton.GameState.MainMenu:
                // _mainScene.Draw(gameTime);
                _mainMenu.Draw(gameTime);
                break;
            case Singleton.GameState.Pause:
                // _playScene.Draw(gameTime);
                _pauseMenu.Draw(gameTime);
                break;
            default:
                _playScene.Draw(gameTime);
                break;
        //     case Singleton.GameState.MainMenu:
        //         _mainMenu.Draw(gameTime);
        //         break;
        //     case Singleton.GameState.Pause:
        //         _mainScene.Draw(gameTime);
        //         _pauseMenu.Draw(gameTime);
        //         break;
        //     default:
        //         _mainScene.Draw(gameTime);
        //         break;
        }

        // _spriteBatch.End();

        base.Draw(gameTime);
    }
}