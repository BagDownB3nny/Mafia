using UnityEngine;
using Mirror;

public class Door : Shootable
{

    [SerializeField] private House house;

    [Command]
    public override void CmdOnShot()
    {
        house.DestroyDoor();
        NetworkServer.Destroy(gameObject);
    }
}
