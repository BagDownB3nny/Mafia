using Mirror;
using UnityEngine;

public abstract class Shootable : NetworkBehaviour
{
    [Command]
    public virtual void CmdOnShot()
    {
        Debug.Log($"{name} was shot!");
    }
}
