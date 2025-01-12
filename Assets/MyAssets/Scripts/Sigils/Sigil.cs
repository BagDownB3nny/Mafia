using Mirror;
using UnityEngine;

public abstract class Sigil : NetworkBehaviour
{
    public abstract void Mark(uint playerNetId);
    public abstract void Unmark();
}