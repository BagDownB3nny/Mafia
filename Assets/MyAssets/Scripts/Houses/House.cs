using Mirror;
using UnityEngine;

public class House : NetworkBehaviour
{

    [SyncVar]
    public bool isDoorPresent = true;

    [Server]
    public void DestroyDoor()
    {
        isDoorPresent = false;
    }
}
