using System;
using System.Collections.Generic;
using System.IO;
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
        public int MapWidth;
        public int MapHeight;

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

                    MapWidth = items.Length;

                    for (int x = 0; x < MapWidth; x++)
                    {
                        if (int.TryParse(items[x], out int tileID) && tileID >= 0)
                        {
                            TileType type = GetTileType(tileID);
                            if(type == TileType.EnemySpawn)
                            {
                                // Store enemy spawn point with its type
                                enemySpawnPoints.Add(new Vector2(x, y), tileID);
                            }

                            Tile tile = new Tile(textureAtlas)
                            {
                                Name = GetTileName(tileID),
                                Type = type,
                                Position = GetTileWorldPositionAt(x, y), // Convert grid position to pixel position
                                Viewport = GetTileViewport(tileID),
                                IsSolid = GetTileCollisionType(tileID)
                            };
                            tiles.Add(new Vector2(x, y), tile);
                        }
                    }
                    y++;
                }
                MapHeight = y;
            }
        }

        public static Vector2 GetTileWorldPositionAt(int tileGridX, int tileGridY)
        {
            return new Vector2(tileGridX * Singleton.TILE_SIZE, tileGridY * Singleton.TILE_SIZE);
        }
        public static Vector2 GetTileWorldPositionAt(Vector2 tileGridPosition)
        {
            return new Vector2(tileGridPosition.X * Singleton.TILE_SIZE, tileGridPosition.Y * Singleton.TILE_SIZE);
        }
        public static Vector2 GetTileGridPositionAt(int x, int y)
        {
            return new Vector2(x / Singleton.TILE_SIZE, y / Singleton.TILE_SIZE); // Grid Position ust be int
        }
        public static Vector2 GetTileGridPositionAt(Vector2 tileWorldPosition)
        {
            return new Vector2((int) tileWorldPosition.X / Singleton.TILE_SIZE, (int) tileWorldPosition.Y / Singleton.TILE_SIZE); // Grid Position ust be int
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
                57 => TileType.Ladder,
                58 => TileType.Ladder_Left,
                59 => TileType.Ladder_Right,
                77 or 78 or 79 => TileType.Ladder_Platform,
                18 => TileType.Ladder_Top,
                14 or 74 => TileType.AmbushBarrier,
                34 => TileType.AmbushAreaTopLeft,
                54 => TileType.AmbushAreaBottomRight,
                97 or 98 or 99 or 117 or 118 or 119 or 137 or 138 or 139 => TileType.EnemySpawn,

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
                tileX * Singleton.TILE_SIZE, 
                tileY * Singleton.TILE_SIZE, 
                Singleton.TILE_SIZE, 
                Singleton.TILE_SIZE
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

        public Tile GetTileAtGetTileAtGridPosition(int tileX, int tileY)
        {
            Vector2 tileGridPosition = new Vector2(tileX, tileY);
            return GetTileAtGridPosition(tileGridPosition);
        }

        public Tile GetTileAtGridPosition(Vector2 tileGridPosition)
        {
            if(tiles.TryGetValue(tileGridPosition, out Tile value))
                return value;
                
            return null;
        }
        public Tile GetTileAtWorldPostion(Vector2 worldPostion)
        {
            return GetTileAtGridPosition(GetTileGridPositionAt(worldPostion));
        }

        public Tile GetTileAtWorldPostion(int x, int y)
        {
            Vector2 worldPostion = new Vector2(x, y);
            return GetTileAtGridPosition(GetTileGridPositionAt(worldPostion));
        }

        // Fixed version of your GetEnemyLocation method
        public Dictionary<Vector2, int> GetEnemySpawnPoints()
        {
            return enemySpawnPoints;
        }

        public List<AmbushArea> GetAmbushAreas(Dictionary<int, BaseEnemy> enemyPrefabs)
        {
            var ambushAreas = new List<AmbushArea>();
            var topLeftTiles = new List<Vector2>();

            // First, find all top-left ambush area tiles
            foreach (var tile in tiles)
            {
                if (tile.Value.Type == TileType.AmbushAreaTopLeft)
                {
                    topLeftTiles.Add(tile.Key);
                }
            }

            // For each top-left tile, find corresponding bottom-right tile
            foreach (Vector2 topLeft in topLeftTiles)
            {
                Vector2 bottomRight = FindCorrespondingBottomRightTile(topLeft);
                
                if (bottomRight != Vector2.Zero)
                {
                    // Create rectangle in world coordinates
                    Rectangle ambushZone = CreateAmbushAreaRectangle(topLeft, bottomRight);
                    
                    // Create AmbushArea
                    AmbushArea ambushArea = new AmbushArea(ambushZone, this, enemyPrefabs);
                    ambushAreas.Add(ambushArea);
                }
            }

            return ambushAreas;
    
        }

        private Vector2 FindCorrespondingBottomRightTile(Vector2 topLeft)
        {
            // Search for the corresponding bottom-right tile within a reasonable range
            for (int x = (int)topLeft.X; x < MapWidth; x++) // Limit search range
            {
                for (int y = (int)topLeft.Y; y < MapHeight; y++) // Limit search range
                {
                    Vector2 bottomRight = new Vector2(x, y);
                    Tile tile = GetTileAtGridPosition(bottomRight);
                    // Check if this tile is a bottom-right ambush area marker
                    if (tile != null && tile.Type == TileType.AmbushAreaBottomRight)
                    {
                        return bottomRight;
                    }
                }
            }
            return Vector2.Zero;
        }

        private Rectangle CreateAmbushAreaRectangle(Vector2 topLeftGridPosition, Vector2 bottomRightGridPosition)
        {
            // Convert grid positions to world positions
            Vector2 topLeftWorld = GetTileWorldPositionAt(topLeftGridPosition);
            Vector2 bottomRightWorld = GetTileWorldPositionAt(bottomRightGridPosition);

            // Calculate width and height
            int width = (int)(bottomRightWorld.X - topLeftWorld.X + Singleton.TILE_SIZE);
            int height = (int)(bottomRightWorld.Y - topLeftWorld.Y + Singleton.TILE_SIZE);

            return new Rectangle(
                (int)topLeftWorld.X, 
                (int)topLeftWorld.Y, 
                width, 
                height
            );
        }
    }
}
