using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;

    [SerializeField] private InventoryUISlot[] inventorySlots;
    private int selectedSlotIndex = 1;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetSelectedSlot(int slotIndex)
    {
        inventorySlots[selectedSlotIndex].SetSelected(false);
        inventorySlots[slotIndex].SetSelected(true);
        selectedSlotIndex = slotIndex;
    }

    public void SetItemVisual(int slotIndex, Sprite itemSprite)
    {
        if (itemSprite == null)
        {
            inventorySlots[slotIndex].ClearItemVisual();
        } else {
            inventorySlots[slotIndex].SetItemVisual(itemSprite);
        }
    }

    public void ClearItemVisual(int slotIndex)
    {
        inventorySlots[slotIndex].ClearItemVisual();
    }
}
