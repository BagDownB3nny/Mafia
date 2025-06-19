using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ItemVisualManager : MonoBehaviour
{
    public Dictionary<Items, GameObject> itemsVisualReferences = new Dictionary<Items, GameObject>();
    [SerializeField] private GameObject itemsVisualParent;

    public static ItemVisualManager instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        foreach (Transform child in itemsVisualParent.transform)
        {
            ItemVisual itemVisual = child.GetComponent<ItemVisual>();
            if (itemVisual != null)
            {
                itemsVisualReferences.Add(itemVisual.item, itemVisual.gameObject);
            }
        }
    }

    public void SetItemVisualActive(Items item, bool isActive)
    {
        if (!itemsVisualReferences.ContainsKey(item)) 
        {
            Debug.LogError($"Item visual reference not found for {item}");
            return;
        }

        itemsVisualReferences[item].SetActive(isActive);
    }
}
