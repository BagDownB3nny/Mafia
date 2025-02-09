using System.Collections.Generic;
using UnityEngine;

public class MafiaHouse : MonoBehaviour
{

    [SerializeField] private List<Transform> mafiaSpawnPoints;

    public static MafiaHouse instance;

    private void Awake()
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

    public List<Transform> GetMafiaSpawnPoints()
    {
        return mafiaSpawnPoints;
    }

    public void AssignMafiaSpawn()
    {
        List<Player> mafiaPlayers = PlayerManager.instance.GetMafiaPlayers();
        // This assumes that 
        for (int i = 0; i < mafiaPlayers.Count; i++)
        {
            mafiaPlayers[i].GetComponent<PlayerTeleporter>().mafiaNightSpawnpoint = mafiaSpawnPoints[i];
        }
    }
}
