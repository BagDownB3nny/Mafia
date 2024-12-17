using Mirror;
using UnityEngine;

public abstract class Shootable : NetworkBehaviour
{

    [Server]
    public virtual void OnShot()
    {
        Debug.Log($"{name} was shot!");
    }
}
