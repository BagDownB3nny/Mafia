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
        PlayerManager.instance.AssignRoles();
        MafiaHouse.instance.AssignMafiaSpawn();
        HouseManager.instance.OpenAllDoors();
        VotingBooth.instance.InitialiseDictionaries();
        VotingManager.instance.StopVoting();
        // TimeManager.instance.StartFirstDay();
        TimeManagerV2.instance.StartGame();
    }
}
