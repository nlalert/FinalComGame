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
    protected GraphicsDeviceManager _graphics;
    protected GraphicsDevice _graphicsDevice;
    protected SpriteBatch _spriteBatch;
    protected ContentManager _content;

    protected UI _ui;

    protected Song _song;

    public virtual void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager content)
    {
        _graphics = graphicsDeviceManager;
        _graphicsDevice = graphicsDevice;
        _content = content;

        _ui = new UI();
    }

    public virtual void LoadContent(SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
    }

    protected virtual void SetupUI()
    {
    }

    public virtual void Update(GameTime gameTime)
    {
        Singleton.Instance.CurrentUI = _ui;
    }   

    public virtual void Draw(GameTime gameTime)
    {
    }   
    
    protected virtual void Reset()
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.MainMenu;
    }

    protected void StopSong()
    {
        if (MediaPlayer.State == MediaState.Playing)
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

}