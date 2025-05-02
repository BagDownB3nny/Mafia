using UnityEngine;

public class SpectatorSpawn : MonoBehaviour
{
    public static Transform spectatorSpawnpoint;

    private void Start()
    {
        spectatorSpawnpoint = transform;
    }
}
