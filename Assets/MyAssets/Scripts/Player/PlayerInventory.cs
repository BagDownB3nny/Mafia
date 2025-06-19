using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    [Header ("Inventory Switching")]
    private Dictionary<KeyCode, int> keycodeToItemIndex = new Dictionary<KeyCode, int>


    // Map keycodes to item indices
    {
        { KeyCode.Alpha1, 0 },
        { KeyCode.Alpha2, 1 },
        { KeyCode.Alpha3, 2 }
    };
    [SyncVar (hook = nameof(OnActiveItemIndexChanged))]
    public int activeItemIndex = 1;
    [SyncVar]
    public InventoryItem activeItem;

    [Header ("Inventory Items")]
    public readonly SyncDictionary<int, InventoryItem> itemsInInventory = new SyncDictionary<int, InventoryItem>();

    [Header ("Inventory items references")]
    [SerializeField] private GameObject inventoryItemsParent;
    public Dictionary<Items, InventoryItem> itemsReferences = new Dictionary<Items, InventoryItem>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdChangeActiveInventorySlot(activeItemIndex);
        SetupInventoryUI();
        AddListenerToInventorySlots();
    }

    [Client]
    public void SetupInventoryUI()
    {
        InventoryUI.instance.SetSelectedSlot(activeItemIndex);
        foreach (var slot in itemsInInventory)
        {
            if (slot.Value != null)
            {
                InventoryUI.instance.SetItemVisual(slot.Key, slot.Value.itemIcon);
            }
        }
    }

    [Client]
    public void AddListenerToInventorySlots()
    {
        itemsInInventory.OnAdd += OnInventoryItemAdded;
        itemsInInventory.OnRemove += OnInventoryItemRemoved;
        itemsInInventory.OnSet += OnInventoryItemSet;
    }

    [Client]
    public void OnInventoryItemAdded(int key)
    {
        Debug.Log("OnInventoryItemAdded: " + key);
        InventoryItem inventoryItem = itemsInInventory[key];
        Sprite itemSprite = inventoryItem.itemIcon;
        InventoryUI.instance.SetItemVisual(key, itemSprite);
    }

    [Client]
    public void OnInventoryItemRemoved(int key, InventoryItem oldItem)
    {
        Debug.Log("OnInventoryItemRemoved: " + key);
        InventoryUI.instance.ClearItemVisual(key);
    }

    [Client]
    public void OnInventoryItemSet(int key, InventoryItem oldItem)
    {
        Debug.Log("OnInventoryItemSet: " + key);
        InventoryItem newItem = itemsInInventory[key];
        Sprite itemSprite = newItem?.itemIcon;
        InventoryUI.instance.SetItemVisual(key, itemSprite);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Fill with 3 empty slots
        for (int i = 0; i < 3; i++)
        {
            itemsInInventory.Add(i, null);
        }
        ServerAddItem(Items.PocketWatch);
    }

    [Client]
    public void Update()
    {
        foreach (var keycode in keycodeToItemIndex.Keys)
        {
            if (Input.GetKeyDown(keycode))
            {
                HandleKeyPress(keycode);
            }
        }
    }

    private InventoryItem GetItemReference(Items item)
    {
        if (itemsReferences.ContainsKey(item))
        {
            return itemsReferences[item];
        }
        else
        {
            foreach (Transform child in inventoryItemsParent.transform)
            {
                InventoryItem inventoryItem = child.GetComponent<InventoryItem>();
                if (inventoryItem != null && inventoryItem.item == item)
                {
                    itemsReferences.Add(item, inventoryItem);
                    return inventoryItem;
                }
            }
        }
        Debug.LogError($"Item reference not found for {item}");
        return null;
    }

    [Client]
    public void HandleKeyPress(KeyCode keycode)
    {
        int newItemIndex = keycodeToItemIndex[keycode];
        if (newItemIndex == activeItemIndex ) return;
        CmdChangeActiveInventorySlot(newItemIndex);
    }

    [Server]
    public void CmdChangeActiveInventorySlot(int slotIndex)
    {
        InventoryItem newActiveItem = itemsInInventory[slotIndex];
        if (activeItem != null)
        {
            activeItem.ServerUnequip();
        }
        activeItemIndex = slotIndex;
        activeItem = newActiveItem;
        if (newActiveItem != null) 
        {
            newActiveItem.ServerEquip();
        }
    }

    [Server]
    public void ServerRemoveItem(Items item)
    {
        // Get index of the slot which holds the item
        foreach (KeyValuePair<int, InventoryItem> slot in itemsInInventory)
        {
            if (slot.Value.item == item)
            {
                itemsInInventory[slot.Key] = null;
                return;
            }
        }
    }

    [Client]
    public void OnActiveItemIndexChanged(int oldValue, int newValue)
    {
        InventoryUI.instance.SetSelectedSlot(newValue);
    }

    [Server]
    public void ServerAddItem(Items item)
    {
        int lowestEmptySlot = GetLowestEmptySlot();
        if (lowestEmptySlot == -1) 
        {
            Debug.LogError("Cannot add item to inventory, no empty slots");
            return;
        }
        InventoryItem inventoryItem = GetItemReference(item);
        itemsInInventory[lowestEmptySlot] = inventoryItem;
    }

    [Server]
    public void ServerAddItem(Items item, int slot)
    {
        if (itemsInInventory[slot] != null)
        {
            Debug.LogError("Cannot add item to inventory, slot is not empty");
            return;
        }
        InventoryItem inventoryItem = GetItemReference(item);
        itemsInInventory[slot] = inventoryItem;
    }

    public int GetLowestEmptySlot()
    {
        for (int i = 0; i < itemsInInventory.Count; i++)
        {
            if (itemsInInventory[i] == null)
            {
                return i;
            }
        }

        // If no empty slots, return -1
        return -1;
    }

    public bool HasItem(Items item)
    {
        bool hasItem = itemsInInventory.Values.Any(inventoryItem => inventoryItem.item == item);
        return hasItem;
    }

    public bool HasInventorySpace()
    {
        return GetLowestEmptySlot() != -1;
    }
}
