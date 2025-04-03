using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public enum ItemID
{        
    None = 0,
    HealthPotion = 14,//consumable
    SpeedPotion = 35,//consumable
    JumpPotion = 34,//consumable
    Barrier = 114,//
    LifeUp = 15,//consumable
    SpeedBoots = 94,//permanent
    CursedGauntlet = 95,//permanent
    Sword = 75,//weapon
    Gun = 55,//weapon
    Staff = 74,//weapon
    SoulStaff = 54,//DONOT DROP
    Grenade = 115,//consumable
    Random = 134
}

public class ItemManager
{
    private static Dictionary<ItemID, Item> _itemPrefabs = new Dictionary<ItemID, Item>();

    public static void AddGameItem(ItemID itemID, Item item){
        _itemPrefabs[itemID] = item;
    }

    public static void SpawnItem(ItemID itemID, Vector2 spawnPosition, List<GameObject> gameObjects){
        if (itemID != ItemID.Random)
        {
            if(!_itemPrefabs.ContainsKey(itemID))
                return;
        }
        else
        {
            // Get all available item types except None, Random, and SoulStaff
            List<ItemID> availableItems = _itemPrefabs.Keys
                .Where(id => id != ItemID.None && id != ItemID.Random && id != ItemID.SoulStaff)
                .ToList();
            
            // Select a random item from the available items
            int randomIndex = Singleton.Instance.Random.Next(availableItems.Count);
            itemID = availableItems[randomIndex];
        }
            
        Item newItem = _itemPrefabs[itemID].Clone() as Item;
        newItem.Position = spawnPosition - new Vector2(Singleton.TILE_SIZE/2,0);
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