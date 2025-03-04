using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace FinalComGame;

public class PlayScene 
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    SpriteFont _font;

    List<GameObject> _gameObjects;
    private GraphicsDevice _graphicsDevice;
    private Texture2D _spaceInvaderTexture;
    int _numObject;
    private Camera _camera;

    private Player player;

    // public PlayScene()
    // {
    //     _graphics = new GraphicsDeviceManager(this);
    //     Content.RootDirectory = "Content";
    //     IsMouseVisible = true;
    // }

    public void Initialize(GraphicsDevice graphicsDevice,GraphicsDeviceManager graphicsDeviceManager)
    {
        _graphics = graphicsDeviceManager;
        _graphicsDevice = graphicsDevice;
        _graphics.PreferredBackBufferWidth = Singleton.SCREEN_WIDTH;
        _graphics.PreferredBackBufferHeight = Singleton.SCREEN_HEIGHT;
        _graphics.ApplyChanges();
        _gameObjects = new List<GameObject>();
        _camera = new Camera(_graphicsDevice.Viewport); // Initialize camera
        Reset();
    }

    public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _spriteBatch = new SpriteBatch(_graphicsDevice);
        _font = content.Load<SpriteFont>("GameFont");
        _spaceInvaderTexture = content.Load<Texture2D>("SpaceInvaderSheet");
        Reset();
    }

    public void Update(GameTime gameTime)
    {
        Singleton.Instance.CurrentKey = Keyboard.GetState();

        //Update
        _numObject = _gameObjects.Count;
        
        switch (Singleton.Instance.CurrentGameState)
        {
            case Singleton.GameState.Playing:
                UpdateAllObjects(gameTime);
                RemoveInactiveObjects();

                _camera.Follow(player); // Make camera follow the player
            break;
        }

        Singleton.Instance.PreviousKey = Singleton.Instance.CurrentKey;

        Console.WriteLine(_gameObjects.Count);

    }

    public void Draw(GameTime gameTime)
    {
        _numObject = _gameObjects.Count;

        switch (Singleton.Instance.CurrentGameState)
        {
            case Singleton.GameState.Playing:
                // Draw the Game World (Apply Camera)
                _spriteBatch.Begin(transformMatrix: _camera.GetTransformation()); // Apply camera matrix
                for (int i = 0; i < _numObject; i++)
                {
                    _gameObjects[i].Draw(_spriteBatch);
                }    
                _spriteBatch.End();

                //  Draw the UI (No Camera Transformation)
                _spriteBatch.Begin(); 
                _spriteBatch.DrawString(_font, "Test UI always move with player, must not move out of screen", new Vector2(10, 10), Color.White);
                _spriteBatch.End();
            break;
        }

        _graphics.BeginDraw();

    }

    public void UpdateAllObjects(GameTime gameTime)
    {
        for (int i = 0; i < _numObject; i++)
        {
            if(_gameObjects[i].IsActive)
                _gameObjects[i].Update(gameTime, _gameObjects);
        }
    }

    public void RemoveInactiveObjects()
    {
        for (int i = 0; i < _numObject; i++)
        {
            if(!_gameObjects[i].IsActive)
            {
                _gameObjects.RemoveAt(i);
                i--;
                _numObject--;
            }
        }
    }

    public void Reset()
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Playing;

        Singleton.Instance.Random = new System.Random();

        
    
        _gameObjects.Clear();

        player = new Player(_spaceInvaderTexture)
        {
            Name = "Player",
            Viewport = new Rectangle(51, 30, 54, 30),
            Position = new Vector2(62, 640),
            Left = Keys.Left,
            Right = Keys.Right,
            Fire = Keys.E,
            Jump = Keys.Space,
            Bullet = new Bullet(_spaceInvaderTexture)
            {
                Name = "BulletPlayer",
                Viewport = new Rectangle(216, 36, 3, 24),
                Velocity = new Vector2(0, -600f)
            }
        };

        _gameObjects.Add(player);

        foreach (GameObject s in _gameObjects)
        {
            s.Reset();
        }
    }
}
