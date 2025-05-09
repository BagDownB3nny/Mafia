using UnityEngine;
using Mirror;
using System.Collections.Generic;

public abstract class Role : NetworkBehaviour
{
    [Header("Role Settings")]
    public abstract string RolePlayerInteractText { get; }
    public abstract bool IsAbleToInteractWithPlayers { get; }

    public abstract string InteractWithDoorText { get; }
    public abstract bool IsAbleToInteractWithDoors { get; }
    protected abstract List<SigilName> SigilsAbleToSee { get; }

    public abstract void InteractWithPlayer(NetworkIdentity player);

    public virtual void InteractWithHouse(NetworkIdentity houseNetId)
    {
        Debug.Log($"Interacting with house {houseNetId.name}");
    }

    [Client]
    public virtual void OnEnable()
    {
        if (isLocalPlayer && isOwned)
        {
            Debug.Log($"Enabling sigils for role {this.name}");
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

    protected virtual void SetNameTags() { }

    protected virtual void ResetNameTags() { }
}
