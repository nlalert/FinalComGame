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
    private Texture2D _HookHeadTexture;
    private Texture2D _RopeTexture; 

    private ParallaxBackground _parallaxBackground;
    private Texture2D _backgroundLayer1;
    private Texture2D _backgroundLayer2;
    private Texture2D _backgroundLayer3;

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
        _HookHeadTexture = _content.Load<Texture2D>("HookHead");
        _RopeTexture = _content.Load<Texture2D>("Rope");


        _textureAtlas = _content.Load<Texture2D>("Tileset");

        _song = _content.Load<Song>("ChillSong");
    }

    public override void Update(GameTime gameTime)
    {
        //Update
        _numObject = _gameObjects.Count;
        if(Singleton.Instance.IsKeyPressed(Keys.Tab))
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
                ResumeSong();
                PlaySong();
                if(Singleton.Instance.IsKeyJustPressed(Keys.Escape))
                {
                    Singleton.Instance.CurrentGameState = Singleton.GameState.Pause;
                }
                UpdateTileMap(gameTime);
                UpdateAllObjects(gameTime);
                UpdateAmbushAreas(gameTime);
                RemoveInactiveObjects();

                Singleton.Instance.Camera.Follow(Singleton.Instance.Player); // Make camera follow the player
                _parallaxBackground.Update(gameTime);
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
            case Singleton.GameState.Pause:
                // Draw background layers (no camera transform for parallax background)
                _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                _parallaxBackground.Draw(_spriteBatch);
                _spriteBatch.End();

                // Draw the Game World (Apply Camera)
                _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Singleton.Instance.Camera.GetTransformation()); // Apply camera matrix
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
        if (Singleton.Instance.Stage == 1){//remove later
            _BGTileMap.Draw(_spriteBatch);
            _MGTileMap.Draw(_spriteBatch);
            _FGTileMap.Draw(_spriteBatch);
        }
        
        //Should be hidden
        //_collisionTileMap.Draw(_spriteBatch);
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

        CreatePlayer();
        CreateEnemies();

        foreach (GameObject s in _gameObjects)
        {
            s.Reset();
        }
    }

    protected void ResetStage()
    {
        _gameObjects.Clear();

        Singleton.Instance.Random = new Random();
        if (Singleton.Instance.Stage == 1)//remove later
        {
            _BGTileMap = new TileMap(_textureAtlas, "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_BackGround.csv", 20);
            _MGTileMap = new TileMap(_textureAtlas, "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_MidGround.csv", 20);
            _FGTileMap = new TileMap(_textureAtlas, "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_ForeGround.csv", 20);
        }
        _collisionTileMap = new TileMap(_textureAtlas, StageManager.GetCurrentStageCollisionPath(), 20);


        Rectangle mapBounds = new Rectangle(0, 0,  _collisionTileMap.MapWidth * Singleton.TILE_SIZE,  _collisionTileMap.MapHeight * Singleton.TILE_SIZE); // Map size
        Singleton.Instance.Camera = new Camera(_graphicsDevice.Viewport, mapBounds); // Initialize camera

        Singleton.Instance.Player.Position = StageManager.GetPlayerWorldSpawnPosition(); // get player location of each stage
        _gameObjects.Add(Singleton.Instance.Player);

        SetUpParallaxBackground();
        InitializeAmbushAreas();
        // SpawnEnemies();
        AddItems();
        SetupHUD();
        
        foreach (GameObject s in _gameObjects)
        {
            s.Reset();
        }
    }

    private void SetUpParallaxBackground()
    {
        // Load background textures
        _backgroundLayer1 = _content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_bg");  // Farthest layer
        _backgroundLayer2 = _content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_mg");  // Middle layer
        _backgroundLayer3 = _content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_fg");  // Closest layer

        // Create parallax background
        _parallaxBackground = new ParallaxBackground(_graphicsDevice.Viewport);

        _parallaxBackground.AddLayer(_backgroundLayer1, 0.0f, 1.0f, Vector2.Zero); // Sky/clouds move very slowly
        _parallaxBackground.AddLayer(_backgroundLayer2, 0.1f, 1.5f, new Vector2(-50,-300)); // Mountains move at medium speed
        _parallaxBackground.AddLayer(_backgroundLayer3, 0.2f, 2.0f, new Vector2(-100,-800)); // Trees move faster (closer to player)
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
        SoundEffect playerChargeBulletSound = _content.Load<SoundEffect>("ChargingBullet");
        SoundEffect playerBulletShotSound = _content.Load<SoundEffect>("BulletShot");
        
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
            Grapple = Keys.R,
            JumpSound = playerJumpSound,
            DashSound = playerDashSound,
            PunchSound = playerPunchSound,
            ChargingSound = playerChargeBulletSound,
            BulletShotSound = playerBulletShotSound,

            Bullet = new PlayerBullet(projectileTexture)
            {
                Name = "BulletPlayer",
                BaseDamageAmount = 15f,
                Speed = 500f,
                Viewport = new Rectangle(1, 5, 12, 5)
            },
            _hookHeadTexture = _HookHeadTexture,
            _ropeTexture = _RopeTexture
        };
    }

    private void CreateEnemies()
    {
        Texture2D _enemyTexture = _content.Load<Texture2D>("Skeleton");
        Texture2D _DogTexture = _content.Load<Texture2D>("HellHound");
        Texture2D _SlimeTexture = _content.Load<Texture2D>("HellSlime");
        Texture2D _DemonTexture = _content.Load<Texture2D>("Demon");
        Texture2D _TowerTexture = _content.Load<Texture2D>("Spitter");
        Texture2D _PlatformTexture = _content.Load<Texture2D>("Crab");
        Texture2D _GiantSlimeTexture = _content.Load<Texture2D>("GiantSlime");
        Texture2D _CerberusTexture = _content.Load<Texture2D>("Cerberus");

        Texture2D projectileTexture = _content.Load<Texture2D>("Projectile");

        SoundEffect hitSound = _content.Load<SoundEffect>("HitEnemy");

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
                    BaseJumpStrength = 490,
                    Friction = 0.96f,

                    HitSound = hitSound
                }
            },

            {
                98,
                new HellhoundEnemy(_DogTexture){
                    Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
                    Viewport = new Rectangle(0, 0, 32, 32),
                    
                    MaxHealth = 1f,
                    BaseAttackDamage = 8f,

                    LimitIdlePatrol = 100,
                    
                    ChargeTime = 2.0f,
                    ChaseDuration = 5f,
                    DashDuration = 1.0f,

                    HitSound = hitSound
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

                    HitSound = hitSound
                }
            },
            {
                117,
                new PlatformEnemy(_PlatformTexture){
                    Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
                    Viewport = new Rectangle(0, 0, 48, 32),

                    MaxHealth = float.MaxValue,

                    HitSound = hitSound // Temp
                }
            },
            {
                118,
                new TowerEnemy(_TowerTexture){
                    Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
                    Viewport = new Rectangle(0, 0, 24, 24),

                    MaxHealth = 150f,

                    HitSound = hitSound,

                    TowerBullet = new TowerBullet(projectileTexture)
                    {
                        Name = "BulletEnemy",
                        BaseDamageAmount = 20f,
                        Speed = 300f,
                        Viewport = new Rectangle(16, 16, 16, 16)
                    }
                }
            },
            {
                119,
                new DemonEnemy(_DemonTexture){
                    Name = "Enemy",//I want to name Skeleton but bullet code dectect enemy by name
                    Viewport = new Rectangle(0, 0, 16, 32),

                    MaxHealth = 100f,

                    HitSound = hitSound,

                    DemonBullet = new DemonBullet(projectileTexture)
                    {
                        Name = "BulletEnemy",
                        BaseDamageAmount = 15f,
                        Speed = 200f,
                        Viewport = new Rectangle(4, 19, 8, 9)
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
                    Friction = 0.96f,

                    HitSound = hitSound
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
            //         Friction = 0.96f,

                    // HitSound = hitSound
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
                    Friction = 0.96f,

                    HitSound = hitSound
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

                    HitSound = hitSound,
                    
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
        Texture2D ItemTexture = _content.Load<Texture2D>("Items");
        Texture2D projectileTexture = _content.Load<Texture2D>("Projectile");
        Texture2D ExplosionEffect = _content.Load<Texture2D>("Explosion");
        Texture2D SoulStaff = _content.Load<Texture2D>("SoulStaff");
        

        //set for all item
        Item.TooltipBackgroundTexture = _content.Load<Texture2D>("ItemSlot");
        Item.PickUpSound = _content.Load<SoundEffect>("PickUp");

        SoundEffect PotionUseSound = _content.Load<SoundEffect>("PotionUse");
        _gameObjects.Add(new Potion(ItemTexture, ItemType.Consumable, TileMap.GetTileWorldPositionAt(12, 90)){
            Name =  "HealthPotion",
            Description = "Test HealthPotion Description",
            Viewport = new Rectangle(64, 64, 32, 32),
            UseSound = PotionUseSound
        });

        _gameObjects.Add(new SpeedPotion(ItemTexture, ItemType.Consumable, TileMap.GetTileWorldPositionAt(31, 90)){
            Name =  "SpeedPotion",
            Description = "Test SpeedPotion Description",
            Viewport = new Rectangle(0, 96, 32, 32),
            UseSound = PotionUseSound
        });

        _gameObjects.Add(new JumpPotion(ItemTexture, ItemType.Consumable, TileMap.GetTileWorldPositionAt(35, 90)){
            Name =  "jumppotion",
            Description = "Test JumpPotion Description",
            Viewport = new Rectangle(32, 64, 32, 32),
            UseSound = PotionUseSound
        });

        _gameObjects.Add(new Barrier(ItemTexture, ItemType.Consumable, TileMap.GetTileWorldPositionAt(20, 90)){
            Name =  "barrier",
            Description = "Test Barrier Description",
            Viewport = new Rectangle(0, 64, 32, 32),
            UseSound = PotionUseSound // Temp
        });

        _gameObjects.Add(new LifeUp(ItemTexture, ItemType.Consumable, TileMap.GetTileWorldPositionAt(16, 90)){
            Name =  "1Up",
            Description = "Test LifeUp Description",
            Viewport = new Rectangle(32, 96, 32, 32),
            UseSound = PotionUseSound // Temp
        });

        _gameObjects.Add(new SpeedBoots(ItemTexture, ItemType.Accessory, TileMap.GetTileWorldPositionAt(24, 90)){
            Name =  "SpeedBoots",
            Description = "Test SpeedBoots Description",
            Viewport = new Rectangle(32, 32, 32, 32)
        });

        _gameObjects.Add(new CursedGauntlet(ItemTexture, ItemType.Accessory, TileMap.GetTileWorldPositionAt(26, 80)){
            Name =  "CursedGauntlet",
            Description = "Test CursedGauntlet Description",
            Viewport = new Rectangle(64, 32, 32, 32)
        });

        SoundEffect SwordSlashSound = _content.Load<SoundEffect>("SwordSlash");
        _gameObjects.Add(new Sword(ItemTexture, ItemType.MeleeWeapon, TileMap.GetTileWorldPositionAt(4, 90)){
            Name =  "Sword",
            Description = "Test Sword Description",
            Viewport = new Rectangle(32, 0, 32, 32),
            SlashSound = SwordSlashSound,
        });

        SoundEffect GunshotSound = _content.Load<SoundEffect>("Gunshot");
        _gameObjects.Add(new Gun(ItemTexture, ItemType.RangeWeapon, TileMap.GetTileWorldPositionAt(8, 90)){
            Name =  "Gun",
            Description = "Test Gun Description",
            Viewport = new Rectangle(0, 0, 32, 32),
            ShootSound = GunshotSound,
        });


        SoundEffect FireBallShootingSound = _content.Load<SoundEffect>("FireBallShooting");
        SoundEffect FireBallExplosionSound = _content.Load<SoundEffect>("FireBallExplosion");
        _gameObjects.Add(new Staff(ItemTexture, ItemType.RangeWeapon, TileMap.GetTileWorldPositionAt(40, 90)){
            Name =  "Staff",
            Description = "Test Staff Description",
            MPCost = 10,
            ShootSound = FireBallShootingSound,

            FireBall = new FireBall(projectileTexture, ExplosionEffect, FireBallExplosionSound)
            {
                Name = "FireBall",
                BaseDamageAmount = 30f,
                Speed = 500f,
                Radius = 60f,
                ExplosionDuration = 0.5f,
                Viewport = new Rectangle(35, 17, 10, 13)
            },
            Viewport = new Rectangle(64, 0, 32, 32),
        });

        _gameObjects.Add(new SoulStaff(ItemTexture,ItemType.Consumable, TileMap.GetTileWorldPositionAt(16, 90)){
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
            Viewport = new Rectangle(0, 32, 32, 32),
        });

        _gameObjects.Add(new Grenade(ItemTexture, ItemType.Consumable, TileMap.GetTileWorldPositionAt(1, 90)){
            Name =  "Grenade",
            Description = "Test GrenadeTemp Description",
            Viewport = new Rectangle(64, 96, 32, 32),
            UseSound = PotionUseSound, // Temp

            GrenadeProjectile = new GrenadeProjectile(projectileTexture, ExplosionEffect, FireBallExplosionSound)
            {
                // Grenade properties
                Name = "GrenadeProjectile",
                BaseDamageAmount = 30f,
                Speed = 450f,
                Radius = 80f,
                ExplosionDuration = 0.5f,
                DetonateDelayDuration = 3.0f,

                Viewport = new Rectangle(35, 17, 10, 13)
            }
        });
    }

    protected override void SetupHUD()
    {
        _ui.ClearHUD();

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
