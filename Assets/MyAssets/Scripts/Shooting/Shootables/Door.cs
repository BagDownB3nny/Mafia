using UnityEngine;
using Mirror;

public class Door : Shootable
{
    public House house;

    [Server]
    public override void OnShot()
    {
        if (house.isProtected)
        {
            PlayerUIManager.instance.SetTemporaryInteractableText("This house is protected!", 1.5f);
            return;
        }
        NetworkServer.Destroy(gameObject);
    }
}
