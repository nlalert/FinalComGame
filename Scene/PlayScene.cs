using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace FinalComGame;

public class PlayScene : Scene
{
    Song _gameMusic;

    //UI
    SpriteFont _font;

    List<GameObject> _gameObjects;
    int _numObject;

    private Texture2D _playerTexture;
    private Texture2D _enemyTexture;
    private Texture2D _textureAtlas;
    private Camera _camera;
    private TileMap _collisionTileMap;
    private TileMap _foreGroundTileMap;
    private TileMap _rockTileMap;
    private TileMap _vegetationTileMap;
    private TileMap _backGroundTileMap;

    private Player player;
    private BaseEnemy baseSkeleton;
    public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager content)
    {
        base.Initialize(graphicsDevice, graphicsDeviceManager, content);

        _graphics.PreferredBackBufferWidth = Singleton.SCREEN_WIDTH;
        _graphics.PreferredBackBufferHeight = Singleton.SCREEN_HEIGHT;
        _graphics.ApplyChanges();

        _gameObjects = new List<GameObject>();
        _camera = new Camera(_graphicsDevice.Viewport); // Initialize camera
    }

    public override void LoadContent(SpriteBatch spriteBatch)
    {
        base.LoadContent(spriteBatch);
        
        _playerTexture = _content.Load<Texture2D>("Char_test");
        _enemyTexture = _content.Load<Texture2D>("EnemyRed");
        _textureAtlas = _content.Load<Texture2D>("Tileset");

        _font = _content.Load<SpriteFont>("GameFont");
        Singleton.Instance.Debug_Font = _content.Load<SpriteFont>("GameFont");

        _gameMusic = _content.Load<Song>("A Night Alone - TrackTribe");
    }

    public override void Update(GameTime gameTime)
    {
        Console.WriteLine(Singleton.Instance.CurrentGameState);
        //Update
        _numObject = _gameObjects.Count;
        if(Singleton.Instance.IsKeyPressed(Keys.R))
            this.ResetGame();
        switch (Singleton.Instance.CurrentGameState)
        {
            case Singleton.GameState.StartingGame:
                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Stop();
                }
                ResetGame();
                Singleton.Instance.CurrentGameState = Singleton.GameState.InitializingStage;
                break;
            case Singleton.GameState.InitializingStage:
                ResetStage();
                // SetUpInitalChipsPattern();

                Singleton.Instance.CurrentGameState = Singleton.GameState.Playing;
                break;
            case Singleton.GameState.Playing:
                if (MediaPlayer.State != MediaState.Playing)
                {
                    MediaPlayer.Play(_gameMusic);
                }
                if(Singleton.Instance.IsKeyJustPressed(Keys.Escape))
                {
                    Singleton.Instance.CurrentGameState = Singleton.GameState.Pause;
                }
                UpdateTileMap(gameTime);
                UpdateAllObjects(gameTime);
                RemoveInactiveObjects();

                _camera.Follow(player); // Make camera follow the player
                break;
            case Singleton.GameState.GameOver:
                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Stop();
                }
                break;
        }

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
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

                _spriteBatch.Begin(); 
                _spriteBatch.DrawString(_font, "Health Bar : " + player.Health + " / " + player.maxHealth, new Vector2(10, 10), Color.White);
                _spriteBatch.DrawString(_font, "MP Bar : " + player.MP + " / " + player.maxMP, new Vector2(10, 70), Color.White);
                _spriteBatch.End();
                break;
        }

        DrawUI();

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

    public void ResetGame()
    {
        _gameObjects.Clear();

        Singleton.Instance.Stage = 1;
        Singleton.Instance.Random = new Random();
        Singleton.Instance.CurrentGameState = Singleton.GameState.InitializingStage;

        AddPlayer();
        AddEnemies();
        AddItems();
        SetupUI();

        foreach (GameObject s in _gameObjects)
        {
            s.Reset();
        }
    }

    protected void ResetStage()
    {
        _gameObjects.Clear();

        Singleton.Instance.Random = new Random();
        //_backGroundTileMap = new TileMap(textureAtlas, "../../../Data/Level_0/Level_0_Background.csv", 20);
        //_foreGroundTileMap = new TileMap(textureAtlas, "../../../Data/Level_0/Level_0_Ground.csv", 20);
        _collisionTileMap = new TileMap(_textureAtlas, StageManager.GetCurrentStagePath(), 20);

        AddPlayer();
        AddEnemies();
        AddItems();

        foreach (GameObject s in _gameObjects)
        {
            s.Reset();
        }
    }

    private void AddPlayer()
    {
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
        SoundEffect playerJumpSound = _content.Load<SoundEffect>("GoofyAhhJump");
        
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
            Item1 = Keys.D1,
            Item2 = Keys.D2,
            JumpSound = playerJumpSound,
            Bullet = new Bullet(_playerTexture)
            {
                Name = "BulletPlayer",
                Viewport = new Rectangle(0, 0, 15, 10)
            }
        };

        _gameObjects.Add(player);
    }

    private void AddEnemies()
    {
        baseSkeleton = new SkeletonEnemy(_enemyTexture,_font){
            Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
            Viewport = new Rectangle(0, 0, 32, 64),
            CanCollideTile = true,
            player = player
        };
        _gameObjects.Add(baseSkeleton);
        // baseSkeleton.Spawn(132, 400, _gameObjects);
        
        baseSkeleton.Spawn(TileMap.GetTileWorldPositionAt(20, 90),_gameObjects);
    }

    private void AddItems()
    {
        Texture2D testItem = _content.Load<Texture2D>("Pickaxe");
        Texture2D HealthPotionTemp = _content.Load<Texture2D>("HealthPotionTemp");
        Texture2D Hermes_Boots = _content.Load<Texture2D>("Hermes_Boots");

        _gameObjects.Add(new Item(testItem, "test", TileMap.GetTileWorldPositionAt(18, 98)){
            Name =  "Pickaxe1",
            Viewport = new Rectangle(0, 0, 32,32)
        });

        _gameObjects.Add(new SpeedBoots(Hermes_Boots, "Hermes_Boots", TileMap.GetTileWorldPositionAt(28, 98)){
            Name =  "HealthPotion",
            Viewport = new Rectangle(0, 0, 32,32)
        });
        
        _gameObjects.Add(new Potion(HealthPotionTemp, "Testing Potion", TileMap.GetTileWorldPositionAt(30, 98)){
            Name =  "HealthPotion",
            Viewport = new Rectangle(0, 0, 32,32)
        });
    }

    protected override void SetupUI()
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
            0, // first slot
            new Rectangle(250, 30, 50, 50),
            player,
            slot,
            slot,
            _font
        );
        ItemSlot Slot2 = new ItemSlot(
            1, //second slot
            new Rectangle(350, 30, 50, 50),
            player,
            slot,
            slot,
            _font
        );

        _ui.AddElement(playerHealth);
        _ui.AddElement(playerMP);
        _ui.AddElement(Slot1);
        _ui.AddElement(Slot2);
    }
}
