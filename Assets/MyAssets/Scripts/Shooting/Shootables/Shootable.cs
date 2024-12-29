using Mirror;
using UnityEngine;

public abstract class Shootable : NetworkBehaviour
{

    [Server]
    public virtual void OnShot(NetworkConnectionToClient shooter)
    {
        Debug.Log($"{name} was shot!");
    }
}

