using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace FinalComGame;

public class PlayScene 
{
    //System
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private ContentManager _content;

    //UI
    SpriteFont _font;
    private UI _ui;

    List<GameObject> _gameObjects;
    int _numObject;

    private GraphicsDevice _graphicsDevice;
    private Texture2D _playerTexture;
    private Texture2D _enemyTexture;

    private Camera _camera;
    private TileMap _collisionTileMap;
    private TileMap _foreGroundTileMap;
    private TileMap _rockTileMap;
    private TileMap _vegetationTileMap;
    private TileMap _backGroundTileMap;

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
        Singleton.Instance.Debug_Font = _content.Load<SpriteFont>("GameFont");
        Texture2D textureAtlas = _content.Load<Texture2D>("Tileset");
        //_backGroundTileMap = new TileMap(textureAtlas, "../../../Data/Level_0/Level_0_Background.csv", 20);
        //_foreGroundTileMap = new TileMap(textureAtlas, "../../../Data/Level_0/Level_0_Ground.csv", 20);
        _collisionTileMap = new TileMap(textureAtlas, "../../../Data/Level_1/Level_1_Collision.csv", 20);

        _ui = new UI();

        Reset();
    }

    public void Update(GameTime gameTime)
    {
        //Update
        _numObject = _gameObjects.Count;
        if(Singleton.Instance.IsKeyPressed(Keys.R))
            this.Reset();
        switch (Singleton.Instance.CurrentGameState)
        {
            case Singleton.GameState.Playing:
                UpdateTileMap(gameTime);
                UpdateAllObjects(gameTime);
                RemoveInactiveObjects();

                _camera.Follow(player); // Make camera follow the player

                // Update UI
                _ui.Update(gameTime);
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
                DrawUI();
                break;
        }

        _graphics.BeginDraw();
    }

    private void DrawUI()
    {
        _spriteBatch.Begin(); 

        _ui.Draw(_spriteBatch);
        _spriteBatch.DrawString(_font, "Health Bar : " + player.Health + " / " + player.maxHealth, new Vector2(10, 10), Color.White);
        _spriteBatch.DrawString(_font, "MP Bar : " + player.MP + " / " + player.maxMP, new Vector2(10, 70), Color.White);

        _spriteBatch.End();
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
        //_backGroundTileMap.Draw(_spriteBatch);
        //_foreGroundTileMap.Draw(_spriteBatch);
        
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
        Texture2D playerIdle = _content.Load<Texture2D>("Char_Idle");
        Texture2D playerRun = _content.Load<Texture2D>("Char_Run");
        Texture2D playerJump = _content.Load<Texture2D>("Char_Jump");
        Texture2D playerFall = _content.Load<Texture2D>("Char_Fall");
        Texture2D playerMelee = _content.Load<Texture2D>("Char_Melee");
        Texture2D playerDash = _content.Load<Texture2D>("Char_Idle");
        Texture2D playerGlide = _content.Load<Texture2D>("EnemyRed");
        Texture2D playerParticle = new Texture2D(_graphicsDevice, 1, 1);
        
        playerParticle.SetData([new Color(193, 255, 219)]);

        player = new Player(playerIdle, playerRun, playerMelee, playerJump, playerFall, playerDash, playerGlide, playerParticle)
        {
            Name = "Player",
            Viewport = new Rectangle(0, 0, 16, 32),
            Position = TileMap.GetTileWorldPositionAt(16, 98),// block column 16, row 98
            WalkSpeed = 200,
            Left = Keys.Left,
            Right = Keys.Right,
            Crouch = Keys.Down,
            Climb = Keys.Up,
            Fire = Keys.E,
            Jump = Keys.Space,
            Dash = Keys.LeftShift,
            Attack = Keys.Q,
            Interact = Keys.F,
            Bullet = new Bullet(_playerTexture)
            {
                Name = "BulletPlayer",
                Viewport = new Rectangle(0, 0, 15, 10)
            }
        };

        _gameObjects.Add(player);
        
        baseSkeleton = new SkeletonEnemy(_enemyTexture,_font){
            Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
            Viewport = new Rectangle(0, 0, 32, 64),
            CanCollideTile = true,
            player = player
        };
        _gameObjects.Add(baseSkeleton);
        // baseSkeleton.Spawn(132, 400, _gameObjects);
        baseSkeleton.Spawn(262, 200, _gameObjects);
        baseSkeleton.Spawn(462, 100, _gameObjects);

        Texture2D testItem = _content.Load<Texture2D>("Pickaxe");

        _gameObjects.Add(new Item(testItem, "test", false, new Vector2(557,1408)){
            Name =  "Pickaxe",
            Viewport = new Rectangle(0, 0, 32,32)
        });
        SetupUI();

        foreach (GameObject s in _gameObjects)
        {
            s.Reset();
        }
    }

    private void SetupUI()
    {
        HealthBar playerHealth = new HealthBar(
            new Rectangle(20, 40, 200, 30),
            player,
            Color.Red,
            Color.Gray
        );

        MPBar playerMP = new MPBar(
            new Rectangle(20, 100, 200, 30),
            player,
            Color.SkyBlue,
            Color.Gray
        );

        Texture2D slot = _content.Load<Texture2D>("ItemSlot");
        ItemSlot Slot1 = new ItemSlot(
            new Rectangle(250, 30, 50, 50),
            player,
            slot,
            slot,
            _font
        );

        _ui.AddElement(playerHealth);
        _ui.AddElement(playerMP);
        _ui.AddElement(Slot1);
    }
}
