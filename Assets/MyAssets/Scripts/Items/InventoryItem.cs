using Mirror;
using UnityEngine;



public abstract class InventoryItem : NetworkBehaviour
{
    [SerializeField] public Items item;
    [SerializeField] public Sprite itemIcon;
    [SerializeField] public GameObject remotePlayerItemVisual;

    [SyncVar (hook = nameof(OnIsEquippedChanged))]
    public bool isEquipped = false;

    [Server]
    public void ServerEquip()
    {
        // ItemVisualManager.instance.SetItemVisualActive(item, true);
        Debug.Log("ServerEquip");
        isEquipped = true;
    }

    [Server]
    public void ServerUnequip()
    {
        // ItemVisualManager.instance.SetItemVisualActive(item, false);
        Debug.Log("ServerUnequip");
        isEquipped = false;
    }

    private void OnIsEquippedChanged(bool oldValue, bool newValue)
    {
        if (!isLocalPlayer) {
            remotePlayerItemVisual.SetActive(newValue);
        } else if (isLocalPlayer) {
            ItemVisualManager.instance.SetItemVisualActive(item, newValue);
        }
    }
}
