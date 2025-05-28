using UnityEngine;

public class CameraCullingMaskManager : MonoBehaviour
{

    public static CameraCullingMaskManager instance;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("More than one CameraCullingMaskManager in the scene");
        }
    }

    public void SetLayerVisible(LayerName layerName)
    {
        Camera.main.cullingMask |= layerName.Mask();
    }

    public void SetLayerInvisible(LayerName layerName)
    {
        Camera.main.cullingMask &= ~layerName.Mask();
    }

    public void SetGhostLayerVisible()
    {
        SetLayerVisible(LayerName.Ghost);
    }

    public void SetGhostLayerInvisible()
    {
        SetLayerInvisible(LayerName.Ghost);
    }

    public void SetNameTagLayerVisible()
    {
        SetLayerVisible(LayerName.NameTag);
    }

    public void SetNameTagLayerInvisible()
    {
        SetLayerInvisible(LayerName.NameTag);
    }
}
