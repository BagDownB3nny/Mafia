using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MafiaHouse : MonoBehaviour
{

    // [SerializeField] private List<Transform> mafiaSpawnPoints;
    [SerializeField] private List<Door> doors;

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

    [Server]
    public void AssignMafiaDoorAuthority()
    {
        List<Player> mafiaPlayers = PlayerManager.instance.GetMafiaPlayers();
        foreach (Player player in mafiaPlayers)
        {
            foreach (Door door in doors)
            {
                InteractableDoor interactableDoor = door.GetComponent<InteractableDoor>();
                interactableDoor.AssignAuthority(player);
            }
        }
    }

    // public List<Transform> GetMafiaSpawnPoints()
    // {
    //     return mafiaSpawnPoints;
    // }

    // public void AssignMafiaSpawn()
    // {
    //     List<Player> mafiaPlayers = PlayerManager.instance.GetMafiaPlayers();
    //     // This assumes that 
    //     for (int i = 0; i < mafiaPlayers.Count; i++)
    //     {
    //         mafiaPlayers[i].GetComponent<PlayerTeleporter>().mafiaNightSpawnpoint = mafiaSpawnPoints[i];
    //     }
    // }
}
