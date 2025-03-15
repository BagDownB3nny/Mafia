using UnityEngine;
using Mirror;
using System.Collections.Generic;

public abstract class Role : NetworkBehaviour
{
    public abstract string rolePlayerInteractText { get; }
    public abstract bool isAbleToInteractWithPlayers { get; }
    protected abstract List<SigilName> sigilsAbleToSee { get; }

    public virtual void InteractWithPlayer(NetworkIdentity player)
    {
        Debug.Log($"Interacting with player {player.name}");
    }

    protected virtual void OnEnable()
    {
        if (isLocalPlayer)
        {
            EnableSigils();
            SetNameTags();
        }
    }

    protected virtual void OnDisable()
    {
        if (isLocalPlayer)
        {
            DisableSigils();
            ResetNameTags();
        }
    }

    private void EnableSigils()
    {
        foreach (SigilName sigil in sigilsAbleToSee)
        {
            CameraCullingMaskManager.instance.SetSigilLayerVisible(sigil);
        }
    }

    private void DisableSigils()
    {
        foreach (SigilName sigil in sigilsAbleToSee)
        {
            CameraCullingMaskManager.instance.SetSigilLayerInvisible(sigil);
        }
    }

    protected abstract void SetNameTags();
    protected abstract void ResetNameTags();
}
