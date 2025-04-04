﻿using System;
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
    List<GameObject> _gameObjects;
    int _numObject;

    private Texture2D _textureAtlas;
    private Texture2D _LaserTexture;
    private Texture2D _HookHeadTexture;
    private Texture2D _RopeTexture; 
    private Texture2D _whiteTexture; // For SignBoard backgrounds         // TODO : Change to desire texture
    private Texture2D _playerTexture;
    private Texture2D _projectileTexture; // Used by many objects
    private Texture2D _itemTexture; // Used for all items
    private Texture2D _slotTexture; // For UI/inventory slots
    private Texture2D _skeletonTexture;
    private Texture2D _hellhoundTexture;
    private Texture2D _slimeTexture;
    private Texture2D _demonTexture;
    private Texture2D _towerTexture;
    private Texture2D _platformEnemyTexture;
    private Texture2D _giantSlimeTexture;
    private Texture2D _cerberusTexture;
    private Texture2D _rhulkTexture;
    private Texture2D _queueTexture;
    private Texture2D _itemSlotTexture;
    private Song _songlevel3;
    private Song _songlevel2;
    private Song _songlevel1;
    private Song _songlevel0;
    // Sound 
    private SoundEffect _jumpSound;
    private SoundEffect _dashSound;
    private SoundEffect _punchSound;
    private SoundEffect _chargeBulletSound;
    private SoundEffect _bulletShotSound;
    private SoundEffect _hitSound;
    private SoundEffect _potionUseSound;
    private SoundEffect _swordSlashSound;
    private SoundEffect _gunshotSound;
    private SoundEffect _fireBallShootingSound;
    private SoundEffect _fireBallExplosionSound;
    private SoundEffect _pickUpSound;

    private SoundEffect _Rhulk_LaserSound;
    private SoundEffect _Rhulk_DashSound;
    
    private StageManager _stageManager;

    public override void Initialize(GameManager gameManager, GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager content)
    {
        base.Initialize(gameManager, graphicsDevice, graphicsDeviceManager, content);

        _graphics.PreferredBackBufferWidth = Singleton.SCREEN_WIDTH;
        _graphics.PreferredBackBufferHeight = Singleton.SCREEN_HEIGHT;
        _graphics.ApplyChanges();

        _gameObjects = new List<GameObject>();
    }

    public override void LoadContent(SpriteBatch spriteBatch)
    {
        base.LoadContent(spriteBatch);

        LoadTextures();
        LoadSounds();
    }

    private void LoadTextures()
    {
        _RopeTexture = _content.Load<Texture2D>("Rope");
        _textureAtlas = _content.Load<Texture2D>("Tileset");
        
        _whiteTexture = new Texture2D(_graphicsDevice, 1, 1);
        _whiteTexture.SetData(new[] { Color.White });
        
        _playerTexture = _content.Load<Texture2D>("Char");
        _projectileTexture = _content.Load<Texture2D>("Projectile");
        _LaserTexture = _content.Load<Texture2D>("Laserbeam");
        _HookHeadTexture = _content.Load<Texture2D>("HookHead");
        _itemTexture = _content.Load<Texture2D>("Items");
        _slotTexture = _content.Load<Texture2D>("ItemSlot");
        
        // Enemy textures
        _skeletonTexture = _content.Load<Texture2D>("Skeleton");
        _hellhoundTexture = _content.Load<Texture2D>("HellHound");
        _slimeTexture = _content.Load<Texture2D>("HellSlime");
        _demonTexture = _content.Load<Texture2D>("Demon");
        _towerTexture = _content.Load<Texture2D>("Spitter");
        _platformEnemyTexture = _content.Load<Texture2D>("Crab");

        _giantSlimeTexture = _content.Load<Texture2D>("LargeSlime");
        _cerberusTexture = _content.Load<Texture2D>("Cerberus");
        _rhulkTexture = _content.Load<Texture2D>("Rhulk");

        _queueTexture = _content.Load<Texture2D>("Queue");

        //UI
        _itemSlotTexture = _content.Load<Texture2D>("ItemSlot");
        //load song
    }

    private void LoadSounds()
    {
        // Load sounds
        _jumpSound = _content.Load<SoundEffect>("GoofyAhhJump");
        _dashSound = _content.Load<SoundEffect>("Dash");
        _punchSound = _content.Load<SoundEffect>("PlayerPunch");
        _chargeBulletSound = _content.Load<SoundEffect>("ChargingBullet");
        _bulletShotSound = _content.Load<SoundEffect>("BulletShot");
        _hitSound = _content.Load<SoundEffect>("HitEnemy");
        _potionUseSound = _content.Load<SoundEffect>("PotionUse");
        _swordSlashSound = _content.Load<SoundEffect>("SwordSlash");
        _gunshotSound = _content.Load<SoundEffect>("Gunshot");
        _fireBallShootingSound = _content.Load<SoundEffect>("FireBallShooting");
        _fireBallExplosionSound = _content.Load<SoundEffect>("FireBallExplosion");
        _pickUpSound = _content.Load<SoundEffect>("PickUp");
        _Rhulk_DashSound = _content.Load<SoundEffect>("RhulkDash_sound");
        _Rhulk_LaserSound = _content.Load<SoundEffect>("Laser_sound");

        // Load songs
        _songlevel0 = _content.Load<Song>("ChillSong");
        _songlevel1 = _content.Load<Song>("Eternity's Divide OST  Snowstorm");
        _songlevel2 = _content.Load<Song>("Eternity's Divide OST  Enter Cold Fusion Core");
        _songlevel3 = _content.Load<Song>("Eternity's Divide OST  Checkpoint 2");
        _song = _songlevel0;//default song

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        //Update
        _numObject = _gameObjects.Count;

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

                UnlockAbilityForStage();
                SetupHUD();
                Singleton.Instance.CurrentGameState = Singleton.GameState.Playing;
                break;
            case Singleton.GameState.Playing:
                ResumeSong();
                PlaySong();
                if(Singleton.Instance.IsKeyJustPressed(Keys.Escape))
                {
                    Singleton.Instance.CurrentGameState = Singleton.GameState.Pause;
                }
                UpdateTileMap(gameTime);
                UpdateAllObjects(gameTime);
                _stageManager.UpdateAmbushAreas(gameTime, _gameObjects);
                RemoveInactiveObjects();

                Singleton.Instance.Camera.Follow(Singleton.Instance.Player); // Make camera follow the player
                UpdateBackGround(gameTime);
                break;
            case Singleton.GameState.StageCompleted:
                UpdateStage();
                break;
        }

        //Console.WriteLine("GameObject :" + _numObject);
        _gameManager.IsMouseVisible = false;
    }

    public override void Draw(GameTime gameTime)
    {
        if(_stageManager == null)
            return;

        _numObject = _gameObjects.Count;

        DrawBackground();
        DrawGameWorld();

        _graphics.BeginDraw();
    }

    private void DrawBackground()
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _stageManager.DrawParallaxBackground(_spriteBatch);
        _spriteBatch.End();
    }

    private void DrawGameWorld()
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Singleton.Instance.Camera.GetTransformation()); // Apply camera matrix
        _stageManager.DrawTileMaps(_spriteBatch);
        DrawAllObjects();
        _spriteBatch.End();
    }

    private void DrawAllObjects()
    {
        for (int i = 0; i < _numObject; i++)
        {
            _gameObjects[i].Draw(_spriteBatch);
        }   
    }

    private void UpdateTileMap(GameTime gameTime)
    {
        _stageManager.UpdateTileMap(gameTime, _gameObjects);
    }

    public void UpdateAllObjects(GameTime gameTime)
    {
        float updateDistanceSquared = Singleton.UPDATE_DISTANCE * Singleton.UPDATE_DISTANCE;
        
        for (int i = 0; i < _numObject; i++)
        {
            GameObject g = _gameObjects[i];
            if (!g.IsActive)
                continue;
                
            if (Vector2.DistanceSquared(g.Position, Singleton.Instance.Player.Position) <= updateDistanceSquared)
                g.Update(gameTime, _gameObjects, _stageManager.CollisionTileMap);
        }
    }

    private void UpdateBackGround(GameTime gameTime)
    {
        _stageManager.UpdateParallaxBackground(gameTime);
    }

    private void UpdateStage()
    {
        if(Singleton.Instance.Stage != 0)
            Singleton.Instance.Player.Life++; // Reward for clearing stage
            
        Singleton.Instance.Stage++;

        

        if (Singleton.Instance.Stage >= 4){
            Singleton.Instance.CurrentGameState = Singleton.GameState.GameWon;
        }
        else
        {
            Singleton.Instance.CurrentGameState = Singleton.GameState.InitializingStage;
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

    public void ResetGame()
    {
        _gameObjects = new List<GameObject>();

        Singleton.Instance.Random = new Random();
        Singleton.Instance.CurrentGameState = Singleton.GameState.InitializingStage;

        CreatePlayer();
        CreateEnemies();
        CreateItemPrefabs();

        foreach (GameObject s in _gameObjects)
        {
            s.Reset();
        }
    }

    protected void ResetStage()
    {
        _gameObjects.Clear();
        
        Singleton.Instance.Random = new Random();
        
        _stageManager = new StageManager();
        _stageManager.LoadTileMaps(_textureAtlas);

        Rectangle mapBounds = new Rectangle(
            0, 
            0,
            _stageManager.GetMapWorldWidth(),  
            _stageManager.GetMapWorldHeight()
        ); // Map size

        Singleton.Instance.Camera = new Camera(_graphicsDevice.Viewport, mapBounds); // Initialize camera

        Singleton.Instance.Player.Position = _stageManager.GetPlayerWorldSpawnPoint();// get player location of each stage
        _gameObjects.Add(Singleton.Instance.Player);

        _stageManager.SetUpParallaxBackground(_content, _graphicsDevice);
        _stageManager.InitializeAmbushAreas();
        AddSignBoard();
        SpawnEnemies();
        SpawnItems();

        if(Singleton.Instance.Stage == 0)
        {
            _song = _songlevel0;
        }
        if(Singleton.Instance.Stage == 1)
        {
            _song = _songlevel1;
        }
        if(Singleton.Instance.Stage == 2)
        {
            _song = _songlevel2;
        }
        if(Singleton.Instance.Stage == 3)
        {
            _song = _songlevel3;
        }

        foreach (GameObject s in _gameObjects)
        {
            s.Reset();
        }
    }

    private void AddSignBoard()
    {
        // TODO : More dynamic stage management sign
        switch (Singleton.Instance.Stage)
        {
            case 0:
                SignBoard WalkTutorialSign = new SignBoard(
                    _whiteTexture,
                    "Press Left or Right Arrow Key to move around!",
                    TileMap.GetTileWorldPositionAt(12, 30),  // TopLeft Position  // TODO : More dynamic
                    200,                    // Width
                    48,                     // Height
                    new Color(10, 10, 40, 220), // Dark blue, semi-transparent
                    Color.Gold
                );
                SignBoard JumpTutorialSign = new SignBoard(
                    _whiteTexture,
                    "Press Space Bar Key to Jump      " +
                    "Longer a you hold Jump Button, Higher the Jump!",
                    TileMap.GetTileWorldPositionAt(30, 28), // TopLeft Position // TODO : More dynamic
                    300,                    // Width
                    64,                     // Height
                    new Color(10, 10, 40, 220), // Dark blue, semi-transparent
                    Color.Gold
                );
                SignBoard ClimbTutorialSign = new SignBoard(
                    _whiteTexture,
                    "Press UP Arrow Key to climb ladder or vines!",
                    TileMap.GetTileWorldPositionAt(55, 22), // TopLeft Position // TODO : More dynamic
                    220,                    // Width
                    48,                     // Height
                    new Color(10, 10, 40, 220), // Dark blue, semi-transparent
                    Color.Gold
                );
                SignBoard DashTutorialSign = new SignBoard(
                    _whiteTexture,
                    "Press SHIFT to Dash pass the gap!",
                    TileMap.GetTileWorldPositionAt(90, 17), // TopLeft Position // TODO : More dynamic
                    160,                    // Width
                    48,                     // Height
                    new Color(10, 10, 40, 220), // Dark blue, semi-transparent
                    Color.Gold
                );
                SignBoard PlatFormJumpTutorialSign = new SignBoard(
                    _whiteTexture,
                    "Jump to get on platform "+     
                    "Press Down to crouch "+
                    "Crouch then Jump to drop below on platform", 
                    TileMap.GetTileWorldPositionAt(123, 16), // TopLeft Position // TODO : More dynamic
                    192,                    // Width
                    96,                     // Height
                    new Color(10, 10, 40, 220), // Dark blue, semi-transparent
                    Color.Gold
                );
                SignBoard ItemTutorialSign = new SignBoard(
                    _whiteTexture,
                    "Preess F to pick up item "+ 
                    "Press (1) (2) to use item",     
                    // TileMap.GetTileWorldPositionAt(10, 30), // TopLeft Position // TODO : More dynamic 
                    TileMap.GetTileWorldPositionAt(149, 30), // TopLeft Position // TODO : More dynamic
                    208,                    // Width 
                    54,                     // Height
                    new Color(10, 10, 40, 220), // Dark blue, semi-transparent
                    Color.Gold
                );
                SignBoard ShootTutorialSign = new SignBoard(
                    _whiteTexture,
                    "Press Q to Punch        "+
                    "Press E to Shoot        "+ 
                    "Hold E to charge bullet ",     
                    // TileMap.GetTileWorldPositionAt(10, 30), // TopLeft Position // TODO : More dynamic 
                    TileMap.GetTileWorldPositionAt(166, 31), // TopLeft Position // TODO : More dynamic
                    192,                    // Width 
                    70,                     // Height
                    new Color(10, 10, 40, 220), // Dark blue, semi-transparent
                    Color.Gold
                );
                SignBoard ItemDropTutorialSign = new SignBoard(
                    _whiteTexture,
                    "Defeat an enemy have chance to spawn an item",
                    // TileMap.GetTileWorldPositionAt(10, 30), // TopLeft Position // TODO : More dynamic 
                    TileMap.GetTileWorldPositionAt(181, 28), // TopLeft Position // TODO : More dynamic
                    144,                    // Width 
                    70,                     // Height
                    new Color(10, 10, 40, 220), // Dark blue, semi-transparent
                    Color.Gold
                );
                SignBoard GoodluckSign = new SignBoard(
                    _whiteTexture,
                    "Ready to escape from this place and go back to delete your history? Jump in the Portal!", 
                    // TileMap.GetTileWorldPositionAt(10, 30), // TopLeft Position // TODO : More dynamic 
                    TileMap.GetTileWorldPositionAt(225, 22), // TopLeft Position // TODO : More dynamic
                    230,                    // Width 
                    70,                     // Height
                    new Color(10, 10, 40, 220), // Dark blue, semi-transparent
                    Color.Gold
                );
                _gameObjects.Add(WalkTutorialSign);
                _gameObjects.Add(JumpTutorialSign);
                _gameObjects.Add(ClimbTutorialSign);
                _gameObjects.Add(DashTutorialSign);
                _gameObjects.Add(PlatFormJumpTutorialSign);
                _gameObjects.Add(ItemTutorialSign);
                _gameObjects.Add(ShootTutorialSign);
                _gameObjects.Add(ItemDropTutorialSign);
                _gameObjects.Add(GoodluckSign);
                break;

            case 2:
                SignBoard GlideTutorialSign = new SignBoard(
                    _whiteTexture,
                    "Holding Jump button while mid air to glide!",
                    TileMap.GetTileWorldPositionAt(23, 100),  // TopLeft Position  // TODO : More dynamic
                    180,                    // Width
                    56,                     // Height
                    new Color(10, 10, 40, 220), // Dark blue, semi-transparent
                    Color.Gold
                );
                _gameObjects.Add(GlideTutorialSign);
                break;

            case 3:
                SignBoard GraplingTutorialSign = new SignBoard(
                    _whiteTexture,
                    "Press R to Grapple onto the hook",
                    TileMap.GetTileWorldPositionAt(64, 66),  // TopLeft Position  // TODO : More dynamic
                    200,                    // Width
                    48,                     // Height
                    new Color(10, 10, 40, 220), // Dark blue, semi-transparent
                    Color.Gold
                );
                _gameObjects.Add(GraplingTutorialSign);
                break;
        }
    }

    private void CreatePlayer()
    {
        Singleton.Instance.Player = new Player(_playerTexture, _whiteTexture, _projectileTexture)
        {
            Name = "Player",
            Life = 3,
            WalkSpeed = 200,
            CrouchSpeed = 100,
            ClimbSpeed = 100,
            MaxHealth = 100,
            MaxMP = 100,

            BaseAttackDamage = 10f,
            AttackDuration = 0.4f, // How long the attack lasts
            AttackCooldown = 0.2f,

            AttackWidth = 24, 
            AttackHeight = Singleton.TILE_SIZE * 2,

            BaseJumpStrength = 600f,

            CoyoteTime = 0.1f, // 100ms of coyote time
            JumpBufferTime = 0.15f, // 150ms jump buffer
            DashSpeed = 400f,
            DashDuration = 0.3f, // Dash lasts for 0.25 seconds
            DashCooldown = 0.2f,
            DashMP = 25f,

            GlideGravityScale = 0.3f, // How much gravity affects gliding (lower = slower fall)
            GlideMaxFallSpeed = 80f, // Maximum fall speed while gliding
            GlideMP = 20f, // MP cost per second while gliding

            GrappleMP = 30f, // MP cost when use Grappling Hook

            MaxChargeTime = 2.0f, // Maximum charge time in seconds
            MinChargePower = 1.0f, // Minimum damage/speed multiplier
            MaxChargePower = 3.0f, // Maximum damage/speed multiplier
            StartChargeMPCost = 5f, // Minimum MP cost per shot
            ChargeMPCost = 15f, // MP cost for fully charged shot

            MPRegenCooldown = 0.6f, // 0.5 seconds before starting MP regenaration
            MPRegenRate = 30f, // MP regenaration per second

            Viewport = ViewportManager.Get("Player"),
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
            Grapple = Keys.R,
            JumpSound = _jumpSound,
            DashSound = _dashSound,
            PunchSound = _punchSound,
            ChargingSound = _chargeBulletSound,
            BulletShotSound = _bulletShotSound,

            Bullet = new PlayerBullet(_projectileTexture)
            {
                Name = "BulletPlayer",
                BaseDamageAmount = 10f,
                Speed = 500f,
                Viewport = ViewportManager.Get("Charge_Bullet_0")
            },
        };
    }

    private void CreateEnemies()
    {
        Dictionary<ItemID, float> defaultLootTableChance = new Dictionary<ItemID, float>{ 
            {ItemID.None, 0.8f},
            {ItemID.HealthPotion, 0.2f},
        };
        Dictionary<ItemID, float> slimeLootTableChance = new Dictionary<ItemID, float>{ 
            {ItemID.None, 0.75f},
            {ItemID.HealthPotion, 0.2f},
            {ItemID.JumpPotion, 0.05f},
        };
        EnemyManager.AddGameEnemy(EnemyID.Slime,
            new SlimeEnemy(_slimeTexture){
                Name = "Slime",
                Viewport = ViewportManager.Get("Slime"),
                MaxHealth = 50f,
                BaseAttackDamage = 3f,

                JumpCooldown = 3.0f,
                BaseJumpStrength = 490,
                Friction = 0.96f,

                HitSound = _hitSound,

                LootTableChance = slimeLootTableChance
            });

        Dictionary<ItemID, float> hellHoundLootTableChance = new Dictionary<ItemID, float>{ 
            {ItemID.None, 0.8f},
            {ItemID.SpeedPotion, 0.2f},
        };
        EnemyManager.AddGameEnemy(EnemyID.Hellhound,
            new HellhoundEnemy(_hellhoundTexture){
                Name = "Hellhound",
                Viewport = ViewportManager.Get("Hellhound"),
                
                MaxHealth = 1f,
                BaseAttackDamage = 8f,

                LimitIdlePatrol = 100,
                
                ChargeTime = 2.0f,
                ChaseDuration = 3.0f,
                DashDuration = 1.0f,

                HitSound = _hitSound,

                LootTableChance = hellHoundLootTableChance
            });

        Dictionary<ItemID, float> SkeletonLootTableChance = new Dictionary<ItemID, float>{ 
            {ItemID.None, 0.75f},
            {ItemID.Gun, 0.05f},
            {ItemID.Grenade, 0.2f},
        };

        EnemyManager.AddGameEnemy(EnemyID.Skeleton,         
            new SkeletonEnemy(_skeletonTexture){
                Name = "Skeleton",
                Viewport = ViewportManager.Get("Skeleton"),

                MaxHealth = 80f,
                BaseAttackDamage = 5f,

                LimitIdlePatrol = 100,

                IgnorePlayerDuration = 3f,

                HitSound = _hitSound,

                LootTableChance = SkeletonLootTableChance
            });

        EnemyManager.AddGameEnemy(EnemyID.PlatformEnemy,
            new PlatformEnemy(_platformEnemyTexture){
                Name = "PlatformEnemy",
                Viewport = ViewportManager.Get("PlatformEnemy"),

                MaxHealth = float.MaxValue,

                HitSound = _hitSound,// Temp

                LootTableChance = defaultLootTableChance 
            });

        Dictionary<ItemID, float> TowerLootTableChance = new Dictionary<ItemID, float>{ 
            {ItemID.None, 0.65f},
            {ItemID.Gun, 0.15f},
            {ItemID.HealthPotion, 0.2f},
        };
        EnemyManager.AddGameEnemy(EnemyID.TowerEnemy,
            new TowerEnemy(_towerTexture){
                Name = "TowerEnemy",
                Viewport = ViewportManager.Get("TowerEnemy"),

                MaxHealth = 150f,

                HitSound = _hitSound,

                TowerBullet = new TowerBullet(_projectileTexture)
                {
                    Name = "BulletEnemy",
                    BaseDamageAmount = 20f,
                    Speed = 300f,
                    Viewport = ViewportManager.Get("TowerEnemy_Bullet")
                },

                LootTableChance = TowerLootTableChance
            });

        Dictionary<ItemID, float> DemonLootTableChance = new Dictionary<ItemID, float>{ 
            {ItemID.None, 0.4f},
            {ItemID.LifeUp, 0.025f},
            {ItemID.Staff, 0.025f},
            {ItemID.JumpPotion, 0.05f},
            {ItemID.HealthPotion, 0.5f},
        };
        EnemyManager.AddGameEnemy(EnemyID.Demon,
            new DemonEnemy(_demonTexture){
                Name = "Demon",
                Viewport = ViewportManager.Get("Demon"),

                MaxHealth = 100f,

                HitSound = _hitSound,

                DemonBullet = new DemonBullet(_projectileTexture)
                {
                    Name = "BulletEnemy",
                    BaseDamageAmount = 15f,
                    Speed = 200f,
                    Viewport = ViewportManager.Get("Demon_Bullet")
                },

                LootTableChance = DemonLootTableChance
            });

        EnemyManager.AddGameEnemy(EnemyID.GiantSlime,         
            new GiantSlime(_giantSlimeTexture, _whiteTexture){
                Name = "GiantSlime",
                Viewport = ViewportManager.Get("GiantSlime"),

                MaxHealth = 1000f,
                BaseAttackDamage = 3f,
                IsIgnorePlatform = true,

                // JumpCooldown = 3.0f,
                BaseJumpStrength = 550,
                Friction = 0.96f,

                HitSound = _hitSound
            });

        EnemyManager.AddGameEnemy(EnemyID.Cerberus,         
            new Cerberus(_cerberusTexture, _whiteTexture){
                Name = "Cerberus",
                Viewport = ViewportManager.Get("Cerberus"),

                MaxHealth = 500,
                BaseAttackDamage = 3f,

                // JumpCooldown = 3.0f,
                BaseJumpStrength = 550,
                Friction = 0.96f,

                HitSound = _hitSound
            }); 

        EnemyManager.AddGameEnemy(EnemyID.Rhulk,         
            new Rhulk(_rhulkTexture){
                Name = "Rhulk",
                Viewport = ViewportManager.Get("Rhulk"),
                MaxHealth = 1000f,
                BaseAttackDamage = 3f,

                LaserSound = _Rhulk_LaserSound,
                DashSound = _Rhulk_DashSound,
                
                // JumpCooldown = 3.0f,
                BaseJumpStrength = 550,
                Friction = 0.96f,

                HitSound = _hitSound,
                
                Laserproj = new DemonLaser(_LaserTexture)
                {
                    Name = "BulletEnemy",
                    BaseDamageAmount = 10f,
                    Viewport = ViewportManager.Get("Rhulk_Laser")
                }
            });

        EnemyManager.AddGameEnemy(EnemyID.Queue,         
            new Queue(_queueTexture){
                Name = "Queue",
                Viewport = ViewportManager.Get("Rhulk"),
            });
    }

    private void CreateItemPrefabs()
    {
        //set for all item
        Item.TooltipBackgroundTexture = _itemSlotTexture;
        Item.PickUpSound = _pickUpSound;

        //TODO : Change these to real ITEM ID
        ItemManager.AddGameItem(ItemID.HealthPotion,
            new Potion(_itemTexture, ItemType.Consumable){
                Name =  "HealthPotion",
                Description = "Test HealthPotion Description",
                Viewport = ViewportManager.Get("Potion_Health"),
                UseSound = _potionUseSound
            });

        ItemManager.AddGameItem(ItemID.SpeedPotion,
            new SpeedPotion(_itemTexture, ItemType.Consumable){
                Name =  "SpeedPotion",
                Description = "Test SpeedPotion Description",
                Viewport = ViewportManager.Get("Potion_Speed"),
                UseSound = _potionUseSound
            });

        ItemManager.AddGameItem(ItemID.JumpPotion,
            new JumpPotion(_itemTexture, ItemType.Consumable){
                Name =  "jumppotion",
                Description = "Test JumpPotion Description",
                Viewport = ViewportManager.Get("Potion_Jump"),
                UseSound = _potionUseSound
            });

        ItemManager.AddGameItem(ItemID.Barrier,
            new Barrier(_itemTexture, ItemType.Consumable){
                Name =  "barrier",
                Description = "Test Barrier Description",
                Viewport = ViewportManager.Get("Barrier"),
                UseSound = _potionUseSound // Temp
            });

        ItemManager.AddGameItem(ItemID.LifeUp,
            new LifeUp(_itemTexture, ItemType.Consumable){
                Name =  "1Up",
                Description = "Test LifeUp Description",
                Viewport = ViewportManager.Get("LifeUp"),
                UseSound = _potionUseSound // Temp
            });

        ItemManager.AddGameItem(ItemID.SpeedBoots,
            new SpeedBoots(_itemTexture, ItemType.Accessory){
                Name =  "SpeedBoots",
                Description = "Test SpeedBoots Description",
                Viewport = ViewportManager.Get("Speed_Boots")
            });

        ItemManager.AddGameItem(ItemID.CursedGauntlet,
            new CursedGauntlet(_itemTexture, ItemType.Accessory){
                Name =  "CursedGauntlet",
                Description = "Test CursedGauntlet Description",
                Viewport = ViewportManager.Get("CursedGauntlet")
            });

        ItemManager.AddGameItem(ItemID.Sword,
            new Sword(_itemTexture, ItemType.MeleeWeapon){
                Name =  "Sword",
                Description = "Test Sword Description",
                Viewport = ViewportManager.Get("Sword"),
                SlashSound = _swordSlashSound,
            });

        ItemManager.AddGameItem(ItemID.Gun,
            new Gun(_itemTexture, ItemType.RangeWeapon){
                Name =  "Gun",
                Description = "Test Gun Description",
                Viewport = ViewportManager.Get("Gun"),
                ShootSound = _gunshotSound,
            });

        ItemManager.AddGameItem(ItemID.Staff,
            new Staff(_itemTexture, ItemType.RangeWeapon){
                Name =  "Staff",
                Description = "Test Staff Description",
                MPCost = 10,
                ShootSound = _fireBallShootingSound,

                FireBall = new FireBall(_projectileTexture)
                {
                    Name = "FireBall",
                    BaseDamageAmount = 30f,
                    Speed = 500f,
                    Radius = 30f,
                    ExplosionDuration = 0.5f,
                    Viewport = ViewportManager.Get("FireBall"),
                    BaseExplosion = new Explosion(_projectileTexture, _fireBallExplosionSound)
                    {
                        Viewport = ViewportManager.Get("Explosion")
                    }
                },
                Viewport = ViewportManager.Get("Fire_Staff")
            });

        ItemManager.AddGameItem(ItemID.SoulStaff,
            new SoulStaff(_itemTexture,ItemType.Consumable){
                Name =  "Staff",
                Description = "Summon Your best Minion!",
                MPCost = 10,
                soulMinion = new SoulMinion(_projectileTexture)
                {
                    Name = "Soul Minion",
                    BaseDamageAmount = 0f,
                    Viewport = ViewportManager.Get("Soul_Minion"),
                    soulBullet = new SoulBullet(_projectileTexture){
                        Name = "Soul Bullet",
                        BaseDamageAmount = 15f,
                        Speed = 150f,
                        Viewport = ViewportManager.Get("Soul_Bullet")
                    }
                },
                Viewport = ViewportManager.Get("Soul_Staff")
            });

        ItemManager.AddGameItem(ItemID.Grenade,
            new Grenade(_itemTexture, ItemType.Consumable){
                Name =  "Grenade",
                Description = "Test GrenadeTemp Description",
                Viewport = ViewportManager.Get("Grenade"),
                UseSound = _potionUseSound, // Temp

                GrenadeProjectile = new GrenadeProjectile(_projectileTexture)
                {
                    // Grenade properties
                    Name = "GrenadeProjectile",
                    BaseDamageAmount = 30f,
                    Speed = 450f,
                    Radius = 50f,
                    ExplosionDuration = 0.5f,
                    DetonateDelayDuration = 3.0f,
                    Viewport = ViewportManager.Get("Grenade_Projectile"),
                    BaseExplosion = new Explosion(_projectileTexture, _fireBallExplosionSound)
                    {
                        Viewport = ViewportManager.Get("Explosion")
                    }
                }
            });
    }

    private void SpawnEnemies()
    {
        EnemyManager.SpawnWorldEnemy(_stageManager, _gameObjects);
    }

    private void SpawnItems()
    {
        _ui.ClearWorldSpaceUI();
        foreach (var itemSpawnPoint in _stageManager.GetItemSpawnPoints())
        {
            ItemManager.SpawnItem(itemSpawnPoint.Value, TileMap.GetTileWorldPositionAt(itemSpawnPoint.Key), _gameObjects);
        }
    }

    protected override void SetupHUD()
    {
        _ui.ClearHUD();

        // Top Left - Health and MP
        TextUI HealthText = new TextUI(            
            new Rectangle(20, 15, 200, 25),
            () => $"HP ({Singleton.Instance.Player.Health:F0} / {Singleton.Instance.Player.MaxHealth:F0})",
            Color.White,
            TextUI.TextAlignment.Left
        );
        
        HealthBar playerHealth = new HealthBar(
            Singleton.Instance.Player,
            new Rectangle(20, 40, 3 * (int) Singleton.Instance.Player.MaxHealth, 25),
            Color.Red,
            Color.Gray
        );

        TextUI MPText = new TextUI(            
            new Rectangle(20, 70, 200, 25),
            () => $"MP ({Singleton.Instance.Player.MP:F0} / {Singleton.Instance.Player.MaxMP:F0})",
            Color.White,
            TextUI.TextAlignment.Left
        );
        
        MPBar playerMP = new MPBar(
            new Rectangle(20, 95, 200, 25),
            Color.SkyBlue,
            Color.Gray
        );

        // Top Right - Lives
        TextUI LifeText = new TextUI(            
            new Rectangle(1220, 25, 60, 25),
            () => $"x{Singleton.Instance.Player.Life}",
            Color.White,
            TextUI.TextAlignment.Center
        );
        ImageUI LifeImage = new ImageUI(
            _playerTexture,
                new Rectangle(1170, 15, 50, 50),
                ViewportManager.Get("Player_Head")// viewport manager
            );

        // Bottom Section - Equipment (moved closer to bottom of screen)
        int slotY = 640; // Increased base Y position for slots (was 600)
        
        // Melee weapon section
        TextUI MeleeWeaponText = new TextUI(            
            new Rectangle(490, slotY - 25, 50, 20),
            "Melee",
            Color.White,
            TextUI.TextAlignment.Center
        );
        
        ItemSlot MeleeWeaponSlot = new ItemSlot(
            Inventory.MELEE_SLOT,
            new Rectangle(490, slotY, 50, 50),
            _itemSlotTexture
        );
        
        TextUI MeleeWeaponButtonText = new TextUI(            
            new Rectangle(490, slotY + 55, 50, 20),
            "Q",
            Color.White,
            TextUI.TextAlignment.Center
        );

        // Ranged weapon section
        TextUI RangeWeaponText = new TextUI(            
            new Rectangle(550, slotY - 25, 50, 20),
            "Range",
            Color.White,
            TextUI.TextAlignment.Center
        );
        
        ItemSlot RangeWeaponSlot = new ItemSlot(
            Inventory.RANGE_SLOT,
            new Rectangle(550, slotY, 50, 50),
            _itemSlotTexture
        );
        
        TextUI RangeWeaponButtonText = new TextUI(            
            new Rectangle(550, slotY + 55, 50, 20),
            "E",
            Color.White,
            TextUI.TextAlignment.Center
        );

        // Item slots section - with shared "Items" label centered over both
        TextUI ItemsLabelText = new TextUI(            
            new Rectangle(640, slotY - 25, 50, 20),
            "Items",
            Color.White,
            TextUI.TextAlignment.Center
        );
        
        ItemSlot ItemSlot1 = new ItemSlot(
            Inventory.ITEM_SLOT_1,
            new Rectangle(610, slotY, 50, 50),
            _itemSlotTexture
        );
        
        TextUI ItemButtonText1 = new TextUI(            
            new Rectangle(610, slotY + 55, 50, 20),
            "1",
            Color.White,
            TextUI.TextAlignment.Center
        );
        
        ItemSlot ItemSlot2 = new ItemSlot(
            Inventory.ITEM_SLOT_2,
            new Rectangle(670, slotY, 50, 50),
            _itemSlotTexture
        );
        
        TextUI ItemButtonText2 = new TextUI(            
            new Rectangle(670, slotY + 55, 50, 20),
            "2",
            Color.White,
            TextUI.TextAlignment.Center
        );

        // Add elements to UI
        _ui.AddHUDElement(HealthText);
        _ui.AddHUDElement(playerHealth);
        _ui.AddHUDElement(MPText);
        _ui.AddHUDElement(playerMP);
        _ui.AddHUDElement(LifeText);
        _ui.AddHUDElement(LifeImage);
        
        // Melee weapon section
        _ui.AddHUDElement(MeleeWeaponText);
        _ui.AddHUDElement(MeleeWeaponSlot);
        _ui.AddHUDElement(MeleeWeaponButtonText);
        
        // Ranged weapon section
        _ui.AddHUDElement(RangeWeaponText);
        _ui.AddHUDElement(RangeWeaponSlot);
        _ui.AddHUDElement(RangeWeaponButtonText);
        
        // Item slots section
        _ui.AddHUDElement(ItemsLabelText);
        _ui.AddHUDElement(ItemSlot1);
        _ui.AddHUDElement(ItemButtonText1);
        _ui.AddHUDElement(ItemSlot2);
        _ui.AddHUDElement(ItemButtonText2);
    }

    public void UnlockAbilityForStage()
    {
        if(Singleton.Instance.Stage >= 0)
        {
            Singleton.Instance.Player.Abilities.UnlockAbility(AbilityType.Dash);
        }
        if(Singleton.Instance.Stage == 0 || Singleton.Instance.Stage >= 2)
        {
            Singleton.Instance.Player.Abilities.UnlockAbility(AbilityType.Glide);
        }
        if(Singleton.Instance.Stage >= 3)
        {
            Singleton.Instance.Player.Abilities.UnlockAbility(AbilityType.Grapple);
        }
    }
}
