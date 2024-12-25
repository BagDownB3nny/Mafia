using UnityEngine;
using Mirror;

public class Door : Shootable
{
    public House house;

    [Server]
    public override void OnShot(NetworkConnectionToClient shooter)
    {
        if (house.isProtected)
        {
            PlayerUIManager.instance.RpcSetTemporaryInteractableText(shooter, "This house is protected!", 1.5f);
            return;
        }
        NetworkServer.Destroy(gameObject);
    }
}
