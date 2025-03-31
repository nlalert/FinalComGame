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
    List<GameObject> _gameObjects;
    int _numObject;

    private Texture2D _playerTexture;
    private Texture2D _textureAtlas;
    private Texture2D _DemonTexture;
    private Texture2D _DemonBulletTexture;
    private Texture2D _TowerTexture;
    private Texture2D _PlatformTexture;
    private Texture2D _GiantSlimeTexture;
    private Texture2D _CerberusTexture;
    private Texture2D _RhulkTexture;
    private Texture2D _LaserTexture;
    private Texture2D _SoulMinion;
    private Texture2D _MinionSoulBullet;

    private Texture2D _parallaxFGtexture;
    private Texture2D _parallaxMGtexture;
    private Texture2D _parallaxBGtexture;
    private ParallaxBackground _parallaxBackground;

    private TileMap _collisionTileMap;
    private TileMap _FGTileMap;
    private TileMap _MGTileMap;
    private TileMap _BGTileMap;

    private List<AmbushArea> ambushAreas;

    private Dictionary<int, BaseEnemy> _enemyPrefabs;
    public override void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager, ContentManager content)
    {
        base.Initialize(graphicsDevice, graphicsDeviceManager, content);

        _graphics.PreferredBackBufferWidth = Singleton.SCREEN_WIDTH;
        _graphics.PreferredBackBufferHeight = Singleton.SCREEN_HEIGHT;
        _graphics.ApplyChanges();

        _gameObjects = new List<GameObject>();
    }

    public override void LoadContent(SpriteBatch spriteBatch)
    {
        base.LoadContent(spriteBatch);
        
        _playerTexture = _content.Load<Texture2D>("Char_test");
        // _enemyTexture = _content.Load<Texture2D>("EnemyRed");
        // _DogTexture = _content.Load<Texture2D>("EnemyDog");
        // _SlimeTexture = _content.Load<Texture2D>("HellSlime");
        _DemonTexture = _content.Load<Texture2D>("EnemyDemon");
        _DemonBulletTexture = _content.Load<Texture2D>("EnemyDemon");
        _TowerTexture = _content.Load<Texture2D>("EnemyTower");
        _PlatformTexture = _content.Load<Texture2D>("EnemyPlatform");
        _GiantSlimeTexture = _content.Load<Texture2D>("GiantSlime");
        _CerberusTexture = _content.Load<Texture2D>("Cerberus");
        _RhulkTexture = _content.Load<Texture2D>("EnemyRhulk");
        _LaserTexture = _content.Load<Texture2D>("Laserbeam");
        _SoulMinion = _content.Load<Texture2D>("SoulMinion");
        _MinionSoulBullet = _content.Load<Texture2D>("MinionSoulBullet");


        _textureAtlas = _content.Load<Texture2D>("Tileset");
        _parallaxFGtexture = _content.Load<Texture2D>("Level_1_Parallax_fg");
        _parallaxMGtexture = _content.Load<Texture2D>("Level_1_Parallax_mg");
        _parallaxBGtexture = _content.Load<Texture2D>("Level_1_Parallax_bg");

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
                UpdateAmbushAreas(gameTime);
                RemoveInactiveObjects();

                Singleton.Instance.Camera.Follow(Singleton.Instance.Player); // Make camera follow the player
                break;
            case Singleton.GameState.StageCompleted:
                Singleton.Instance.Stage++;
                if (Singleton.Instance.Stage >= 4){
                    Singleton.Instance.CurrentGameState = Singleton.GameState.GameWon;
                }
                else
                {
                    Singleton.Instance.CurrentGameState = Singleton.GameState.InitializingStage;
                }
                break;
            case Singleton.GameState.GameOver:
                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Stop();
                }
                break;
        }

        //Console.WriteLine("GameObject :" + _numObject);

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        _numObject = _gameObjects.Count;

        switch (Singleton.Instance.CurrentGameState)
        {
            case Singleton.GameState.Playing:
                // Draw the Game World (Apply Camera)

                _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Singleton.Instance.Camera.GetTransformation()); // Apply camera matrix
                // _parallaxBackground.Draw(_spriteBatch);
                DrawTileMap();
                DrawAllObjects();
                _spriteBatch.End();

                _spriteBatch.Begin(samplerState: SamplerState.PointClamp); 
                _spriteBatch.DrawString(Singleton.Instance.GameFont, "Health Bar : " + Singleton.Instance.Player.Health + " / " + Singleton.Instance.Player.MaxHealth, new Vector2(10, 10), Color.White);
                _spriteBatch.DrawString(Singleton.Instance.GameFont, "MP Bar : " + Singleton.Instance.Player.MP + " / " + Singleton.Instance.Player.MaxMP, new Vector2(10, 70), Color.White);
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
        // Console.WriteLine(_gameObjects.Count);
    }

    private void UpdateAmbushAreas(GameTime gameTime)
    {
        foreach (var ambushArea in ambushAreas)
        {
            ambushArea.Update(gameTime, _gameObjects, _collisionTileMap);
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
        if (Singleton.Instance.Stage == 1){
        _BGTileMap.Draw(_spriteBatch);
        _MGTileMap.Draw(_spriteBatch);
        _FGTileMap.Draw(_spriteBatch);
        }
        
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
        _gameObjects = new List<GameObject>();

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
        if (Singleton.Instance.Stage == 1)
        {
            _BGTileMap = new TileMap(_textureAtlas, "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_BackGround.csv", 20);
            _MGTileMap = new TileMap(_textureAtlas, "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_MidGround.csv", 20);
            _FGTileMap = new TileMap(_textureAtlas, "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_ForeGround.csv", 20);
        }
        _collisionTileMap = new TileMap(_textureAtlas, StageManager.GetCurrentStageCollisionPath(), 20);

        Rectangle mapBounds = new Rectangle(0, 0,  _collisionTileMap.MapWidth * Singleton.TILE_SIZE,  _collisionTileMap.MapHeight * Singleton.TILE_SIZE); // Map size
        Singleton.Instance.Camera = new Camera(_graphicsDevice.Viewport, mapBounds); // Initialize camera

        _parallaxBackground = new ParallaxBackground(_parallaxFGtexture, _parallaxMGtexture, _parallaxBGtexture, StageManager.GetPlayerWorldSpawnPosition());

        Singleton.Instance.Player.Position = StageManager.GetPlayerWorldSpawnPosition(); // get player location of each stage
        _gameObjects.Add(Singleton.Instance.Player);

        InitializeAmbushAreas();
        // SpawnEnemies();
        AddItems();
        SetupUI();
        
        foreach (GameObject s in _gameObjects)
        {
            s.Reset();
        }
    }
    
    // In your PlayScene or main game class
    public void InitializeAmbushAreas()
    {
        ambushAreas = _collisionTileMap.GetAmbushAreas(_enemyPrefabs);
    }

    private void CreatePlayer()
    {
        // Load sprite sheets
        Texture2D playerTexture = _content.Load<Texture2D>("Char");
        Texture2D playerParticle = new Texture2D(_graphicsDevice, 1, 1);
        Texture2D projectileTexture = _content.Load<Texture2D>("Projectile");
    
        SoundEffect playerJumpSound = _content.Load<SoundEffect>("GoofyAhhJump");
        SoundEffect playerDashSound = _content.Load<SoundEffect>("Dash");
        SoundEffect playerPunchSound = _content.Load<SoundEffect>("PlayerPunch");
        
        Singleton.Instance.Player = new Player(playerTexture, playerParticle)
        {
            Name = "Player",
            Position = StageManager.GetPlayerWorldSpawnPosition(),// get player location of each stage
            Life = 2,
            WalkSpeed = 200,
            CrouchSpeed = 100,
            ClimbSpeed = 100,
            MaxHealth = 100,
            MaxMP = 100,

            BaseAttackDamage = 10f,
            AttackDuration = 0.4f, // How long the attack lasts
            AttackCooldown = 0.2f,

            AttackWidth = 24, 
            AttackHeight = 32,

            BaseJumpStrength = 600f,

            CoyoteTime = 0.1f, // 100ms of coyote time
            JumpBufferTime = 0.15f, // 150ms jump buffer
            DashSpeed = 400f,
            DashDuration = 0.3f, // Dash lasts for 0.25 seconds
            DashCooldown = 0.2f,
            DashMP = 20f,

            GlideGravityScale = 0.3f, // How much gravity affects gliding (lower = slower fall)
            GlideMaxFallSpeed = 80f, // Maximum fall speed while gliding
            GlideMP = 20f, // MP cost per second while gliding

            MaxChargeTime = 2.0f, // Maximum charge time in seconds
            MinChargePower = 1.0f, // Minimum damage/speed multiplier
            MaxChargePower = 3.0f, // Maximum damage/speed multiplier
            StartChargeMPCost = 5f, // Minimum MP cost per shot
            ChargeMPCost = 15f, // MP cost for fully charged shot

            MPRegenCooldown = 0.6f, // 0.5 seconds before starting MP regenaration
            MPRegenRate = 30f, // MP regenaration per second

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
            DashSound = playerDashSound,
            PunchSound = playerPunchSound,

            Bullet = new PlayerBullet(projectileTexture)
            {
                Name = "BulletPlayer",
                BaseDamageAmount = 15f,
                Speed = 500f,
                Viewport = new Rectangle(0, 0, 9, 5)
            },
        };
    }

    private void CreateEnemies()
    {
        Texture2D _enemyTexture = _content.Load<Texture2D>("Skeleton");
        Texture2D _DogTexture = _content.Load<Texture2D>("EnemyDog");
        Texture2D _SlimeTexture = _content.Load<Texture2D>("HellSlime");
        Texture2D _DemonTexture = _content.Load<Texture2D>("EnemyDemon");
        Texture2D _DemonBulletTexture = _content.Load<Texture2D>("EnemyDemon");
        Texture2D _TowerTexture = _content.Load<Texture2D>("Spitter");
        Texture2D _PlatformTexture = _content.Load<Texture2D>("EnemyPlatform");
        Texture2D _GiantSlimeTexture = _content.Load<Texture2D>("GiantSlime");
        Texture2D _CerberusTexture = _content.Load<Texture2D>("Cerberus");

        Texture2D projectileTexture = _content.Load<Texture2D>("Projectile");

        // Create a dictionary of enemy prefabs
        _enemyPrefabs = new Dictionary<int, BaseEnemy>
        {
            {
                97,         
                new SlimeEnemy(_SlimeTexture, new Texture2D(_graphicsDevice, 1, 1)){
                    Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
                    Viewport = new Rectangle(0, 0, 16, 16),
                    MaxHealth = 50f,
                    BaseAttackDamage = 3f,

                    JumpCooldown = 3.0f,
                    BaseJumpStrength = 550,
                    Friction = 0.96f
                }
            },

            {
                98,
                new HellhoundEnemy(_DogTexture){
                    Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
                    Viewport = new Rectangle(0, 0, 64, 32),
                    
                    MaxHealth = 1f,
                    BaseAttackDamage = 8f,

                    LimitIdlePatrol = 100,
                    
                    ChargeTime = 2.0f,
                    ChaseDuration = 5f,
                    DashDuration = 1.0f,
                }
            },

            {
                99,         
                new SkeletonEnemy(_enemyTexture){
                    Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
                    Viewport = new Rectangle(0, 0, 16, 32),

                    MaxHealth = 80f,
                    BaseAttackDamage = 5f,

                    LimitIdlePatrol = 100,

                    IgnorePlayerDuration = 3f,
                }
            },
            {
                117,
                new PlatformEnemy(_PlatformTexture){
                    Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
                    Viewport = new Rectangle(0, 0, 64, 32),

                    MaxHealth = float.MaxValue,
                }
            },
            {
                118,
                new TowerEnemy(_TowerTexture){
                    Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
                    Viewport = new Rectangle(0, 0, 16, 16),

                    MaxHealth = 150f,

                    TowerBullet = new TowerBullet(projectileTexture)
                    {
                        Name = "BulletEnemy",
                        BaseDamageAmount = 20f,
                        Speed = 300f,
                        Viewport = new Rectangle(0, 0, 12, 12),
                        spriteViewport = new Rectangle(16, 16, 16, 16)
                    }
                }
            },
            {
                119,
                new DemonEnemy(_DemonTexture){
                    Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
                    Viewport = new Rectangle(0, 0, 32, 64),

                    MaxHealth = 100f,

                    DemonBullet = new DemonBullet(_DemonBulletTexture)
                    {
                        Name = "BulletEnemy",
                        BaseDamageAmount = 15f,
                        Speed = 150f,
                        Viewport = new Rectangle(0, 0, 32, 32)
                    }
                }
            },
            {
                137,         
                new GiantSlime(_GiantSlimeTexture, new Texture2D(_graphicsDevice, 1, 1)){
                    Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
                    Viewport = new Rectangle(0, 0, 64, 48),

                    MaxHealth = 1f,
                    BaseAttackDamage = 3f,

                    // JumpCooldown = 3.0f,
                    BaseJumpStrength = 550,
                    Friction = 0.96f
                }
            },
            //DONOT REMOVE This. just add new number please cuz Feen's dont know where and what to assign this number
            // {
            //     97,         
            //     new GiantSlime(_GiantSlimeTexture, new Texture2D(_graphicsDevice, 1, 1)){
            //         Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
            //         Viewport = new Rectangle(0, 0, 64, 48),

            //         MaxHealth = 1f,
            //         BaseAttackDamage = 3f,

            //         // JumpCooldown = 3.0f,
            //         BaseJumpStrength = 550,
            //         Friction = 0.96f
            //     }
            // },
            {
                138,         
                new Cerberus(_CerberusTexture, new Texture2D(_graphicsDevice, 1, 1)){
                    Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
                    Viewport = new Rectangle(0, 0, 64, 48),

                    MaxHealth = 100f,
                    BaseAttackDamage = 3f,

                    // JumpCooldown = 3.0f,
                    BaseJumpStrength = 550,
                    Friction = 0.96f
                }
            },
            {
                199,         
                new Rhulk(_RhulkTexture){
                    Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
                    Viewport = new Rectangle(0, 0, 22, 64),

                    MaxHealth = 100f,
                    BaseAttackDamage = 3f,

                    // JumpCooldown = 3.0f,
                    BaseJumpStrength = 550,
                    Friction = 0.96f,
                    Laserproj = new DemonLaser(_LaserTexture)
                    {
                        Name = "BulletEnemy",
                        BaseDamageAmount = 20f,
                        Viewport = new Rectangle(0, 0, 10, 200),
                    },
                }
            },
        };

    }

    private void SpawnEnemies()
    {
        foreach (var enemySpawnPoint in _collisionTileMap.GetEnemySpawnPoints())
        {
            bool isEnemyPositionInAmbushArea = false;
            foreach (AmbushArea ambushArea in ambushAreas)
            {
                if(ambushArea.IsEnemyPositionInAmbushArea(enemySpawnPoint.Key))
                {
                    isEnemyPositionInAmbushArea = true;
                    break;
                }
            }
            if(!isEnemyPositionInAmbushArea)
                _enemyPrefabs[enemySpawnPoint.Value].Spawn(TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key), _gameObjects);
            
        }
    }

    private void AddItems()
    {
        Texture2D testItem = _content.Load<Texture2D>("Pickaxe");
        Texture2D sword = _content.Load<Texture2D>("Sword");
        Texture2D HealthPotionTemp = _content.Load<Texture2D>("HealthPotionTemp");
        Texture2D Hermes_Boots = _content.Load<Texture2D>("Hermes_Boots");
        Texture2D LifeUP = _content.Load<Texture2D>("1Up");
        Texture2D Gun = _content.Load<Texture2D>("Items");
        Texture2D Staff = _content.Load<Texture2D>("Staff");
        Texture2D Bunny = _content.Load<Texture2D>("Bunny");
        Texture2D Gauntlet = _content.Load<Texture2D>("Gauntlet");
        Texture2D FireBall = _content.Load<Texture2D>("Projectile");
        Texture2D ExplosionEffect = _content.Load<Texture2D>("Explosion");
        Texture2D SoulStaff = _content.Load<Texture2D>("SoulStaff");
        

        //set for all item
        Item.TooltipBackgroundTexture = _content.Load<Texture2D>("ItemSlot");
        Item.PickUpSound = _content.Load<SoundEffect>("PickUp");

        _gameObjects.Add(new Barrier(testItem, ItemType.Consumable, TileMap.GetTileWorldPositionAt(20, 90)){
            Name =  "barrier",
            Description = "Test Barrier Description",
            Viewport = new Rectangle(0, 0, 32,32)
        });

        _gameObjects.Add(new SpeedBoots(Hermes_Boots, ItemType.Accessory, TileMap.GetTileWorldPositionAt(24, 90)){
            Name =  "HealthPotion",
            Description = "Test SpeedBoots Description",
            Viewport = new Rectangle(0, 0, 32,32)
        });

        _gameObjects.Add(new CursedGauntlet(Gauntlet, ItemType.Accessory, TileMap.GetTileWorldPositionAt(26, 80)){
            Name =  "CursedGauntlet",
            Description = "Test CursedGauntlet Description",
            Viewport = new Rectangle(0, 0, 32,32)
        });
        
        _gameObjects.Add(new Potion(HealthPotionTemp, ItemType.Consumable, TileMap.GetTileWorldPositionAt(12, 90)){
            Name =  "HealthPotion",
            Description = "Test HealthPotion Description",
            Viewport = new Rectangle(0, 0, 32,32)
        });

        _gameObjects.Add(new SpeedPotion(HealthPotionTemp, ItemType.Consumable, TileMap.GetTileWorldPositionAt(31, 90)){
            Name =  "SpeedPotion",
            Description = "Test SpeedPotion Description",
            Viewport = new Rectangle(0, 0, 32,32)
        });
        
        _gameObjects.Add(new LifeUp(LifeUP, ItemType.Consumable, TileMap.GetTileWorldPositionAt(16, 90)){
            Name =  "1Up",
            Description = "Test LifeUp Description",
            Viewport = new Rectangle(0, 0, 32,32)
        });

        SoundEffect SwordSlashSound = _content.Load<SoundEffect>("SwordSlash");
        _gameObjects.Add(new Sword(sword, ItemType.MeleeWeapon, TileMap.GetTileWorldPositionAt(4, 90)){
            Name =  "Sword",
            Description = "Test Sword Description",
            Viewport = new Rectangle(0, 0, 32,32),
            SlashSound = SwordSlashSound,
        });

        _gameObjects.Add(new Gun(Gun, ItemType.RangeWeapon, TileMap.GetTileWorldPositionAt(8, 90)){
            Name =  "Gun",
            Description = "Test Gun Description",
            Viewport = new Rectangle(0, 0, 32,32)
        });


        SoundEffect FireBallShootingSound = _content.Load<SoundEffect>("FireBallShooting");
        SoundEffect FireBallExplosionSound = _content.Load<SoundEffect>("FireBallExplosion");
        _gameObjects.Add(new Staff(Staff, ItemType.RangeWeapon, TileMap.GetTileWorldPositionAt(40, 90)){
            Name =  "Staff",
            Description = "Test Staff Description",
            MPCost = 10,
            FireBallShootingSound = FireBallShootingSound,

            FireBall = new FireBall(FireBall, ExplosionEffect, FireBallExplosionSound)
            {
                Name = "FireBall",
                BaseDamageAmount = 30f,
                Speed = 500f,
                Radius = 60f,
                ExplosionDuration = 0.5f,
                Viewport = new Rectangle(0, 0, 8, 8)
            },
            Viewport = new Rectangle(0, 0, 32,32),
        });

        _gameObjects.Add(new JumpPotion(Bunny, ItemType.Consumable, TileMap.GetTileWorldPositionAt(35, 90)){
            Name =  "jumppotion",
            Description = "Test JumpPotion Description",
            Viewport = new Rectangle(0, 0, 32,32)
        });


        _gameObjects.Add(new SoulStaff(SoulStaff,ItemType.Consumable, TileMap.GetTileWorldPositionAt(16, 90)){
            Name =  "Staff",
            Description = "Summon Your best Minion!",
            MPCost = 10,
            soulMinion = new SoulMinion(_SoulMinion)
            {
                Name = "Soul Minion",
                BaseDamageAmount = 0f,
                Viewport = new Rectangle(0, 0, 12, 12),
                soulBullet = new SoulBullet(_MinionSoulBullet){
                    Name = "Soul Bullet",
                    BaseDamageAmount = 15f,
                    Speed = 150f,
                    Viewport = new Rectangle(0, 0, 10, 10),
                }
            },
            Viewport = new Rectangle(0, 0, 32,32),
        });
    }

    protected override void SetupUI()
    {
        HealthBar playerHealth = new HealthBar(
            Singleton.Instance.Player,
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

        ItemSlot MeleeWeaponSlot = new ItemSlot(
            0, // first slot
            new Rectangle(250, 30, 50, 50),
            slot,
            slot
        );

        ItemSlot RangeWeaponSlot = new ItemSlot(
            1, // first slot
            new Rectangle(350, 30, 50, 50),
            slot,
            slot
        );
        ItemSlot ItemSlot1 = new ItemSlot(
            2, // first slot
            new Rectangle(550, 30, 50, 50),
            slot,
            slot
        );
        ItemSlot ItemSlot2 = new ItemSlot(
            3, //second slot
            new Rectangle(650, 30, 50, 50),
            slot,
            slot
        );

        _ui.AddHUDElement(playerHealth);
        _ui.AddHUDElement(playerMP);
        _ui.AddHUDElement(MeleeWeaponSlot);
        _ui.AddHUDElement(RangeWeaponSlot);
        _ui.AddHUDElement(ItemSlot1);
        _ui.AddHUDElement(ItemSlot2);
    }
}
