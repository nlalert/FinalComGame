using System;
using System.Collections.Generic;
using System.Linq;
using FinalComGame;

public class Inventory
{
    // Constants for slot types
    public const int MELEE_SLOT = 0;
    public const int RANGE_SLOT = 1;
    public const int ITEM_SLOT_1 = 2;
    public const int ITEM_SLOT_2 = 3;
    public const int MAX_SLOTS = 4;
    
    // Item slots array
    private Item[] _itemSlots;
    
    // List of active consumable items
    private List<Item> _activeConsumables;
    
    public Inventory()
    {
        _itemSlots = new Item[MAX_SLOTS];
        _activeConsumables = new List<Item>();
    }
    
    public void Reset()
    {
        _itemSlots = new Item[MAX_SLOTS];
        _activeConsumables.Clear();
    }
    
    // Get item at specified slot
    public Item GetItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < MAX_SLOTS)
            return _itemSlots[slotIndex];
        
        return null;
    }
    
    // Check if an item slot is empty
    public bool IsSlotEmpty(int slotIndex)
    {
        return GetItem(slotIndex) == null;
    }
    
    // Check if the inventory has an empty item slot (slots 2-3)
    public bool HasEmptyItemSlot()
    {
        return IsSlotEmpty(ITEM_SLOT_1) || IsSlotEmpty(ITEM_SLOT_2);
    }
    
    // Get the first empty item slot (2-3), or -1 if none available
    public int GetFirstEmptyItemSlot()
    {
        if (IsSlotEmpty(ITEM_SLOT_1))
            return ITEM_SLOT_1;
        else if (IsSlotEmpty(ITEM_SLOT_2))
            return ITEM_SLOT_2;
        
        return -1;
    }
    
    // Add an item to the inventory
    public bool AddItem(Item item, int slotIndex = -1)
    {
        // If slot specified, try to add to that slot
        if (slotIndex >= 0 && slotIndex < MAX_SLOTS)
        {
            // For weapon slots, handle replacing the old weapon
            if ((slotIndex == MELEE_SLOT && item.Type == ItemType.MeleeWeapon) ||
                (slotIndex == RANGE_SLOT && item.Type == ItemType.RangeWeapon))
            {
                _itemSlots[slotIndex]?.OnDrop(Singleton.Instance.Player.Position);
                _itemSlots[slotIndex] = item;
                return true;
            }
            // For consumable/item slots, only add if slot is empty
            else if (slotIndex >= ITEM_SLOT_1 && IsSlotEmpty(slotIndex))
            {
                _itemSlots[slotIndex] = item;
                return true;
            }
        }
        // Auto-assign to appropriate slot based on item type
        else
        {
            if (item.Type == ItemType.MeleeWeapon)
            {
                _itemSlots[MELEE_SLOT]?.OnDrop(Singleton.Instance.Player.Position);
                _itemSlots[MELEE_SLOT] = item;
                return true;
            }
            else if (item.Type == ItemType.RangeWeapon)
            {
                _itemSlots[RANGE_SLOT]?.OnDrop(Singleton.Instance.Player.Position);
                _itemSlots[RANGE_SLOT] = item;
                return true;
            }
            else if (item.Type == ItemType.Consumable || item.Type == ItemType.Accessory)
            {
                int emptySlot = GetFirstEmptyItemSlot();
                if (emptySlot != -1)
                {
                    _itemSlots[emptySlot] = item;
                    return true;
                }
            }
        }
        
        return false;
    }
    
    // Remove an item from a slot
    public void RemoveItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < MAX_SLOTS && _itemSlots[slotIndex] != null)
        {
            _itemSlots[slotIndex].IsActive = false;
            _itemSlots[slotIndex] = null;
        }
    }
    
    // Use a consumable item
    public void UseItem(int slotIndex)
    {
        if (slotIndex >= ITEM_SLOT_1 && slotIndex < MAX_SLOTS && 
            _itemSlots[slotIndex] != null && 
            _itemSlots[slotIndex].Type == ItemType.Consumable)
        {
            _itemSlots[slotIndex].Use(slotIndex);
        }
    }
    
    // Add a consumable to the active consumables list and remove from inventory
    public void AddActiveConsumable(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < MAX_SLOTS && _itemSlots[slotIndex] != null)
        {
            _activeConsumables.Add(_itemSlots[slotIndex]);
            _itemSlots[slotIndex] = null;
        }
    }
    
    // Update all active items' passive abilities
    public void UpdateActiveItemPassives(float deltaTime, List<GameObject> gameObjects)
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            if (_itemSlots[i] == null || _itemSlots[i].Type == ItemType.Consumable) 
                continue;

            _itemSlots[i].ActiveAbility(deltaTime, i, gameObjects);
        }
    }
    
    // Update all active consumable items
    public void UpdateActiveConsumables(float deltaTime, List<GameObject> gameObjects)
    {
        for (int i = _activeConsumables.Count - 1; i >= 0; i--)
        {
            if (!_activeConsumables[i].IsActive)
            {
                _activeConsumables.RemoveAt(i);
            }
            else
            {
                _activeConsumables[i].ActiveAbility(deltaTime, i, gameObjects);
            }
        }
    }
    
    // Check for and pick up nearby items
    public void CheckForItemPickup(List<GameObject> gameObjects)
    {
        foreach (var item in gameObjects.OfType<Item>())
        {
            if (item.InPickupRadius() && !item.IsPickedUp)
            {
                if (item.Type == ItemType.MeleeWeapon)
                {
                    AddItem(item, MELEE_SLOT);
                    item.OnPickup(MELEE_SLOT);
                    break;
                }
                else if (item.Type == ItemType.RangeWeapon)
                {
                    AddItem(item, RANGE_SLOT);
                    item.OnPickup(RANGE_SLOT);
                    break;
                }
                else
                {
                    int emptySlot = GetFirstEmptyItemSlot();
                    if (emptySlot != -1)
                    {
                        AddItem(item, emptySlot);
                        item.OnPickup(emptySlot);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Inventory full, cannot pick up " + item.Name);
                    }
                }
            }
        }
    }
    
    // Helper methods for weapon management
    
    public void SetMeleeWeapon(Item weapon)
    {
        if (weapon == null || weapon.Type == ItemType.MeleeWeapon)
        {
            _itemSlots[MELEE_SLOT] = weapon;
        }
    }
    
    public void SetRangeWeapon(Item weapon)
    {
        if (weapon == null || weapon.Type == ItemType.RangeWeapon)
        {
            _itemSlots[RANGE_SLOT] = weapon;
        }
    }
    
    public Sword GetSword()
    {
        return _itemSlots[MELEE_SLOT] as Sword;
    }
    
    public bool HasMeleeWeapon()
    {
        return _itemSlots[MELEE_SLOT] != null;
    }
    
    public bool HasRangeWeapon()
    {
        return _itemSlots[RANGE_SLOT] != null;
    }
    
    public IShootable GetRangeWeapon()
    {
        return _itemSlots[RANGE_SLOT] as IShootable;
    }
    
    // For integrating with existing Player class
    public Item[] GetAllItems()
    {
        return _itemSlots;
    }
}