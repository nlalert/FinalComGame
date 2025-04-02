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
    Rhulk = 139
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
}
