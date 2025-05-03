using Mirror;

public class Door : NetworkBehaviour
{

    public bool isOutsideDoor;
    public House house;

    [SyncVar]
    public bool isKnockedDown;

    [SyncVar]
    public bool isEnabled = true;
}
