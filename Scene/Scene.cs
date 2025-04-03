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

public class Scene
{
    //System
    protected GameManager _gameManager;
    protected GraphicsDeviceManager _graphics;
    protected GraphicsDevice _graphicsDevice;
    protected SpriteBatch _spriteBatch;
    protected ContentManager _content;

    protected UI _ui;

    protected Song _song;

    public virtual void Initialize(GameManager gameManager, GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager content)
    {
        _gameManager = gameManager;
        _graphics = graphicsDeviceManager;
        _graphicsDevice = graphicsDevice;
        _content = content;

        _ui = new UI();
    }

    public virtual void LoadContent(SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
    }

    protected virtual void SetupHUD()
    {
    }

    public virtual void Update(GameTime gameTime)
    {
        Singleton.Instance.CurrentUI = _ui;
    }   

    public virtual void Draw(GameTime gameTime)
    {
    }   
    
    protected void DrawWorldSpaceUI()
    {
        _ui.DrawWorldSpaceUI(_spriteBatch);
    }
    
    protected virtual void Reset()
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.MainMenu;
    }

    protected void StopSong()
    {
        if (MediaPlayer.State != MediaState.Stopped)
        {
            MediaPlayer.Stop();
        }
    }

    protected void PlaySong()
    {
        if (MediaPlayer.State != MediaState.Playing)
        {
            MediaPlayer.Play(_song);
        }
    }

    protected void ResumeSong()
    {
        if (MediaPlayer.State == MediaState.Paused)
        {
            MediaPlayer.Resume();
        }
    }

    protected void PauseSong()
    {
        if (MediaPlayer.State == MediaState.Playing)
        {
            MediaPlayer.Pause();
        }
    }

    protected virtual void ResumeButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Playing;
    }

    protected virtual void RestartButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.StartingGame;
        if(Singleton.Instance.Stage >= 1)
            Singleton.Instance.Stage = 1;
    }

    protected virtual void SettingButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Settings;
    }

    protected virtual void MainMenuButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.MainMenu;
    }

    protected virtual void ExitGameButton_OnClick(object sender, EventArgs e)
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Exit;
    }
}