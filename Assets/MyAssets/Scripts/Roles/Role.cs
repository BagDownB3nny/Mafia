using UnityEngine;
using Mirror;
using System.Collections.Generic;

public abstract class Role : NetworkBehaviour
{
    protected abstract List<SigilName> SigilsAbleToSee { get; }

    [Client]
    public virtual void OnEnable()
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

    protected virtual void SetNameTags() { }

    protected virtual void ResetNameTags() { }
}
