using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class TileMap
    {
        public Dictionary<Vector2, Tile> Tiles; //Grid Position, tile
        private Dictionary<Vector2, int> _enemySpawnPoints; // Grid position, enemy type ID
        private Dictionary<Vector2, int> _itemSpawnPoints; // Grid position, enemy type ID

        private Texture2D textureAtlas;
        private int numTilesPerRow;
        public int MapWidth;
        public int MapHeight;

        public TileMap(Texture2D textureAtlas, string mapPath, int numTilesPerRow)
        {
            this.textureAtlas = textureAtlas;
            this.numTilesPerRow = numTilesPerRow;
            Tiles = new Dictionary<Vector2, Tile>();
            _enemySpawnPoints = new Dictionary<Vector2, int>();

            LoadMap(mapPath);
        }

        public void Update(GameTime gameTime, List<GameObject> gameObjects)
        {
            foreach (var tile in Tiles)
            {
                tile.Value.Update(gameTime, gameObjects, this);
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tile in Tiles)
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
                                _enemySpawnPoints.Add(new Vector2(x, y), tileID);
                            }

                            Tile tile = new Tile(textureAtlas)
                            {
                                Name = GetTileName(tileID),
                                Type = type,
                                Position = GetTileWorldPositionAt(x, y), // Convert grid position to pixel position
                                Viewport = GetTileViewport(tileID),
                                IsSolid = GetTileCollisionType(tileID)
                            };
                            Tiles.Add(new Vector2(x, y), tile);
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
                19 => TileType.Platform,
                57 => TileType.Ladder,
                37 => TileType.Ladder_Top,
                58 => TileType.Ladder_Left,
                59 => TileType.Ladder_Right,
                77 or 78 or 79 => TileType.Ladder_Platform,
                18 => TileType.AmbushBarrier,
                39 => TileType.AmbushAreaTopLeft,
                38 => TileType.AmbushAreaBottomRight,
                97 or 98 or 99 or 117 or 118 or 119 or 137 or 138 or 139 or 199 => TileType.EnemySpawn,

                _ => TileType.None
            };
        }

        private static bool GetTileCollisionType(int tileID)
        {
            return tileID switch
            {
                17 or 19 => true,
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
            foreach (var tile in Tiles)
            {
                if (tile.Value.Type == TileType.Barrier && tile.Value.Rectangle.Contains(position))
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
            if(Tiles.TryGetValue(tileGridPosition, out Tile value))
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

        public Dictionary<Vector2, int> GetEnemySpawnPoints()
        {
            return _enemySpawnPoints;
        }

        public Dictionary<Vector2, int> GetItemSpawnPoints()
        {
            return _itemSpawnPoints;
        }

        public List<AmbushArea> GetAmbushAreas(Dictionary<int, BaseEnemy> enemyPrefabs)
        {
            var ambushAreas = new List<AmbushArea>();
            var processedTopLefts = new HashSet<Vector2>(); // To avoid duplicate processing
            
            // First pass: Identify all top-left and bottom-right tiles and store their positions
            Dictionary<Vector2, Tile> topLeftTiles = new Dictionary<Vector2, Tile>();
            Dictionary<Vector2, Tile> bottomRightTiles = new Dictionary<Vector2, Tile>();
            
            foreach (var tile in Tiles)
            {
                if (tile.Value.Type == TileType.AmbushAreaTopLeft)
                {
                    topLeftTiles.Add(tile.Key, tile.Value);
                }
                else if (tile.Value.Type == TileType.AmbushAreaBottomRight)
                {
                    bottomRightTiles.Add(tile.Key, tile.Value);
                }
            }
            
            // Second pass: Match top-left corners with bottom-right corners
            foreach (var topLeftPos in topLeftTiles.Keys)
            {
                if (processedTopLefts.Contains(topLeftPos))
                    continue;
                    
                // Find the closest valid bottom-right corner by checking if it's actually to the bottom-right
                Vector2 bestBottomRight = Vector2.Zero;
                float shortestDistance = float.MaxValue;
                
                foreach (var bottomRightPos in bottomRightTiles.Keys)
                {
                    // Ensure the bottom-right corner is actually to the bottom-right of our top-left
                    if (bottomRightPos.X <= topLeftPos.X || bottomRightPos.Y <= topLeftPos.Y)
                        continue;

                    // Calculate the rectangle area formed by these two corners
                    Rectangle potentialArea = CreateAmbushAreaRectangle(topLeftPos, bottomRightPos);
                    
                    // Check if this is a valid ambush area: all corners must be either our specific corners or empty/non-solid
                    if (!IsValidAmbushArea(potentialArea, topLeftPos, bottomRightPos))
                        continue;
   
                    float distance = Vector2.Distance(topLeftPos, bottomRightPos);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        bestBottomRight = bottomRightPos;
                    }
                }
                
                // If we found a valid bottom-right corner, create the ambush area
                if (bestBottomRight == Vector2.Zero)
                    continue;
                
                Rectangle ambushZone = CreateAmbushAreaRectangle(topLeftPos, bestBottomRight);
                AmbushArea ambushArea = new AmbushArea(ambushZone, this, enemyPrefabs);
                ambushAreas.Add(ambushArea);
                
                // Mark these tiles as processed to avoid duplicate areas
                processedTopLefts.Add(topLeftPos);
            }
            
            return ambushAreas;
        }

        private bool IsValidAmbushArea(Rectangle area, Vector2 topLeftTile, Vector2 bottomRightTile)
        {
            // Get grid coordinates of all four corners
            Vector2 topLeft = GetTileGridPositionAt(new Vector2(area.Left, area.Top));
            Vector2 topRight = GetTileGridPositionAt(new Vector2(area.Right - 1, area.Top));
            Vector2 bottomLeft = GetTileGridPositionAt(new Vector2(area.Left, area.Bottom - 1));
            
            // Check each corner to ensure we have a valid rectangle
            // Top-left must be our marker
            if (topLeft != topLeftTile)
                return false;
                
            // Bottom-right must be our marker
            if (bottomRightTile != GetTileGridPositionAt(new Vector2(area.Right - 1, area.Bottom - 1)))
                return false;
            
            // Check that top-right and bottom-left aren't ambush markers
            Tile topRightTile = GetTileAtGridPosition(topRight);
            if (topRightTile != null && (topRightTile.Type == TileType.AmbushAreaTopLeft || topRightTile.Type == TileType.AmbushAreaBottomRight))
                return false;
                
            Tile bottomLeftTile = GetTileAtGridPosition(bottomLeft);
            if (bottomLeftTile != null && (bottomLeftTile.Type == TileType.AmbushAreaTopLeft || bottomLeftTile.Type == TileType.AmbushAreaBottomRight))
                return false;
            
            // Check interior tiles - shouldn't contain other ambush area markers
            for (int x = (int)topLeft.X + 1; x < (int)bottomRightTile.X; x++)
            {
                for (int y = (int)topLeft.Y + 1; y < (int)bottomRightTile.Y; y++)
                {
                    Tile tile = GetTileAtGridPosition(new Vector2(x, y));
                    if (tile != null && (tile.Type == TileType.AmbushAreaTopLeft || tile.Type == TileType.AmbushAreaBottomRight))
                        return false;
                }
            }
            
            // All checks passed
            return true;
        }

        private Rectangle CreateAmbushAreaRectangle(Vector2 topLeftGridPosition, Vector2 bottomRightGridPosition)
        {
            // Convert grid positions to world positions
            Vector2 topLeftWorld = GetTileWorldPositionAt(topLeftGridPosition);
            Vector2 bottomRightWorld = GetTileWorldPositionAt(bottomRightGridPosition);
            
            // Add tile size to bottom right to include the full tile
            bottomRightWorld.X += Singleton.TILE_SIZE;
            bottomRightWorld.Y += Singleton.TILE_SIZE;

            // Calculate width and height
            int width = (int)(bottomRightWorld.X - topLeftWorld.X);
            int height = (int)(bottomRightWorld.Y - topLeftWorld.Y);

            return new Rectangle(
                (int)topLeftWorld.X, 
                (int)topLeftWorld.Y, 
                width, 
                height
            );
        }
    }
}
