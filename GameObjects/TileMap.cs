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
        private Texture2D textureAtlas;
        private int numTilesPerRow;

        public TileMap(Texture2D textureAtlas, string mapPath, int numTilesPerRow)
        {
            this.textureAtlas = textureAtlas;
            this.numTilesPerRow = numTilesPerRow;
            tiles = LoadMap(mapPath);
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

        private Dictionary<Vector2, Tile> LoadMap(string filepath)
        {
            Dictionary<Vector2, Tile> result = new Dictionary<Vector2, Tile>();

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
                            Tile tile = new Tile(textureAtlas)
                            {
                                Name = GetTileName(tileID),
                                Type = GetTileType(tileID),
                                Position = GetTileWorldPosition(x, y), // Convert grid position to pixel position
                                Viewport = GetTileViewport(tileID),
                                IsSolid = GetTileCollisionType(tileID)
                            };
                            result.Add(new Vector2(x, y), tile);
                        }
                    }
                    y++;
                }
            }

            return result;
        }

        private static Vector2 GetTileWorldPosition(int x, int y)
        {
            return new Vector2(x * Singleton.BLOCK_SIZE, y * Singleton.BLOCK_SIZE);
        }

        private static string GetTileName(int tileID)
        {
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
    }
}
