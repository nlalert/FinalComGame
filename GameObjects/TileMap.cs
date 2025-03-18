using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame
{
    public class TileMap
    {
        public List<Tile> tiles; 
        private Texture2D textureAtlas;
        private int numTilesPerRow;

        public TileMap(Texture2D textureAtlas, string mapPath, int numTilesPerRow)
        {
            this.textureAtlas = textureAtlas;
            this.numTilesPerRow = numTilesPerRow;
            tiles = LoadMap(mapPath);
        }

        private List<Tile> LoadMap(string filepath)
        {
            List<Tile>  result = new List<Tile>();

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
                            int tileX = tileID % numTilesPerRow;
                            int tileY = tileID / numTilesPerRow;
                            bool collision = false;

                            if (tileID == 17 || tileID == 37)
                            {
                                collision = true;
                            }

                            Tile tile = new Tile(textureAtlas)
                            {
                                Name = "Tile",
                                Position = new Vector2(x * Singleton.BLOCK_SIZE, y * Singleton.BLOCK_SIZE), // Convert grid position to pixel position
                                Viewport = new Rectangle(
                                    tileX * Singleton.BLOCK_SIZE, 
                                    tileY * Singleton.BLOCK_SIZE, 
                                    Singleton.BLOCK_SIZE, 
                                    Singleton.BLOCK_SIZE
                                ),                            
                                IsSolid = collision

                            };

                            result.Add(tile);
                        }
                    }
                    y++;
                }
            }

            return result;
        }

        public void Update(GameTime gameTime, List<GameObject> gameObjects)
        {
            foreach (Tile tile in tiles)
            {
                tile.Update(gameTime, gameObjects, this);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile tile in tiles)
            {
                tile.Draw(spriteBatch);
            }
        }
    }
}
