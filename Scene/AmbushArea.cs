using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class AmbushArea
    {
        private Rectangle _triggerZone;
        private Dictionary<Vector2, int> _enemyWorldSpawnPoints;
        private List<BaseEnemy> _spawnedEnemies;
        private bool _isActive;
        private bool _isCleared;

        // Dictionary to map spawn types to enemy prefabs
        private Dictionary<int, BaseEnemy> _enemyPrefabs;

        public AmbushArea(Rectangle triggerZone, TileMap tileMap, Dictionary<int, BaseEnemy> enemyPrefabs)
        {
            _triggerZone = triggerZone;
            _spawnedEnemies = new List<BaseEnemy>();
            _enemyPrefabs = enemyPrefabs;
            
            // Find enemy spawn points within this area
            _enemyWorldSpawnPoints = new Dictionary<Vector2, int>();
            foreach (var spawnPoint in tileMap.GetEnemySpawnPoints())
            {
                Vector2 worldPosition = TileMap.GetTileWorldPositionAt(spawnPoint.Key);
                if (triggerZone.Contains(worldPosition))
                {
                    _enemyWorldSpawnPoints.Add(worldPosition, spawnPoint.Value);
                }
            }
        }

        public bool IsEnemyPositionInAmbushArea(Vector2 enemySpawnPoint)
        {
            return _enemyWorldSpawnPoints.ContainsKey(TileMap.GetTileWorldPositionAt(enemySpawnPoint));
        }

        public void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            // Check if player is in the trigger zone
            if (!_isActive && _triggerZone.Contains(Singleton.Instance.Player.Position))
            {
                ActivateAmbush(gameObjects, tileMap);
                Console.WriteLine("Ambush Activate");
            }

            // Check if all enemies are defeated
            if (_isActive && !_isCleared)
            {
                _spawnedEnemies.RemoveAll(enemy => !enemy.IsActive);

                if (_spawnedEnemies.Count == 0)
                {
                    _isCleared = true;
                    ChangeBarrierCollision(tileMap, false);
                    Console.WriteLine("Ambush Cleared");
                }
            }
        }

        private void ActivateAmbush(List<GameObject> gameObjects, TileMap tileMap)
        {
            _isActive = true;
            _isCleared = false;

            ChangeBarrierCollision(tileMap, true);

            foreach (var enemySpawnPoint in _enemyWorldSpawnPoints)
            {
                _enemyPrefabs[enemySpawnPoint.Value].Spawn(enemySpawnPoint.Key, gameObjects, _spawnedEnemies);
            }
        }

        private static void ChangeBarrierCollision(TileMap tileMap, bool IsSolid)
        {
            foreach (var tile in tileMap.tiles)
            {
                if(tile.Value.Type == TileType.AmbushBarrier)
                    tile.Value.IsSolid = IsSolid;
            }
        }
    }
}