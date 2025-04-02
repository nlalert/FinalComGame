using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public enum ItemID
{        
    //TODO : Change Item ID to  real id from tilemap
    None = 0,
    HealthPotion = 1000,
    SpeedPotion = 1001,
    JumpPotion = 1002,
    Barrier = 1003,
    LifeUp = 1004,
    SpeedBoots = 1005,
    CursedGauntlet = 1006,
    Sword = 1007,
    Gun = 1008,
    Staff = 1009,
    SoulStaff = 1010,
    Grenade = 1011
}

public class ItemManager
{
    private static Dictionary<ItemID, Item> _itemPrefabs = new Dictionary<ItemID, Item>();

    public static void AddGameItem(ItemID itemID, Item item){
        _itemPrefabs[itemID] = item;
    }

    public static void SpawnItem(ItemID itemID, Vector2 spawnPosition, List<GameObject> gameObjects){
        if (!_itemPrefabs.ContainsKey(itemID))
            return;
            
        Item newItem = _itemPrefabs[itemID].Clone() as Item;
        newItem.Position = spawnPosition;
        newItem.OnSpawn();
        gameObjects.Add(newItem);
    }
    
    // Convert from tile ID to item ID
    public static ItemID GetItemIDFromTileID(int tileID)
    {
        if (Enum.IsDefined(typeof(ItemID), tileID))
            return (ItemID)tileID;
        return ItemID.None;
    }
    
    // For random spawning with the dictionary
    public static void RandomSpawnItem(Dictionary<ItemID, float> itemDropChances, Vector2 spawnPosition, List<GameObject> gameObjects)
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

        // Generate a random value between 0 and totalChance
        float randomValue = (float)(Singleton.Instance.Random.NextDouble() * totalChance);

        // Determine which item to drop based on the random value
        float currentThreshold = 0f;
        ItemID selectedItemID = ItemID.None;

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
        if (selectedItemID != ItemID.None && _itemPrefabs.ContainsKey(selectedItemID))
        {
            SpawnItem(selectedItemID, spawnPosition, gameObjects);
        }
    }
}