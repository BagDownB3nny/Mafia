using Mirror;
using UnityEngine;

public class SceneLight : NetworkBehaviour
{
    [SerializeField] private GameObject objectWithLight;

    public override void OnStartServer()
    {
        LightManager.instance.AddSceneLight(this);
    }

    [ClientRpc]
    public void OnRpc()
    {
        On();
    }

    [ClientRpc]
    public void OffRpc()
    {
        Off();
    }

    [Client]
    public void On()
    {
        objectWithLight.SetActive(true);
    }

    [Client]
    public void Off()
    {
        objectWithLight.SetActive(false);
    }
}
