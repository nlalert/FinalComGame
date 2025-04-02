using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class EnemyManager
{
    private static Dictionary<int, BaseEnemy> _enemyPrefabs = new Dictionary<int, BaseEnemy>();

    public static void AddGameEnemy(int enemyID, BaseEnemy enemy){
        _enemyPrefabs.TryAdd(enemyID, enemy);
    }

    public static BaseEnemy SpawnEnemy(int enemyID, Vector2 spawnPosition, List<GameObject> gameObjects){
        return _enemyPrefabs[enemyID].Spawn(spawnPosition, gameObjects);
    }

    public static void SpawnWorldEnemy(Dictionary<Vector2, int> enemySpawnPoints, List<AmbushArea> ambushAreas, List<GameObject> gameObjects)
    {
        foreach (var enemySpawnPoint in enemySpawnPoints)
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
                SpawnEnemy(enemySpawnPoint.Value, TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key), gameObjects);
        }
    }
}