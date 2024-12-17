using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class House : NetworkBehaviour
{
    [SerializeField] private List<Door> doors;

    public void SetDoorsActive()
    {
        foreach (Door door in doors)
        {
            if (door.gameObject != null)
            {
                door.gameObject.SetActive(true);
            }
        }
    }

    public void SetDoorsInactive()
    {
        foreach (Door door in doors)
        {
            if (door.gameObject != null)
            {
                door.gameObject.SetActive(false);
            }
        }
    }
}
