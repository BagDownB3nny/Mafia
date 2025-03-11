using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class LightManager : NetworkBehaviour
{
    public static LightManager instance;
    private List<SceneLight> sceneLights = new List<SceneLight>();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Server]
    public void AddSceneLight(SceneLight sceneLight)
    {
        sceneLights.Add(sceneLight);
    }

    [Server]
    public void TurnOnAllLights()
    {
        foreach (SceneLight sceneLight in sceneLights)
        {
            sceneLight.OnRpc();
        }
    }

    [Server]
    public void TurnOffAllLights()
    {
        foreach (SceneLight sceneLight in sceneLights)
        {
            sceneLight.OffRpc();
        }
    }
}
