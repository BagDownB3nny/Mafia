using UnityEngine;
using Mirror;

public class Door : Shootable
{
    [Server]
    public override void OnShot()
    {
        NetworkServer.Destroy(gameObject);
    }
}
