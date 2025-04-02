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

    private Texture2D _textureAtlas;
    private Texture2D _LaserTexture;
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

        _LaserTexture = _content.Load<Texture2D>("Laserbeam");
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
        //if (Singleton.Instance.Stage == 1){//remove later
        _BGTileMap.Draw(_spriteBatch);
        _MGTileMap.Draw(_spriteBatch);
        _FGTileMap.Draw(_spriteBatch);
        //}
        
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
        //if (Singleton.Instance.Stage == 1)//remove later
        //{
            _BGTileMap = new TileMap(_textureAtlas, "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_BackGround.csv", 20);
            _MGTileMap = new TileMap(_textureAtlas, "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_MidGround.csv", 20);
            _FGTileMap = new TileMap(_textureAtlas, "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_ForeGround.csv", 20);
        //}
        _collisionTileMap = new TileMap(_textureAtlas, GetCurrentStageCollisionPath(), 20);

        Rectangle mapBounds = new Rectangle(0, 0,  _collisionTileMap.MapWidth * Singleton.TILE_SIZE,  _collisionTileMap.MapHeight * Singleton.TILE_SIZE); // Map size
        Singleton.Instance.Camera = new Camera(_graphicsDevice.Viewport, mapBounds); // Initialize camera

        Singleton.Instance.Player.Position = _collisionTileMap.GetPlayerSpawnPoint();// get player location of each stage
        _gameObjects.Add(Singleton.Instance.Player);

        SetUpParallaxBackground();
        InitializeAmbushAreas();
        SpawnEnemies();
        SpawnItems();
        AddItems(); // TODO: Remove Later this is only for testing
        SetupHUD();
        
        foreach (GameObject s in _gameObjects)
        {
            s.Reset();
        }
    }

    private void SetUpParallaxBackground()
    {
        // // Load background textures
        // _backgroundLayer1 = _content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_bg");  // Farthest layer
        // _backgroundLayer2 = _content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_mg");  // Middle layer
        // _backgroundLayer3 = _content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_fg");  // Closest layer

        _backgroundLayer1 = _content.Load<Texture2D>("Level_1_Parallax_bg");  // Farthest layer
        _backgroundLayer2 = _content.Load<Texture2D>("Level_1_Parallax_mg");  // Middle layer
        _backgroundLayer3 = _content.Load<Texture2D>("Level_1_Parallax_fg");  // Closest layer

        // Create parallax background
        _parallaxBackground = new ParallaxBackground(_graphicsDevice.Viewport);

        _parallaxBackground.AddLayer(_backgroundLayer1, 0.0f, 1.0f, Vector2.Zero); // Sky/clouds move very slowly
        _parallaxBackground.AddLayer(_backgroundLayer2, 0.1f, 1.5f, new Vector2(-50,-300)); // Mountains move at medium speed
        _parallaxBackground.AddLayer(_backgroundLayer3, 0.2f, 2.0f, new Vector2(-100,-800)); // Trees move faster (closer to player)
    }

    // In your PlayScene or main game class
    public void InitializeAmbushAreas()
    {
        ambushAreas = _collisionTileMap.GetAmbushAreas();
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
            AttackHeight = Singleton.TILE_SIZE * 2,

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
            JumpSound = playerJumpSound,
            DashSound = playerDashSound,
            PunchSound = playerPunchSound,
            ChargingSound = playerChargeBulletSound,
            BulletShotSound = playerBulletShotSound,

            Bullet = new PlayerBullet(projectileTexture)
            {
                Name = "BulletPlayer",
                BaseDamageAmount = 10f,
                Speed = 500f,
                Viewport = ViewportManager.Get("Charge_Bullet_0")
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

        Texture2D _GiantSlimeTexture = _content.Load<Texture2D>("LargeSlime");
        Texture2D _CerberusTexture = _content.Load<Texture2D>("Cerberus");
        Texture2D _RhulkTexture = _content.Load<Texture2D>("Rhulk");

        Texture2D projectileTexture = _content.Load<Texture2D>("Projectile");

        SoundEffect hitSound = _content.Load<SoundEffect>("HitEnemy");

        Dictionary<ItemID, float> defaultLootTableChance = new Dictionary<ItemID, float>{ 
            {ItemID.None, 0.8f},
            {ItemID.HealthPotion, 0.2f},
        };

        EnemyManager.AddGameEnemy(EnemyID.Slime,
            new SlimeEnemy(_SlimeTexture){
                Name = "Slime",
                Viewport = ViewportManager.Get("Slime"),
                MaxHealth = 50f,
                BaseAttackDamage = 3f,

                JumpCooldown = 3.0f,
                BaseJumpStrength = 490,
                Friction = 0.96f,

                HitSound = hitSound,

                LootTableChance = defaultLootTableChance
            });

        EnemyManager.AddGameEnemy(EnemyID.Hellhound,
                new HellhoundEnemy(_DogTexture){
                    Name = "Hellhound",
                    Viewport = ViewportManager.Get("Hellhound"),
                    
                    MaxHealth = 1f,
                    BaseAttackDamage = 8f,

                    LimitIdlePatrol = 100,
                    
                    ChargeTime = 2.0f,
                    ChaseDuration = 3.0f,
                    DashDuration = 1.0f,

                    HitSound = hitSound,

                    LootTableChance = defaultLootTableChance
                });

        EnemyManager.AddGameEnemy(EnemyID.Skeleton,         
            new SkeletonEnemy(_enemyTexture){
                Name = "Skeleton",
                Viewport = ViewportManager.Get("Skeleton"),

                MaxHealth = 80f,
                BaseAttackDamage = 5f,

                LimitIdlePatrol = 100,

                IgnorePlayerDuration = 3f,

                HitSound = hitSound,

                LootTableChance = defaultLootTableChance
            });

        EnemyManager.AddGameEnemy(EnemyID.PlatformEnemy,
            new PlatformEnemy(_PlatformTexture){
                Name = "PlatformEnemy",
                Viewport = ViewportManager.Get("PlatformEnemy"),

                MaxHealth = float.MaxValue,

                HitSound = hitSound,// Temp

                LootTableChance = defaultLootTableChance 
            });

        EnemyManager.AddGameEnemy(EnemyID.TowerEnemy,
            new TowerEnemy(_TowerTexture){
                Name = "TowerEnemy",
                Viewport = ViewportManager.Get("TowerEnemy"),

                MaxHealth = 150f,

                HitSound = hitSound,

                TowerBullet = new TowerBullet(projectileTexture)
                {
                    Name = "BulletEnemy",
                    BaseDamageAmount = 20f,
                    Speed = 300f,
                    Viewport = ViewportManager.Get("TowerEnemy_Bullet")
                },

                LootTableChance = defaultLootTableChance
            });

        EnemyManager.AddGameEnemy(EnemyID.Demon,
            new DemonEnemy(_DemonTexture){
                Name = "Demon",
                Viewport = ViewportManager.Get("Demon"),

                MaxHealth = 100f,

                HitSound = hitSound,

                DemonBullet = new DemonBullet(projectileTexture)
                {
                    Name = "BulletEnemy",
                    BaseDamageAmount = 15f,
                    Speed = 200f,
                    Viewport = ViewportManager.Get("Demon_Bullet")
                },

                LootTableChance = defaultLootTableChance
            });

        EnemyManager.AddGameEnemy(EnemyID.GiantSlime,         
            new GiantSlime(_GiantSlimeTexture, new Texture2D(_graphicsDevice, 1, 1)){
                Name = "GiantSlime",
                Viewport = ViewportManager.Get("GiantSlime"),

                MaxHealth = 1000f,
                BaseAttackDamage = 3f,
                IsIgnorePlatform = true,

                // JumpCooldown = 3.0f,
                BaseJumpStrength = 550,
                Friction = 0.96f,

                HitSound = hitSound
            });

        EnemyManager.AddGameEnemy(EnemyID.Cerberus,         
            new Cerberus(_CerberusTexture, new Texture2D(_graphicsDevice, 1, 1)){
                Name = "Cerberus",
                Viewport = ViewportManager.Get("Cerberus"),

                MaxHealth = 1000,
                BaseAttackDamage = 3f,

                // JumpCooldown = 3.0f,
                BaseJumpStrength = 550,
                Friction = 0.96f,

                HitSound = hitSound
            });

        EnemyManager.AddGameEnemy(EnemyID.Rhulk,         
            new Rhulk(_RhulkTexture){
                Name = "Rhulk",
                Viewport = ViewportManager.Get("Rhulk"),

                MaxHealth = 1000f,
                BaseAttackDamage = 3f,

                // JumpCooldown = 3.0f,
                BaseJumpStrength = 550,
                Friction = 0.96f,

                HitSound = hitSound,
                
                Laserproj = new DemonLaser(_LaserTexture)
                {
                    Name = "BulletEnemy",
                    BaseDamageAmount = 20f,
                    Viewport = ViewportManager.Get("Rhulk_Laser")
                }
            });
    }

    private void CreateItemPrefabs()
    {
        //set for all item
        Item.TooltipBackgroundTexture = _content.Load<Texture2D>("ItemSlot");
        Item.PickUpSound = _content.Load<SoundEffect>("PickUp");

        Texture2D ItemTexture = _content.Load<Texture2D>("Items");
        Texture2D projectileTexture = _content.Load<Texture2D>("Projectile");
        
        SoundEffect PotionUseSound = _content.Load<SoundEffect>("PotionUse");
        SoundEffect SwordSlashSound = _content.Load<SoundEffect>("SwordSlash");
        SoundEffect GunshotSound = _content.Load<SoundEffect>("Gunshot");
        SoundEffect FireBallShootingSound = _content.Load<SoundEffect>("FireBallShooting");
        SoundEffect FireBallExplosionSound = _content.Load<SoundEffect>("FireBallExplosion");

        //TODO : Change these to real ITEM ID
        ItemManager.AddGameItem(ItemID.HealthPotion,
            new Potion(ItemTexture, ItemType.Consumable){
                Name =  "HealthPotion",
                Description = "Test HealthPotion Description",
                Viewport = ViewportManager.Get("Potion_Health"),
                UseSound = PotionUseSound
            });

        ItemManager.AddGameItem(ItemID.SpeedPotion,
            new SpeedPotion(ItemTexture, ItemType.Consumable){
                Name =  "SpeedPotion",
                Description = "Test SpeedPotion Description",
                Viewport = ViewportManager.Get("Potion_Speed"),
                UseSound = PotionUseSound
            });

        ItemManager.AddGameItem(ItemID.JumpPotion,
            new JumpPotion(ItemTexture, ItemType.Consumable){
                Name =  "jumppotion",
                Description = "Test JumpPotion Description",
                Viewport = ViewportManager.Get("Potion_Jump"),
                UseSound = PotionUseSound
            });

        ItemManager.AddGameItem(ItemID.Barrier,
            new Barrier(ItemTexture, ItemType.Consumable){
                Name =  "barrier",
                Description = "Test Barrier Description",
                Viewport = ViewportManager.Get("Barrier"),
                UseSound = PotionUseSound // Temp
            });

        ItemManager.AddGameItem(ItemID.LifeUp,
            new LifeUp(ItemTexture, ItemType.Consumable){
                Name =  "1Up",
                Description = "Test LifeUp Description",
                Viewport = ViewportManager.Get("LifeUp"),
                UseSound = PotionUseSound // Temp
            });

        ItemManager.AddGameItem(ItemID.SpeedBoots,
            new SpeedBoots(ItemTexture, ItemType.Accessory){
                Name =  "SpeedBoots",
                Description = "Test SpeedBoots Description",
                Viewport = ViewportManager.Get("Speed_Boots")
            });

        ItemManager.AddGameItem(ItemID.CursedGauntlet,
            new CursedGauntlet(ItemTexture, ItemType.Accessory){
                Name =  "CursedGauntlet",
                Description = "Test CursedGauntlet Description",
                Viewport = ViewportManager.Get("CursedGauntlet")
            });

        ItemManager.AddGameItem(ItemID.Sword,
            new Sword(ItemTexture, ItemType.MeleeWeapon){
                Name =  "Sword",
                Description = "Test Sword Description",
                Viewport = ViewportManager.Get("Sword"),
                SlashSound = SwordSlashSound,
            });

        ItemManager.AddGameItem(ItemID.Gun,
            new Gun(ItemTexture, ItemType.RangeWeapon){
                Name =  "Gun",
                Description = "Test Gun Description",
                Viewport = ViewportManager.Get("Gun"),
                ShootSound = GunshotSound,
            });

        ItemManager.AddGameItem(ItemID.Staff,
            new Staff(ItemTexture, ItemType.RangeWeapon){
                Name =  "Staff",
                Description = "Test Staff Description",
                MPCost = 10,
                ShootSound = FireBallShootingSound,

                FireBall = new FireBall(projectileTexture)
                {
                    Name = "FireBall",
                    BaseDamageAmount = 30f,
                    Speed = 500f,
                    Radius = 30f,
                    ExplosionDuration = 0.5f,
                    Viewport = ViewportManager.Get("FireBall"),
                    BaseExplosion = new Explosion(projectileTexture, FireBallExplosionSound)
                    {
                        Viewport = ViewportManager.Get("Explosion")
                    }
                },
                Viewport = ViewportManager.Get("Fire_Staff")
            });

        ItemManager.AddGameItem(ItemID.SoulStaff,
            new SoulStaff(ItemTexture,ItemType.Consumable){
                Name =  "Staff",
                Description = "Summon Your best Minion!",
                MPCost = 10,
                soulMinion = new SoulMinion(projectileTexture)
                {
                    Name = "Soul Minion",
                    BaseDamageAmount = 0f,
                    Viewport = ViewportManager.Get("Soul_Minion"),
                    soulBullet = new SoulBullet(projectileTexture){
                        Name = "Soul Bullet",
                        BaseDamageAmount = 15f,
                        Speed = 150f,
                        Viewport = ViewportManager.Get("Soul_Bullet")
                    }
                },
                Viewport = ViewportManager.Get("Soul_Staff")
            });

        ItemManager.AddGameItem(ItemID.Grenade,
            new Grenade(ItemTexture, ItemType.Consumable){
                Name =  "Grenade",
                Description = "Test GrenadeTemp Description",
                Viewport = ViewportManager.Get("Grenade"),
                UseSound = PotionUseSound, // Temp

                GrenadeProjectile = new GrenadeProjectile(projectileTexture)
                {
                    // Grenade properties
                    Name = "GrenadeProjectile",
                    BaseDamageAmount = 30f,
                    Speed = 450f,
                    Radius = 50f,
                    ExplosionDuration = 0.5f,
                    DetonateDelayDuration = 3.0f,
                    Viewport = ViewportManager.Get("Grenade_Projectile"),
                    BaseExplosion = new Explosion(projectileTexture, FireBallExplosionSound)
                    {
                        Viewport = ViewportManager.Get("Explosion")
                    }
                }
            });
    }

    private void SpawnEnemies()
    {
        EnemyManager.SpawnWorldEnemy(_collisionTileMap.GetEnemySpawnPoints(), ambushAreas, _gameObjects);
    }

    private void SpawnItems()
    {
        _ui.ClearWorldSpaceUI();
        foreach (var itemSpawnPoint in _collisionTileMap.GetItemSpawnPoints())
        {
            ItemManager.SpawnItem(itemSpawnPoint.Value, TileMap.GetTileWorldPositionAt(itemSpawnPoint.Key), _gameObjects);
        }
    }

    private void AddItems()
    {
        ItemManager.SpawnItem(ItemID.HealthPotion, TileMap.GetTileWorldPositionAt(12, 90), _gameObjects);
        ItemManager.SpawnItem(ItemID.SpeedPotion, TileMap.GetTileWorldPositionAt(31, 90), _gameObjects);
        ItemManager.SpawnItem(ItemID.JumpPotion, TileMap.GetTileWorldPositionAt(35, 90), _gameObjects);
        ItemManager.SpawnItem(ItemID.Barrier, TileMap.GetTileWorldPositionAt(20, 90), _gameObjects);
        ItemManager.SpawnItem(ItemID.LifeUp, TileMap.GetTileWorldPositionAt(16, 90), _gameObjects);
        ItemManager.SpawnItem(ItemID.SpeedBoots, TileMap.GetTileWorldPositionAt(24, 90), _gameObjects);
        ItemManager.SpawnItem(ItemID.CursedGauntlet, TileMap.GetTileWorldPositionAt(26, 80), _gameObjects);
        ItemManager.SpawnItem(ItemID.Sword, TileMap.GetTileWorldPositionAt(4, 90), _gameObjects);
        ItemManager.SpawnItem(ItemID.Gun, TileMap.GetTileWorldPositionAt(8, 90), _gameObjects);
        ItemManager.SpawnItem(ItemID.Staff, TileMap.GetTileWorldPositionAt(40, 90), _gameObjects);
        ItemManager.SpawnItem(ItemID.SoulStaff, TileMap.GetTileWorldPositionAt(17, 90), _gameObjects);
        ItemManager.SpawnItem(ItemID.Grenade, TileMap.GetTileWorldPositionAt(10, 90), _gameObjects);
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
            Inventory.MELEE_SLOT,
            new Rectangle(250, 30, 50, 50),
            slot,
            slot
        );

        ItemSlot RangeWeaponSlot = new ItemSlot(
            Inventory.RANGE_SLOT,
            new Rectangle(350, 30, 50, 50),
            slot,
            slot
        );
        ItemSlot ItemSlot1 = new ItemSlot(
            Inventory.ITEM_SLOT_1,
            new Rectangle(550, 30, 50, 50),
            slot,
            slot
        );
        ItemSlot ItemSlot2 = new ItemSlot(
            Inventory.ITEM_SLOT_2,
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

    public static string GetCurrentStageCollisionPath()
    {
        if (Singleton.Instance.Stage >= 4)
        {
            Console.WriteLine("No more stage : Replaying");
            Singleton.Instance.Stage = 1;
        }

        return "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage + "_Collision.csv";
    }
}
