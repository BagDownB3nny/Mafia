using Mirror;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    public GameObject activeItem;

    public int activeItemIndex = 0;
    public GameObject[] items;

    [Client]
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleInventorySlot(0);
        }
    }

    public Item GetActiveItemScript()
    {
        return activeItem?.GetComponent<Item>();
    }

    public void ToggleInventorySlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Length)
        {
            Debug.LogError("Invalid slot index");
            return;
        }
        GameObject newItem = items[slotIndex];
        bool isNewItemEqualActiveItem = activeItem == newItem;
        activeItem.SetActive(false);
        activeItem = null;

        // Only enable item if it's not the same as the current active item
        if (!isNewItemEqualActiveItem)
        {
            activeItem = newItem;
            activeItem.SetActive(true);
        }
    }
}
