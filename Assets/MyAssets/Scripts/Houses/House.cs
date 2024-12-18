using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class House : NetworkBehaviour
{
    [SerializeField] private List<Door> doors;


    // This function sets the parent GameObject of all doors to be "Doors"
    // Doors cannot be a child of the house because Mirror does not support nested network objects
    // The house is a network object since it is spawned in by the server, dependent on the number of players
    // The doors are network objects since they require unique network identities, since they can be shot and destroyed
    [Server]
    public void SetupDoorsNetwork(Transform doorsParent)
    {
        foreach (Door door in doors)
        {
            door.AddComponent<NetworkIdentity>();
            door.AddComponent<NetworkTransformUnreliable>();
            door.transform.SetParent(doorsParent);
        }
    }

    public List<Door> GetAllDoors()
    {
        return doors;
    }



    [Server]
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
