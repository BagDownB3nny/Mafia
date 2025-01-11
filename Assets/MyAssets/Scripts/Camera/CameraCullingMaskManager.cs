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

    public void SetSigilLayerVisible(Sigil sigil)
    {
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer(sigil.ToString());
    }

    public void SetSigilLayerInvisible(Sigil sigil)
    {
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer(sigil.ToString()));
    }

    public void SetAllSigilsVisible()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(Sigil)).Length; i++)
        {
            SetSigilLayerVisible((Sigil)i);
        }
    }

    public void SetAllSigilsInvisible()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(Sigil)).Length; i++)
        {
            SetSigilLayerInvisible((Sigil)i);
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
}
