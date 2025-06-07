using Mirror;
using UnityEngine;

public class Door : NetworkBehaviour
{

    public bool isOutsideDoor;
    public House house;

    [SyncVar]
    public bool isKnockedDown;

    [SyncVar]
    public bool isEnabled = true;

    [SerializeField] public bool isKnockable = true;
}
