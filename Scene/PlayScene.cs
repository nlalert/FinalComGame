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
    private ContentManager _content;

    SpriteFont _font;

    List<GameObject> _gameObjects;
    private GraphicsDevice _graphicsDevice;
    private Texture2D _playerTexture;
    private Texture2D _enemyTexture;

    int _numObject;
    private Camera _camera;
    private TileMap _collisionTileMap;
    private TileMap _foreGroundTileMap;
    private TileMap _middleGroundTileMap;

    private Player player;
    private BaseEnemy baseSkeleton;
    public void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager content)
    {
        _graphics = graphicsDeviceManager;
        _graphicsDevice = graphicsDevice;
        _graphics.PreferredBackBufferWidth = Singleton.SCREEN_WIDTH;
        _graphics.PreferredBackBufferHeight = Singleton.SCREEN_HEIGHT;
        _graphics.ApplyChanges();

        _content = content;

        _gameObjects = new List<GameObject>();
        _camera = new Camera(_graphicsDevice.Viewport); // Initialize camera
    }

    public void LoadContent(SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
        _font = _content.Load<SpriteFont>("GameFont");
        _playerTexture = _content.Load<Texture2D>("Char_test");
        _enemyTexture = _content.Load<Texture2D>("EnemyRed");

        Texture2D textureAtlas = _content.Load<Texture2D>("Tiles_Test");
        _middleGroundTileMap = new TileMap(textureAtlas, "../../../Data/Level_Test/Level_Test_Mg.csv", 8);
        _foreGroundTileMap = new TileMap(textureAtlas, "../../../Data/Level_Test/Level_Test_Fg.csv", 8);
        _collisionTileMap = new TileMap(textureAtlas, "../../../Data/Level_Test/Level_Test_Collision.csv", 8);

        Reset();
    }

    public void Update(GameTime gameTime)
    {
        //Update
        _numObject = _gameObjects.Count;
        
        switch (Singleton.Instance.CurrentGameState)
        {
            case Singleton.GameState.Playing:
                UpdateTileMap(gameTime);
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
                DrawTileMap();
                DrawAllObjects();
                _spriteBatch.End();

                //  Draw the UI (No Camera Transformation)
                _spriteBatch.Begin(); 
                _spriteBatch.DrawString(_font, "Test UI always move with player, must not move out of screen", new Vector2(10, 10), Color.White);
                _spriteBatch.End();
                break;
        }

        _graphics.BeginDraw();
    }

    private void UpdateTileMap(GameTime gameTime)
    {
        _collisionTileMap.Update(gameTime, _gameObjects);
    }

    public void UpdateAllObjects(GameTime gameTime)
    {
        for (int i = 0; i < _numObject; i++)
        {
            if(_gameObjects[i].IsActive)
                _gameObjects[i].Update(gameTime, _gameObjects, _collisionTileMap);
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

    private void DrawTileMap()
    {
        _middleGroundTileMap.Draw(_spriteBatch);
        _foreGroundTileMap.Draw(_spriteBatch);
        
        //Should be hidden
        _collisionTileMap.Draw(_spriteBatch);
    }

    private void DrawAllObjects()
    {
        for (int i = 0; i < _numObject; i++)
        {
            _gameObjects[i].Draw(_spriteBatch);
        }   
    }

    public void Reset()
    {
        Singleton.Instance.CurrentGameState = Singleton.GameState.Playing;

        Singleton.Instance.Random = new Random();

        _gameObjects.Clear();

        // Load sprite sheets
        Texture2D playerIdle = _content.Load<Texture2D>("Char_Animation_Test");
        Texture2D playerRun = _content.Load<Texture2D>("EnemyRed");
        Texture2D playerJump = _content.Load<Texture2D>("Player");

        player = new Player(playerIdle, playerRun, playerJump)
        {
            Name = "Player",
            Viewport = new Rectangle(0, 0, 16, 32),
            Position = new Vector2(Singleton.SCREEN_WIDTH/2, Singleton.SCREEN_HEIGHT/2),
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
