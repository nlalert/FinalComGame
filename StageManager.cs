using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class StageManager
{
    private ParallaxBackground _parallaxBackground;
    private Texture2D _backgroundLayer1;
    private Texture2D _backgroundLayer2;
    private Texture2D _backgroundLayer3;
    
    private TileMap _collisionTileMap;
    private TileMap _backgroundTileMap;
    private TileMap _middlegroundTileMap;
    private TileMap _foregroundTileMap;

    private List<AmbushArea> _ambushAreas;
    private Dictionary<Vector2, EnemyID> _enemySpawnPoints; 
    private Dictionary<Vector2, ItemID> _itemSpawnPoints;
    private Vector2 _playerSpawnPoint;

    public StageManager()
    {
        _ambushAreas = new List<AmbushArea>();
        _enemySpawnPoints = new Dictionary<Vector2, EnemyID>();
        _itemSpawnPoints = new Dictionary<Vector2, ItemID>();
    }

    public void UpdateParallaxBackground(GameTime gameTime)
    {
        _parallaxBackground.Update(gameTime);
    }

    public void UpdateTileMap(GameTime gameTime, List<GameObject> gameObjects)
    {
        _collisionTileMap.Update(gameTime, gameObjects);
    }

    public TileMap GetCollisionTileMap()
    {
        return _collisionTileMap;
    }

    public void DrawParallaxBackground(SpriteBatch spriteBatch)
    {
        _parallaxBackground.Draw(spriteBatch);
    }

    public void DrawTileMaps(SpriteBatch spriteBatch)
    {
        _backgroundTileMap.Draw(spriteBatch);
        _middlegroundTileMap.Draw(spriteBatch);
        _foregroundTileMap.Draw(spriteBatch);
        
        //Should be hidden
        //_collisionTileMap.Draw(spriteBatch);
    }

    public void LoadTileMaps(Texture2D textureAtlas)
    {
        _backgroundTileMap = new TileMap(textureAtlas, 20);
        _middlegroundTileMap = new TileMap(textureAtlas, 20);
        _foregroundTileMap = new TileMap(textureAtlas, 20);
        
        _collisionTileMap = new TileMap(textureAtlas, 20);

        _backgroundTileMap.LoadMap(GetCurrentStagePath() + "_BackGround.csv");
        _middlegroundTileMap.LoadMap(GetCurrentStagePath() + "_MidGround.csv");
        _foregroundTileMap.LoadMap(GetCurrentStagePath() + "_ForeGround.csv");
        
        _collisionTileMap.LoadMap(GetCurrentStagePath() + "_Collision.csv");

        _enemySpawnPoints = _collisionTileMap.GetEnemySpawnPoints();
        _itemSpawnPoints = _collisionTileMap.GetItemSpawnPoints();
        _playerSpawnPoint = _collisionTileMap.GetPlayerSpawnPoint();
    }

    public static string GetCurrentStagePath()
    {
        return "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage;
    }

    public Dictionary<Vector2, EnemyID> GetEnemySpawnPoints()
    {
        return _collisionTileMap.GetEnemySpawnPoints();
    }

    public Dictionary<Vector2, ItemID> GetItemSpawnPoints()
    {
        return _collisionTileMap.GetItemSpawnPoints();
    }

    public Vector2 GetPlayerWorldSpawnPoint()
    {
        return _collisionTileMap.GetPlayerSpawnPoint();
    }

    public int GetMapWorldWidth()
    {
        return _collisionTileMap.MapWidth * Singleton.TILE_SIZE;
    }
    public int GetMapWorldHeight()
    {
        return _collisionTileMap.MapHeight * Singleton.TILE_SIZE;
    }


    public void SetUpParallaxBackground(ContentManager content, GraphicsDevice graphicsDevice)
    {
        LoadParallaxBackgoundTextures(content);
        CreateParallaxBackground(graphicsDevice);
    }

    private void CreateParallaxBackground(GraphicsDevice graphicsDevice)
    {
        // Create parallax background
        _parallaxBackground = new ParallaxBackground(graphicsDevice.Viewport);

        _parallaxBackground.AddLayer(_backgroundLayer1, 0.0f, 1.0f, Vector2.Zero); // Sky/clouds move very slowly
        _parallaxBackground.AddLayer(_backgroundLayer2, 0.1f, 1.5f, new Vector2(-50,-300)); // Mountains move at medium speed
        _parallaxBackground.AddLayer(_backgroundLayer3, 0.2f, 2.0f, new Vector2(-100,-800)); // Trees move faster (closer to player)
    }

    private void LoadParallaxBackgoundTextures(ContentManager content)
    {
        // _backgroundLayer1 = content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_bg");  // Farthest layer
        // _backgroundLayer2 = content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_mg");  // Middle layer
        // _backgroundLayer3 = content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_fg");  // Closest layer

        _backgroundLayer1 = content.Load<Texture2D>("Level_1_Parallax_bg");  // Farthest layer
        _backgroundLayer2 = content.Load<Texture2D>("Level_1_Parallax_mg");  // Middle layer
        _backgroundLayer3 = content.Load<Texture2D>("Level_1_Parallax_fg");  // Closest layer
    }

    public void UpdateAmbushAreas(GameTime gameTime, List<GameObject> gameObjects)
    {
        foreach (var ambushArea in _ambushAreas)
        {
            ambushArea.Update(gameTime, gameObjects, _collisionTileMap);
        }
    }

    public void InitializeAmbushAreas()
    {
        _ambushAreas = _collisionTileMap.GetAmbushAreas();
    }

    public List<AmbushArea> GetAmbushAreas()
    {
        return _ambushAreas;
    }
}