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

    public static void RandomSpawnItem(Dictionary<int, float> itemDropChances, Vector2 spawnPosition, List<GameObject> gameObjects)
    {
        // Check if we have any items to spawn
        if (itemDropChances == null || itemDropChances.Count == 0)
            return;

        // Calculate total drop chance
        float totalChance = 0f;
        foreach (var chance in itemDropChances.Values)
        {
            totalChance += chance;
        }

        float randomValue = (float)(Singleton.Instance.Random.NextDouble() * totalChance);

        // Determine which item to drop based on the random value
        float currentThreshold = 0f;
        int selectedItemID = -1;

        foreach (var kvp in itemDropChances)
        {
            currentThreshold += kvp.Value;
            
            // If random value is less than current threshold, select this item
            if (randomValue <= currentThreshold)
            {
                selectedItemID = kvp.Key;
                break;
            }
        }

        // If an item was selected and exists in prefabs, spawn it
        if (selectedItemID != -1 && _itemPrefabs.ContainsKey(selectedItemID))
        {
            SpawnItem(selectedItemID, spawnPosition, gameObjects);
        }
    }
}