using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class ItemManager
{
    private static Dictionary<int, Item> _itemPrefabs = new Dictionary<int, Item>();

    public static void AddGameItem(int itemID, Item item){
        _itemPrefabs.TryAdd(itemID, item);
    }

    public static void SpawnItem(int itemID, Vector2 spawnPosition, List<GameObject> gameObjects){
        Item newItem = _itemPrefabs[itemID].Clone() as Item;
        newItem.Position = spawnPosition;
        newItem.OnSpawn();
        gameObjects.Add(newItem);
    }
}