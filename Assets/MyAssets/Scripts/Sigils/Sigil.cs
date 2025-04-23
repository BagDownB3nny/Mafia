using Mirror;
using UnityEngine;

public abstract class Sigil : NetworkBehaviour
{
    public abstract void Mark(uint netId);
    public abstract void Unmark();
}