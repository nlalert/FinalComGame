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
    
    public TileMap CollisionTileMap;
    public TileMap BackgroundTileMap;
    public TileMap MiddlegroundTileMap;
    public TileMap ForegroundTileMap;

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
        CollisionTileMap.Update(gameTime, gameObjects);
    }

    public void DrawParallaxBackground(SpriteBatch spriteBatch)
    {
        _parallaxBackground.Draw(spriteBatch);
    }

    public void DrawTileMaps(SpriteBatch spriteBatch)
    {
        BackgroundTileMap.Draw(spriteBatch);
        MiddlegroundTileMap.Draw(spriteBatch);
        ForegroundTileMap.Draw(spriteBatch);
        
        //Should be hidden
        //_collisionTileMap.Draw(spriteBatch);
    }

    public void LoadTileMaps(Texture2D textureAtlas)
    {
        BackgroundTileMap = new TileMap(textureAtlas, 20);
        MiddlegroundTileMap = new TileMap(textureAtlas, 20);
        ForegroundTileMap = new TileMap(textureAtlas, 20);
        
        CollisionTileMap = new TileMap(textureAtlas, 20);

        BackgroundTileMap.LoadMap(GetCurrentStagePath() + "_BackGround.csv");
        MiddlegroundTileMap.LoadMap(GetCurrentStagePath() + "_MidGround.csv");
        ForegroundTileMap.LoadMap(GetCurrentStagePath() + "_ForeGround.csv");
        
        CollisionTileMap.LoadMap(GetCurrentStagePath() + "_Collision.csv");

        _enemySpawnPoints = CollisionTileMap.GetEnemySpawnPoints();
        _itemSpawnPoints = CollisionTileMap.GetItemSpawnPoints();
        _playerSpawnPoint = CollisionTileMap.GetPlayerSpawnPoint();
    }

    public static string GetCurrentStagePath()
    {
        return "../../../Data/Level_" + Singleton.Instance.Stage + "/Level_" + Singleton.Instance.Stage;
    }

    public Dictionary<Vector2, EnemyID> GetEnemySpawnPoints()
    {
        return _enemySpawnPoints;
    }

    public Dictionary<Vector2, ItemID> GetItemSpawnPoints()
    {
        return _itemSpawnPoints;
    }

    public Vector2 GetPlayerWorldSpawnPoint()
    {
        return _playerSpawnPoint;
    }

    public int GetMapWorldWidth()
    {
        return CollisionTileMap.MapWidth * Singleton.TILE_SIZE;
    }
    public int GetMapWorldHeight()
    {
        return CollisionTileMap.MapHeight * Singleton.TILE_SIZE;
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

        _parallaxBackground.AddLayer(_backgroundLayer1, 0.0f, 1.0f); // Sky/clouds move very slowly
        _parallaxBackground.AddLayer(_backgroundLayer2, 0.1f, 1.5f); // Mountains move at medium speed
        _parallaxBackground.AddLayer(_backgroundLayer3, 0.2f, 2.0f); // Trees move faster (closer to player)
    }

    private void LoadParallaxBackgoundTextures(ContentManager content)
    {
        _backgroundLayer1 = content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_bg");  // Farthest layer
        _backgroundLayer2 = content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_mg");  // Middle layer
        _backgroundLayer3 = content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_fg");  // Closest layer
    }

    public void UpdateAmbushAreas(GameTime gameTime, List<GameObject> gameObjects)
    {
        foreach (var ambushArea in _ambushAreas)
        {
            ambushArea.Update(gameTime, gameObjects, this);
        }
    }

    public void InitializeAmbushAreas()
    {
        _ambushAreas = CollisionTileMap.GetAmbushAreas();
    }

    public List<AmbushArea> GetAmbushAreas()
    {
        return _ambushAreas;
    }
}