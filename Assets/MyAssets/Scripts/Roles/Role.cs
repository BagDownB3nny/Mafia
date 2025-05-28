using Mirror;
using System.Collections.Generic;

public abstract class Role : NetworkBehaviour
{
    protected abstract List<LayerName> LayersAbleToSee { get; }

    [Client]
    public virtual void OnEnable()
    {
        if (isLocalPlayer)
        {
            ShowHideables();
            SetNameTags();
        }
    }

    [Client]
    protected virtual void OnDisable()
    {
        if (isLocalPlayer)
        {
            HideHideables();
            ResetNameTags();
        }
    }

    [Client]
    private void ShowHideables()
    {
        foreach (LayerName layerName in LayersAbleToSee)
        {
            CameraCullingMaskManager.instance.SetLayerVisible(layerName);
        }
    }

    private void HideHideables()
    {
        foreach (LayerName layerName  in LayersAbleToSee)
        {
            CameraCullingMaskManager.instance.SetLayerInvisible(layerName);
        }
    }

    protected virtual void SetNameTags() { }

    protected virtual void ResetNameTags() { }
}
