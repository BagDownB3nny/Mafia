using UnityEngine;
using Mirror;

public class Door : Shootable
{

    [SerializeField] private House house;

    [Server]
    public override void OnShot()
    {
        house.DestroyDoor();
        NetworkServer.Destroy(gameObject);
    }
}
