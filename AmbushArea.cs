using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class AmbushArea
    {
        private Rectangle triggerZone;
        private Dictionary<Vector2, int> enemySpawnPoints;
        private List<BaseEnemy> spawnedEnemies;
        private bool isActive;
        private bool isCleared;

        SlimeEnemy slimeEnemy;
        public AmbushArea(Rectangle triggerZone, TileMap tileMap, SlimeEnemy tempSlime)
        {
            slimeEnemy = tempSlime;
            this.triggerZone = triggerZone;
            spawnedEnemies = new List<BaseEnemy>();
            
            // Find enemy spawn points within this area
            enemySpawnPoints = new Dictionary<Vector2, int>();
            foreach (var spawnPoint in tileMap.GetEnemySpawnPoints())
            {
                Vector2 worldPosition = TileMap.GetTileWorldPositionAt(spawnPoint.Key);
                if (triggerZone.Contains(worldPosition))
                {
                    Console.WriteLine("Add : "+spawnPoint.Value);
                    enemySpawnPoints.Add(spawnPoint.Key, spawnPoint.Value);
                }
            }
        }

        public void Update(GameTime gameTime, List<GameObject> gameObjects, TileMap tileMap)
        {
            // Check if player is in the trigger zone
            if (!isActive && triggerZone.Contains(Singleton.Instance.Player.Position))
            {
                ActivateAmbush(gameObjects, tileMap);
                Console.WriteLine("Active!");
            }

            // Check if all enemies are defeated
            if (!isCleared)
            {
                spawnedEnemies.RemoveAll(enemy => !enemy.IsActive);

                if (spawnedEnemies.Count == 0)
                {
                    isCleared = true;
                    //TODO : Optimize this
                    foreach (var tile in tileMap.tiles)
                    {
                        if(tile.Value.Type == TileType.AmbushBarrier)
                            tile.Value.IsSolid = false;
                    }
                    Console.WriteLine("Clear");
                }
            }
        }

        private void ActivateAmbush(List<GameObject> gameObjects, TileMap tileMap)
        {
            isActive = true;
            isCleared = false;
            //TODO : Optimize this
            foreach (var tile in tileMap.tiles)
            {
                if(tile.Value.Type == TileType.AmbushBarrier)
                    tile.Value.IsSolid = true;
            }

            foreach (var enemySpawnPoint in tileMap.GetEnemySpawnPoints())
            {
                switch (enemySpawnPoint.Value)
                {
                    case 97:
                        // HellhoundEnemy.
                        slimeEnemy.Spawn(TileMap.GetTileWorldPositionAt(enemySpawnPoint.Key), gameObjects, spawnedEnemies);
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

        public bool CanPlayerLeave()
        {
            // Player can only leave if the ambush area is cleared
            return isCleared;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Optional: Draw debug visualization of the ambush area
            // This helps during development to see the trigger zone
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.Red * 0.3f });
            spriteBatch.Draw(pixel, triggerZone, Color.White);
        }
    }
}