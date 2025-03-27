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
        private Dictionary<Vector2, int> _enemySpawnPoints;
        private List<BaseEnemy> _spawnedEnemies;
        private bool _isActive;
        private bool _isCleared;

        SlimeEnemy slimeEnemy;
        public AmbushArea(Rectangle triggerZone, TileMap tileMap, SlimeEnemy tempSlime)
        {
            slimeEnemy = tempSlime;
            this._triggerZone = triggerZone;
            _spawnedEnemies = new List<BaseEnemy>();
            
            // Find enemy spawn points within this area
            _enemySpawnPoints = new Dictionary<Vector2, int>();
            foreach (var spawnPoint in tileMap.GetEnemySpawnPoints())
            {
                Vector2 worldPosition = TileMap.GetTileWorldPositionAt(spawnPoint.Key);
                if (triggerZone.Contains(worldPosition))
                {
                    Console.WriteLine("Add : "+spawnPoint.Value);
                    _enemySpawnPoints.Add(spawnPoint.Key, spawnPoint.Value);
                }
            }
        }

        public void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            // Check if player is in the trigger zone
            if (!_isActive && _triggerZone.Contains(Singleton.Instance.Player.Position))
            {
                ActivateAmbush(gameObjects, tileMap);
                Console.WriteLine("Active!");
            }

            // Check if all enemies are defeated
            if (!_isCleared)
            {
                _spawnedEnemies.RemoveAll(enemy => !enemy.IsActive);

                if (_spawnedEnemies.Count == 0)
                {
                    _isCleared = true;
                    ChangeBarrierCollision(tileMap, false);
                    Console.WriteLine("Clear");
                }
            }
        }

        private void ActivateAmbush(List<GameObject> gameObjects, TileMap tileMap)
        {
            _isActive = true;
            _isCleared = false;

            ChangeBarrierCollision(tileMap, true);

            foreach (var enemySpawnPoint in _enemySpawnPoints)
            {
                switch (enemySpawnPoint.Value)
                {
                    case 97:
                        // HellhoundEnemy.
                        slimeEnemy.Spawn(TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key), gameObjects, _spawnedEnemies);
                        // _enemyDog.Spawn(TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key), _gameObjects);
                        // _baseSkeleton.Spawn(TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key), _gameObjects);
                        // _enemyDemon.Spawn(TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key), _gameObjects);
                        // _enemyTower.Spawn(TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key)+ new Vector2(0,-196), _gameObjects);
                        break;
                    default:
                        break;
                }
                
            }  

            
        }

        private static void ChangeBarrierCollision(TileMap tileMap, bool IsSolid)
        {
            //TODO : Optimize this
            foreach (var tile in tileMap.tiles)
            {
                if(tile.Value.Type == TileType.AmbushBarrier)
                    tile.Value.IsSolid = IsSolid;
            }
        }
    }
}