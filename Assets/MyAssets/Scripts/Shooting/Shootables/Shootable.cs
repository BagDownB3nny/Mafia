using Mirror;
using UnityEngine;

public abstract class Shootable : NetworkBehaviour
{

    [Server]
    public virtual bool OnShot(NetworkConnectionToClient shooter)
    {
        return true;
    }
}

