using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public enum EnemyID
{
    None = 0,
    Slime = 97,
    Hellhound = 98,
    Skeleton = 99,
    PlatformEnemy = 117,
    TowerEnemy = 118,
    Demon = 119,
    GiantSlime = 137,
    Cerberus = 138,
    Rhulk = 199
}


public class EnemyManager
{
    private static Dictionary<EnemyID, BaseEnemy> _enemyPrefabs = new Dictionary<EnemyID, BaseEnemy>();

    // Add enemy prototype to manager with enum-based ID
    public static void AddGameEnemy(EnemyID enemyID, BaseEnemy enemy)
    {
        _enemyPrefabs[enemyID] = enemy;
    }

    // Spawn a specific enemy at a position
    public static BaseEnemy SpawnEnemy(EnemyID enemyID, Vector2 spawnPosition, List<GameObject> gameObjects)
    {
        if (!_enemyPrefabs.ContainsKey(enemyID))
            return null;
            
        return _enemyPrefabs[enemyID].Spawn(spawnPosition, gameObjects);
    }

    // Convert from tile ID to enemy ID
    public static EnemyID GetEnemyIDFromTileID(int tileID)
    {
        if (Enum.IsDefined(typeof(EnemyID), tileID))
            return (EnemyID)tileID;
        return EnemyID.None;
    }

    // Spawn enemies in the world based on tile map data
    public static void SpawnWorldEnemy(Dictionary<Vector2, EnemyID> enemySpawnPoints, List<AmbushArea> ambushAreas, List<GameObject> gameObjects)
    {
        foreach (var enemySpawnPoint in enemySpawnPoints)
        {
            bool isEnemyPositionInAmbushArea = false;
            foreach (AmbushArea ambushArea in ambushAreas)
            {
                if (ambushArea.IsEnemyPositionInAmbushArea(enemySpawnPoint.Key))
                {
                    isEnemyPositionInAmbushArea = true;
                    break;
                }
            }
            
            if (!isEnemyPositionInAmbushArea && enemySpawnPoint.Value != EnemyID.None)
            {
                SpawnEnemy(enemySpawnPoint.Value, TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key), gameObjects);
            }
        }
    }

    // For backward compatibility - convert from Vector2/int dictionary to Vector2/EnemyID dictionary
    public static Dictionary<Vector2, EnemyID> ConvertSpawnPoints(Dictionary<Vector2, int> oldSpawnPoints)
    {
        Dictionary<Vector2, EnemyID> newSpawnPoints = new Dictionary<Vector2, EnemyID>();
        
        foreach (var kvp in oldSpawnPoints)
        {
            newSpawnPoints[kvp.Key] = GetEnemyIDFromTileID(kvp.Value);
        }
        
        return newSpawnPoints;
    }

    // Helper method to get enemy by ID (useful for debugging/editor tools)
    public static BaseEnemy GetEnemyPrefab(EnemyID enemyID)
    {
        if (_enemyPrefabs.TryGetValue(enemyID, out BaseEnemy enemy))
            return enemy;
        return null;
    }

    // Helper method to get all available enemy types
    public static List<EnemyID> GetAllEnemyTypes()
    {
        return new List<EnemyID>(_enemyPrefabs.Keys);
    }

    // Spawn a random enemy at a position (useful for random encounters or tests)
    public static BaseEnemy SpawnRandomEnemy(Vector2 spawnPosition, List<GameObject> gameObjects)
    {
        if (_enemyPrefabs.Count == 0)
            return null;
            
        List<EnemyID> enemyTypes = new List<EnemyID>(_enemyPrefabs.Keys);
        int randomIndex = Singleton.Instance.Random.Next(enemyTypes.Count);
        return SpawnEnemy(enemyTypes[randomIndex], spawnPosition, gameObjects);
    }
}
