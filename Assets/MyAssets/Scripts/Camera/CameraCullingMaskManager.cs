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

    public void SetSigilLayerVisible(SigilName sigil)
    {
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer(sigil.ToString());
    }

    public void SetSigilLayerInvisible(SigilName sigil)
    {
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer(sigil.ToString()));
    }

    public void SetAllSigilsVisible()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(SigilName)).Length; i++)
        {
            SetSigilLayerVisible((SigilName)i);
        }
    }

    public void SetAllSigilsInvisible()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(SigilName)).Length; i++)
        {
            SetSigilLayerInvisible((SigilName)i);
        }
    }

    public void SetGhostLayerVisible()
    {
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Ghost");
    }

    public void SetGhostLayerInvisible()
    {
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Ghost"));
    }

    public void SetNameTagLayerVisible()
    {
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("NameTag");
        Debug.Log("NameTag layer visible");
    }

    public void SetNameTagLayerInvisible()
    {
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("NameTag"));
    }
}
