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
    //UI
    SpriteFont _font;

    List<GameObject> _gameObjects;
    int _numObject;

    private Texture2D _playerTexture;
    private Texture2D _enemyTexture;
    private Texture2D _DogTexture;
    private Texture2D _SlimeTexture;
    private Texture2D _textureAtlas;
    private Camera _camera;
    private TileMap _collisionTileMap;
    private TileMap _enemyMap;
    private TileMap _foreGroundTileMap;
    private TileMap _rockTileMap;
    private TileMap _vegetationTileMap;
    private TileMap _backGroundTileMap;

    private BaseEnemy baseSkeleton;
    private BaseEnemy enemyDog;
    private BaseEnemy enemySlime;

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
        _DogTexture = _content.Load<Texture2D>("EnemyDog");
        _SlimeTexture = _content.Load<Texture2D>("EnemySlime");
        _textureAtlas = _content.Load<Texture2D>("Tileset");

        _font = _content.Load<SpriteFont>("GameFont");
        Singleton.Instance.Debug_Font = _content.Load<SpriteFont>("GameFont");

        _song = _content.Load<Song>("A Night Alone - TrackTribe");
    }

    public override void Update(GameTime gameTime)
    {
        //Update
        _numObject = _gameObjects.Count;
        if(Singleton.Instance.IsKeyPressed(Keys.R))
            this.ResetGame();
        switch (Singleton.Instance.CurrentGameState)
        {
            case Singleton.GameState.StartingGame:
                StopSong();
                ResetGame();
                Singleton.Instance.CurrentGameState = Singleton.GameState.InitializingStage;
                break;
            case Singleton.GameState.InitializingStage:
                StopSong();
                ResetStage();
                // SetUpInitalChipsPattern();

                Singleton.Instance.CurrentGameState = Singleton.GameState.Playing;
                break;
            case Singleton.GameState.Playing:
                PlaySong();
                if(Singleton.Instance.IsKeyJustPressed(Keys.Escape))
                {
                    Singleton.Instance.CurrentGameState = Singleton.GameState.Pause;
                }
                UpdateTileMap(gameTime);
                UpdateAllObjects(gameTime);
                RemoveInactiveObjects();

                _camera.Follow(Singleton.Instance.Player); // Make camera follow the player
                break;
            case Singleton.GameState.StageCompleted:
                if (Singleton.Instance.Stage == 4){
                    Singleton.Instance.CurrentGameState = Singleton.GameState.GameWon;
                    break;
                }
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
                _spriteBatch.DrawString(_font, "Health Bar : " + Singleton.Instance.Player.Health + " / " + Singleton.Instance.Player.maxHealth, new Vector2(10, 10), Color.White);
                _spriteBatch.DrawString(_font, "MP Bar : " + Singleton.Instance.Player.MP + " / " + Singleton.Instance.Player.maxMP, new Vector2(10, 70), Color.White);
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
        // Console.WriteLine(_gameObjects.Count);
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

        CreatePlayer();
        _gameObjects.Add(Singleton.Instance.Player);
        CreateEnemies();
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
        _collisionTileMap = new TileMap(_textureAtlas, StageManager.GetCurrentStageCollisionPath(), 20);
        _enemyMap = new TileMap(StageManager.GetCurrentStageEnemyMapPath());

        Singleton.Instance.Player.Position = StageManager.GetPlayerWorldSpawnPosition(); // get player location of each stage
        _gameObjects.Add(Singleton.Instance.Player);
        SpawnEnemies();
        AddItems();
        SetupUI();
        
        foreach (GameObject s in _gameObjects)
        {
            s.Reset();
        }
    }

    private void CreatePlayer()
    {
        // Load sprite sheets
        Texture2D playerIdle = _content.Load<Texture2D>("Char_Idle");
        Texture2D playerRun = _content.Load<Texture2D>("Char_Run");
        Texture2D playerJump = _content.Load<Texture2D>("Char_Jump");
        Texture2D playerFall = _content.Load<Texture2D>("Char_Fall");
        Texture2D playerMelee = _content.Load<Texture2D>("Char_Melee");
        Texture2D playerDash = _content.Load<Texture2D>("Char_Dash");
        Texture2D playerGlide = _content.Load<Texture2D>("EnemyRed");
        Texture2D playerCharge = _content.Load<Texture2D>("EnemyRed");
        Texture2D playerParticle = new Texture2D(_graphicsDevice, 1, 1);
        
        playerParticle.SetData([new Color(193, 255, 219)]);
        SoundEffect playerJumpSound = _content.Load<SoundEffect>("GoofyAhhJump");
        
        Singleton.Instance.Player = new Player(playerIdle, playerRun, playerMelee, playerJump, playerFall, playerDash, playerGlide, playerCharge, playerParticle)
        {
            Name = "Player",
            Position = StageManager.GetPlayerWorldSpawnPosition(),// get player location of each stage
            Life = 2,
            WalkSpeed = 200,
            crouchSpeed = 100,
            climbSpeed = 100,
            maxHealth = 100,
            maxMP = 100,
            coyoteTime = 0.1f, // 100ms of coyote time
            jumpBufferTime = 0.15f, // 150ms jump buffer
            dashSpeed = 400f,
            dashDuration = 0.4f, // Dash lasts for 0.4 seconds
            dashCooldown = 0.5f,
            dashMP = 20f,
            glideGravityScale = 0.3f, // How much gravity affects gliding (lower = slower fall)
            glideMaxFallSpeed = 80f, // Maximum fall speed while gliding
            glideMP = 10f, // MP cost per second while gliding
            maxChargeTime = 2.0f, // Maximum charge time in seconds
            minChargePower = 1.0f, // Minimum damage/speed multiplier
            maxChargePower = 3.0f, // Maximum damage/speed multiplier
            chargeMPCost = 15f, // MP cost for fully charged shot

            Viewport = new Rectangle(0, 0, 16, 32),
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
            Bullet = new Bullet(_playerTexture, 10)
            {
                Name = "BulletPlayer",
                Viewport = new Rectangle(0, 0, 15, 10)
            }
        };
    }

    private void CreateEnemies()
    {
        baseSkeleton = new SkeletonEnemy(_enemyTexture,_font){
            Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
            Viewport = new Rectangle(0, 0, 32, 64),
            CanCollideTile = true,
        };
        enemyDog = new HellhoundEnemy(_DogTexture,_font){
            Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
            Viewport = new Rectangle(0, 0, 64, 32),
            CanCollideTile = true,
        };
        enemySlime = new SlimeEnemy(_SlimeTexture,_font){
            Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
            Viewport = new Rectangle(0, 0, 32, 32),
            CanCollideTile = true,
        };
    }

    private void SpawnEnemies()
    {
        foreach (var enemy in _enemyMap.GetEnemySpawnPoints())
        {
            switch (enemy.Value)
            {
                case 97:
                    // HellhoundEnemy.
                    enemySlime.Spawn(TileMap.GetTileWorldPositionAt(enemy.Key), _gameObjects);
                    // enemyDog.Spawn(TileMap.GetTileWorldPositionAt(enemy.Key), _gameObjects);
                    // baseSkeleton.Spawn(TileMap.GetTileWorldPositionAt(enemy.Key), _gameObjects);
                    break;
                default:
                    break;
            }
            
        }  
    }

    private void AddItems()
    {
        Texture2D testItem = _content.Load<Texture2D>("Pickaxe");
        Texture2D HealthPotionTemp = _content.Load<Texture2D>("HealthPotionTemp");
        Texture2D Hermes_Boots = _content.Load<Texture2D>("Hermes_Boots");

        _gameObjects.Add(new Item(testItem, "test", TileMap.GetTileWorldPositionAt(18, 90)){
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
            Color.Red,
            Color.Gray
        );

        MPBar playerMP = new MPBar(
            new Rectangle(20, 100, 200, 30),
            Color.SkyBlue,
            Color.Gray
        );

        Texture2D slot = _content.Load<Texture2D>("ItemSlot");
        ItemSlot Slot1 = new ItemSlot(
            0, // first slot
            new Rectangle(250, 30, 50, 50),
            slot,
            slot,
            _font
        );
        ItemSlot Slot2 = new ItemSlot(
            1, //second slot
            new Rectangle(350, 30, 50, 50),
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
