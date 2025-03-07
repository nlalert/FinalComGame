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
    private Texture2D _playerTexture;
    private Texture2D _enemyTexture;
    private Texture2D _blockTexture;
    int _numObject;
    private Camera _camera;

    private Player player;
    private BaseEnemy baseSkeleton;
    private Tile tileTest;

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
        _spriteBatch = spriteBatch;
        _font = content.Load<SpriteFont>("GameFont");
        _playerTexture = content.Load<Texture2D>("Char_test");
        _enemyTexture = content.Load<Texture2D>("EnemyRed");
        _blockTexture = content.Load<Texture2D>("Ground_test");
        Reset();
    }

    public void Update(GameTime gameTime)
    {
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
    }

    public void Draw(GameTime gameTime)
    {
        _numObject = _gameObjects.Count;

        switch (Singleton.Instance.CurrentGameState)
        {
            case Singleton.GameState.Playing:
                // Draw the Game World (Apply Camera)
                _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetTransformation()); // Apply camera matrix
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

        player = new Player(_playerTexture)
        {
            Name = "Player",
            Viewport = new Rectangle(0, 0, 16, 32),
            Position = new Vector2(62, 640),
            Speed = 400,
            Left = Keys.Left,
            Right = Keys.Right,
            Fire = Keys.E,
            Jump = Keys.Space,
            Bullet = new Bullet(_playerTexture)
            {
                Name = "BulletPlayer",
                Viewport = new Rectangle(0, 0, 15, 10)
            }
        };

        _gameObjects.Add(player);

        int level = Singleton.SCREEN_HEIGHT/Singleton.BLOCK_SIZE + 1;
        for (int i = 0; i <= Singleton.SCREEN_WIDTH/Singleton.BLOCK_SIZE; i++)
        {
            tileTest = new Tile(_blockTexture)
            {
                Name = "Tile",
                Position = new Vector2(i * Singleton.BLOCK_SIZE,level * Singleton.BLOCK_SIZE),
                Viewport = new Rectangle(0, 0, Singleton.BLOCK_SIZE, Singleton.BLOCK_SIZE),
                IsSolid = true
            };
            _gameObjects.Add(tileTest);
        }

        List<Vector2> tilePositions = new List<Vector2>
        {
            new Vector2(0, 18),
            new Vector2(21, 18),
            new Vector2(5, 18),
            new Vector2(6, 17),
            new Vector2(7, 17),
            new Vector2(8, 17),
            new Vector2(4, 14), // 28*16 = 448
            new Vector2(10, 12),
            new Vector2(11, 11),
            new Vector2(12, 10),
            new Vector2(13, 9),
            new Vector2(14, 8)
        };

        foreach (var pos in tilePositions)
        {
            Tile tile = new Tile(_blockTexture)
            {
                Name = "Tile",
                Position = new Vector2(pos.X * Singleton.BLOCK_SIZE, 2 *pos.Y * Singleton.BLOCK_SIZE), // Convert grid position to pixel position
                Viewport = new Rectangle(0, 0, Singleton.BLOCK_SIZE, Singleton.BLOCK_SIZE),
                IsSolid = true
            };

            _gameObjects.Add(tile);
        }


        
        baseSkeleton = new SkeletonEnemy(_enemyTexture){
            Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
            Viewport = new Rectangle(0, 0, 32, 64),
            // Position = new Vector2(162, 640),
        };
        _gameObjects.Add(baseSkeleton);
        baseSkeleton.Spawn(162, 600, _gameObjects);
        baseSkeleton.Spawn(262, 600, _gameObjects);
        baseSkeleton.Spawn(362, 600, _gameObjects);

        foreach (GameObject s in _gameObjects)
        {
            s.Reset();
        }
    }
}
