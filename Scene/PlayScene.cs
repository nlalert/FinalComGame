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
    private Texture2D _DemonTexture;
    private Texture2D _DemonBulletTexture;
    private Texture2D _TowerTexture;
    private Texture2D _PlatformTexture;

    private Texture2D _parallaxFGtexture;
    private Texture2D _parallaxMGtexture;
    private Texture2D _parallaxBGtexture;
    private ParallaxBackground _parallaxBackground;

    private TileMap _collisionTileMap;
    private TileMap _enemyMap;
    private TileMap _FGTileMap;
    private TileMap _MGTileMap;
    private TileMap _BGTileMap;

    private Camera _camera;

    private BaseEnemy _baseSkeleton;
    private BaseEnemy _enemyDog;
    private BaseEnemy _enemySlime;
    private BaseEnemy _enemyDemon;
    private BaseEnemy _enemyTower;
    private BaseEnemy _enemyPlatform;

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
        _DemonTexture = _content.Load<Texture2D>("EnemyDemon");
        _DemonBulletTexture = _content.Load<Texture2D>("EnemyDemon");
        _TowerTexture = _content.Load<Texture2D>("EnemyTower");
        _PlatformTexture = _content.Load<Texture2D>("EnemyPlatform");

        _textureAtlas = _content.Load<Texture2D>("Tileset");
        _parallaxFGtexture = _content.Load<Texture2D>("Level_1_Parallax_fg");
        _parallaxMGtexture = _content.Load<Texture2D>("Level_1_Parallax_mg");
        _parallaxBGtexture = _content.Load<Texture2D>("Level_1_Parallax_bg");

        _font = _content.Load<SpriteFont>("GameFont");
        Singleton.Instance.Debug_Font = _content.Load<SpriteFont>("GameFont");

        _song = _content.Load<Song>("ChillSong");
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
                _parallaxBackground.Update();
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
                _parallaxBackground.Draw(_spriteBatch);
                DrawTileMap();
                DrawAllObjects();
                _spriteBatch.End();

                _spriteBatch.Begin(); 
                _spriteBatch.DrawString(_font, "Health Bar : " + Singleton.Instance.Player.Health + " / " + Singleton.Instance.Player.MaxHealth, new Vector2(10, 10), Color.White);
                _spriteBatch.DrawString(_font, "MP Bar : " + Singleton.Instance.Player.MP + " / " + Singleton.Instance.Player.MaxMP, new Vector2(10, 70), Color.White);
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
        _BGTileMap.Draw(_spriteBatch);
        _MGTileMap.Draw(_spriteBatch);
        _FGTileMap.Draw(_spriteBatch);
        
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

        _parallaxBackground = new ParallaxBackground(_parallaxFGtexture, _parallaxMGtexture, _parallaxBGtexture, StageManager.GetPlayerWorldSpawnPosition());

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
        _BGTileMap = new TileMap(_textureAtlas, "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_BackGround.csv", 20);
        _MGTileMap = new TileMap(_textureAtlas, "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_MidGround.csv", 20);
        _FGTileMap = new TileMap(_textureAtlas, "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_ForeGround.csv", 20);
        _collisionTileMap = new TileMap(_textureAtlas, StageManager.GetCurrentStageCollisionPath(), 20);

        _parallaxBackground = new ParallaxBackground(_parallaxFGtexture, _parallaxMGtexture, _parallaxBGtexture, StageManager.GetPlayerWorldSpawnPosition());

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
            CrouchSpeed = 100,
            ClimbSpeed = 100,
            MaxHealth = 100,
            MaxMP = 100,

            AttackDamage = 10f,
            AttackDuration = 0.4f, // How long the attack lasts
            AttackCooldown = 0.2f,

            JumpStrength = 800f,

            CoyoteTime = 0.1f, // 100ms of coyote time
            JumpBufferTime = 0.15f, // 150ms jump buffer
            DashSpeed = 400f,
            DashDuration = 0.4f, // Dash lasts for 0.4 seconds
            DashCooldown = 0.5f,
            DashMP = 20f,
            GlideGravityScale = 0.3f, // How much gravity affects gliding (lower = slower fall)
            GlideMaxFallSpeed = 80f, // Maximum fall speed while gliding
            GlideMP = 10f, // MP cost per second while gliding
            MaxChargeTime = 2.0f, // Maximum charge time in seconds
            MinChargePower = 1.0f, // Minimum damage/speed multiplier
            MaxChargePower = 3.0f, // Maximum damage/speed multiplier
            ChargeMPCost = 15f, // MP cost for fully charged shot

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
            Bullet = new PlayerBullet(_playerTexture)
            {
                Name = "BulletPlayer",
                Viewport = new Rectangle(0, 0, 15, 10)
            }
        };
    }

    private void CreateEnemies()
    {
        _baseSkeleton = new SkeletonEnemy(_enemyTexture,_font){
            Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
            Viewport = new Rectangle(0, 0, 32, 64),
            CanCollideTile = true,
            MaxHealth = 80f,
            AttackDamage = 5f,

            LimitIdlePatrol = 100,

            IgnorePlayerDuration = 3f,
        };
        _enemyDog = new HellhoundEnemy(_DogTexture,_font){
            Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
            Viewport = new Rectangle(0, 0, 64, 32),
            CanCollideTile = true,
            MaxHealth = 100f,
            AttackDamage = 8f,

            LimitIdlePatrol = 100,
            
            ChargeTime = 2.0f,
            ChaseDuration = 5f,
            DashDuration = 1.0f,
        };
        _enemySlime = new SlimeEnemy(_SlimeTexture,_font){
            Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
            Viewport = new Rectangle(0, 0, 32, 32),
            CanCollideTile = true,
            MaxHealth = 50f,
            AttackDamage = 3f,

            JumpCooldown = 1.5f,
            JumpStrength = 750,
            Friction = 0.96f
        };
        _enemyDemon = new DemonEnemy(_DemonTexture,_font){
            Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
            Viewport = new Rectangle(0, 0, 32, 64),
            CanCollideTile = true,
            DemonBullet = new DemonBullet(_DemonBulletTexture)
        };
        _enemyTower = new TowerEnemy(_DemonTexture,_font){
            Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
            Viewport = new Rectangle(0, 0, 32, 32),
            CanCollideTile = true,
            TowerBullet = new TowerBullet(_DemonBulletTexture)
        };
        _enemyPlatform = new PlatformEnemy(_PlatformTexture,_font){
            Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
            Viewport = new Rectangle(0, 0, 64, 32),
            CanCollideTile = true,
        };
    }

    private void SpawnEnemies()
    {
        // _baseSkeleton.Spawn(TileMap.GetTileWorldPositionAt(Singleton.Instance.Player.Position), _gameObjects);
        _enemyPlatform.Spawn(Singleton.Instance.Player.Position, _gameObjects);

        foreach (var enemySpawnPoint in _collisionTileMap.GetEnemySpawnPoints())
        {
            switch (enemySpawnPoint.Value)
            {
                case 97:
                    // HellhoundEnemy.
                    // _enemySlime.Spawn(TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key), _gameObjects);
                    // _enemyDog.Spawn(TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key), _gameObjects);
                    // _baseSkeleton.Spawn(TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key), _gameObjects);
                    // _enemyDemon.Spawn(TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key), _gameObjects);
                    // _enemyTower.Spawn(TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key)+ new Vector2(0,-196), _gameObjects);

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
        Texture2D LifeUP = _content.Load<Texture2D>("1Up");

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

        _gameObjects.Add(new LifeUp(LifeUP, "Testing 1Up", TileMap.GetTileWorldPositionAt(16, 98)){
            Name =  "1Up",
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
