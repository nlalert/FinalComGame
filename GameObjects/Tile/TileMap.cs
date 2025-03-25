using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class TileMap
    {
        public Dictionary<Vector2, Tile> tiles; //Grid Position, tile
        public Dictionary<Vector2, int> enemySpawnPoints; // Grid position, enemy type ID
        private Texture2D textureAtlas;
        private int numTilesPerRow;

        public TileMap(Texture2D textureAtlas, string mapPath, int numTilesPerRow)
        {
            this.textureAtlas = textureAtlas;
            this.numTilesPerRow = numTilesPerRow;
            tiles = new Dictionary<Vector2, Tile>();
            enemySpawnPoints = new Dictionary<Vector2, int>();
            LoadMap(mapPath);
        }

        public void Update(GameTime gameTime, List<GameObject> gameObjects)
        {
            foreach (var tile in tiles)
            {
                tile.Value.Update(gameTime, gameObjects, this);
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tile in tiles)
            {
                tile.Value.Draw(spriteBatch);
            }
        }

        private void LoadMap(string filepath)
        {
            using (StreamReader reader = new StreamReader(filepath))
            {
                int y = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] items = line.Split(',');

                    for (int x = 0; x < items.Length; x++)
                    {
                        if (int.TryParse(items[x], out int tileID) && tileID >= 0)
                        {
                            if(IsEnemyTile(tileID))
                            {
                                // Store enemy spawn point with its type
                                enemySpawnPoints.Add(new Vector2(x, y), tileID);
                            }
                            else
                            {
                                Tile tile = new Tile(textureAtlas)
                                {
                                    Name = GetTileName(tileID),
                                    Type = GetTileType(tileID),
                                    Position = GetTileWorldPositionAt(x, y), // Convert grid position to pixel position
                                    Viewport = GetTileViewport(tileID),
                                    IsSolid = GetTileCollisionType(tileID)
                                };
                                tiles.Add(new Vector2(x, y), tile);
                            }
                        }
                    }
                    y++;
                }
            }

        }

        public static Vector2 GetTileWorldPositionAt(int tileGridX, int tileGridY)
        {
            return new Vector2(tileGridX * Singleton.BLOCK_SIZE, tileGridY * Singleton.BLOCK_SIZE);
        }
        public static Vector2 GetTileWorldPositionAt(Vector2 tileGridPosition)
        {
            return new Vector2(tileGridPosition.X * Singleton.BLOCK_SIZE, tileGridPosition.Y * Singleton.BLOCK_SIZE);
        }

        private static bool IsEnemyTile(int tileID)
        {
            return tileID == 97; //Add more later
        }

        private static string GetTileName(int tileID)
        {
            //TODO Add more detail
            return tileID switch
            {
                _ => "Tile",
            };
        }

        private static TileType GetTileType(int tileID)
        {
            return tileID switch
            {
                17 => TileType.Barrier,
                37 => TileType.Platform,
                57 or 58 or 59 => TileType.Ladder,
                77 or 78 or 79 => TileType.Platform_Ladder,
                _ => TileType.None
            };
        }

        private static bool GetTileCollisionType(int tileID)
        {
            return tileID switch
            {
                17 or 37 => true,
                _ => false
            };
        }

        private Rectangle GetTileViewport(int tileID)
        {
            int tileX = tileID % numTilesPerRow;
            int tileY = tileID / numTilesPerRow;
            
            return new Rectangle(
                tileX * Singleton.BLOCK_SIZE, 
                tileY * Singleton.BLOCK_SIZE, 
                Singleton.BLOCK_SIZE, 
                Singleton.BLOCK_SIZE
            );              
        }

        public bool IsObstacle(Vector2 position)
        {
            foreach (var tile in tiles)
            {
                if (tile.Value.IsSolid && tile.Value.Rectangle.Contains(position))
                {
                    return true; // There's an obstacle at this position
                }
            }
            return false;
        }

        public Tile GetTileAt(int tileX, int tileY)
        {
            Vector2 tileGridPosition = new Vector2(tileX, tileY);
            if(tiles.TryGetValue(tileGridPosition, out Tile value))
                return value;
                
            return null;
        }

        // Fixed version of your GetEnemyLocation method
        public Dictionary<Vector2, int> GetEnemySpawnPoints()
        {
            return enemySpawnPoints;
        }
    }
}
