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
        _backgroundTileMap = new TileMap(textureAtlas, GetCurrentStagePath() + "_BackGround.csv", 20);
        _middlegroundTileMap = new TileMap(textureAtlas, GetCurrentStagePath() + "_MidGround.csv", 20);
        _foregroundTileMap = new TileMap(textureAtlas, GetCurrentStagePath() + "_ForeGround.csv", 20);
        
        _collisionTileMap = new TileMap(textureAtlas, GetCurrentStagePath() + "_Collision.csv", 20);
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

    public int GetMapWorldWidth()
    {
        return _collisionTileMap.MapWidth * Singleton.TILE_SIZE;
    }
    public int GetMapWorldHeight()
    {
        return _collisionTileMap.MapHeight * Singleton.TILE_SIZE;
    }

    public Vector2 GetPlayerWorldSpawnPoint()
    {
        return _collisionTileMap.GetPlayerSpawnPoint();
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
        // _backgroundLayer1 = content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_bg");  // Farthest layer
        // _backgroundLayer2 = content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_mg");  // Middle layer
        // _backgroundLayer3 = content.Load<Texture2D>("Level_" + Singleton.Instance.Stage + "_Parallax_fg");  // Closest layer

        _backgroundLayer1 = content.Load<Texture2D>("Level_1_Parallax_bg");  // Farthest layer
        _backgroundLayer2 = content.Load<Texture2D>("Level_1_Parallax_mg");  // Middle layer
        _backgroundLayer3 = content.Load<Texture2D>("Level_1_Parallax_fg");  // Closest layer
    }
}