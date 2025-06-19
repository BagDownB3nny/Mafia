using UnityEngine;

public class InventoryAxe : InventoryItem
{
    public void Update()
    {
        if (isEquipped && Input.GetKeyDown(KeyCode.Mouse1))
        {
            Debug.Log("Axe swing");
        }
    }
}
