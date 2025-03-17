using UnityEngine;
using Mirror;
using System.Collections.Generic;

public abstract class Role : NetworkBehaviour
{
    public abstract string RolePlayerInteractText { get; }
    public abstract bool IsAbleToInteractWithPlayers { get; }
    protected abstract List<SigilName> SigilsAbleToSee { get; }

    public virtual void InteractWithPlayer(NetworkIdentity player)
    {
        Debug.Log($"Interacting with player {player.name}");
    }

    [Client]
    protected virtual void OnEnable()
    {
        if (isLocalPlayer)
        {
            EnableSigils();
            SetNameTags();
        }
    }

    [Client]
    protected virtual void OnDisable()
    {
        if (isLocalPlayer)
        {
            DisableSigils();
            ResetNameTags();
        }
    }

    [Client]
    private void EnableSigils()
    {
        foreach (SigilName sigil in SigilsAbleToSee)
        {
            CameraCullingMaskManager.instance.SetSigilLayerVisible(sigil);
        }
    }

    private void DisableSigils()
    {
        foreach (SigilName sigil in SigilsAbleToSee)
        {
            CameraCullingMaskManager.instance.SetSigilLayerInvisible(sigil);
        }
    }

    protected virtual void SetNameTags() {}

    protected virtual void ResetNameTags() {}
}
