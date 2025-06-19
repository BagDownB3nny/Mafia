using UnityEngine;
using UnityEngine.UI;

public class InventoryUISlot : MonoBehaviour
{
    [SerializeField] private GameObject selectedVisual;
    [SerializeField] private Image itemVisual;
    public bool isMafiaSlot = false;
        
    public void SetSelected(bool selected)
    {
        selectedVisual.SetActive(selected);
    }

    public void SetItemVisual(Sprite itemSprite)
    {
        itemVisual.sprite = itemSprite;
        itemVisual.enabled = true;
    }

    public void ClearItemVisual()
    {
        itemVisual.sprite = null;
        itemVisual.enabled = false;
    }

    public void DisableSlot()
    {
        gameObject.SetActive(false);
        // TODO: add a visual effect to indicate that the slot is disabled
    }
}
