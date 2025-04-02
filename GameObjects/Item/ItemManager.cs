using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FinalComGame;

public class ItemManager
{
    private static Dictionary<string, Item> _itemList = new Dictionary<string, Item>();

    public static void AddGameItem(string itemName, Item item){
        _itemList.Add(itemName, item);
    }

    public static void SpawnItem(string itemName, float x, float y, List<GameObject> gameObjects){
        Item newItem = _itemList[itemName].Clone() as Item;
        newItem.Position = new Vector2(x, y);
        gameObjects.Add(newItem);
    }
       
}