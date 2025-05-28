using Mirror;

public abstract class Sigil : NetworkBehaviour, IHideable
{
    public abstract LayerName Layer { get; }
    public abstract void Mark(uint netId);
    public abstract void Unmark();
}