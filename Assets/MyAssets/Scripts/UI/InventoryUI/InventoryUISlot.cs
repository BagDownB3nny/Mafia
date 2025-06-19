using UnityEngine;
using UnityEngine.UI;

public class InventoryUISlot : MonoBehaviour
{
    [SerializeField] private GameObject selectedVisual;
    [SerializeField] private Image itemVisual;
        
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
}
