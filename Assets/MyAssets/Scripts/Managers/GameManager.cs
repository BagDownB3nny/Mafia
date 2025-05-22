using UnityEngine;
using Mirror;


// Methods:
// Assign roles to players
public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

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
    public void StartGame()
    {
        Debug.Log("Starting game");
        PlayerManager.instance.AssignRoles();
        MafiaHouse.instance.AssignMafiaDoorAuthority();
        VotingBooth.instance.InitialiseDictionaries();
        VotingManager.instance.StopVoting();
        TimeManagerV2.instance.StartGame();
        DayCycleManager.instance.StartGame();
        GameEndManager.instance.StartGame();
    }
}
