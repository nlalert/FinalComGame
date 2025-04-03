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
        private Dictionary<Vector2, EnemyID> _enemyWorldSpawnPoints;
        private List<BaseEnemy> _spawnedEnemies;
        private Dictionary<Vector2, Tile> _activatedBarrierTiles;
        private bool _isActive;
        private bool _isCleared;

        public AmbushArea(Rectangle triggerZone, TileMap tileMap)
        {
            _triggerZone = triggerZone;
            _spawnedEnemies = new List<BaseEnemy>();
            _activatedBarrierTiles = new Dictionary<Vector2, Tile>();
            
            // Find enemy spawn points within this area
            _enemyWorldSpawnPoints = new Dictionary<Vector2, EnemyID>();
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

        public void Update(GameTime gameTime, List<GameObject> gameObjects, StageManager stageManager)
        {
            // Check if player is in the trigger zone
            if (!_isActive && _triggerZone.Contains(Singleton.Instance.Player.Position))
            {
                ActivateAmbush(gameObjects, stageManager);
                Console.WriteLine("Ambush Activate");
            }

            // Check if all enemies are defeated
            if (_isActive && !_isCleared)
            {
                _spawnedEnemies.RemoveAll(enemy => !enemy.IsActive);

                if (_spawnedEnemies.Count == 0)
                {
                    _isCleared = true;
                    DisableBarrierCollision(stageManager);
                    Singleton.Instance.CurrentUI.Prompt("Ambush Cleared");
                }
            }
        }

        private void ActivateAmbush(List<GameObject> gameObjects, StageManager stageManager)
        {
            _isActive = true;
            _isCleared = false;

            EnableBarrierCollision(stageManager);

            foreach (var enemySpawnPoint in _enemyWorldSpawnPoints)
            {
                BaseEnemy newEnemy = EnemyManager.SpawnEnemy(enemySpawnPoint.Value, enemySpawnPoint.Key, gameObjects);
                _spawnedEnemies.Add(newEnemy);
            }
        }

        private void EnableBarrierCollision(StageManager stageManager)
        {

            foreach (var tile in stageManager.CollisionTileMap.Tiles)
            {
                if(tile.Value.Type == TileType.AmbushBarrier)
                {
                    _activatedBarrierTiles.Add(tile.Key, tile.Value);
                    tile.Value.IsSolid = true;

                    // Close Door
                    if (stageManager.ForegroundTileMap.Tiles.TryGetValue(tile.Key, out Tile foregroundTile))
                    {
                        foregroundTile.ID++;
                        foregroundTile.Viewport = stageManager.ForegroundTileMap.GetTileViewport(foregroundTile.ID);
                    }
                }
            }
        }

        private void DisableBarrierCollision(StageManager stageManager)
        {
            foreach (var tile in _activatedBarrierTiles)
            {
                tile.Value.IsSolid = false;

                // Open Door
                if (stageManager.ForegroundTileMap.Tiles.TryGetValue(tile.Key, out Tile foregroundTile))
                {
                    foregroundTile.ID--;
                    foregroundTile.Viewport = stageManager.ForegroundTileMap.GetTileViewport(foregroundTile.ID);
                }
            }
        }
    }
}