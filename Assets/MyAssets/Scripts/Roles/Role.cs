using UnityEngine;
using Mirror;
using System.Collections.Generic;

public abstract class Role : NetworkBehaviour
{
    public abstract string rolePlayerInteractText { get; }
    public abstract bool isAbleToInteractWithPlayers { get; }
    protected abstract List<Sigils> sigilsAbleToSee { get; }

    public virtual void InteractWithPlayer(NetworkIdentity player)
    {
        Debug.Log($"Interacting with player {player.name}");
    }

    public virtual void OnEnable()
    {
        if (isLocalPlayer)
        {
            foreach (Sigils sigil in sigilsAbleToSee)
            {
                CameraCullingMaskManager.instance.SetSigilLayerVisible(sigil);
            }
        }
    }

    public virtual void OnDisable()
    {
        if (isLocalPlayer)
        {
            foreach (Sigils sigil in sigilsAbleToSee)
            {
                CameraCullingMaskManager.instance.SetSigilLayerInvisible(sigil);
            }
        }
    }
}
