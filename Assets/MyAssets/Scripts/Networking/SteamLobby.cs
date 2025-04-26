using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;
using System.IO;
public class SteamLobby : MonoBehaviour
{
    [SerializeField] private NetworkManager networkManager;
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    public static SteamLobby instance;

    [SerializeField] public TMPro.TextMeshProUGUI lobbyCodeText;

    public string LobbyCode = "";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        if (networkManager == null) return;

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void SetLobbyCode(string code)
    {
        Debug.Log("Setting lobby code to " + code);
        LobbyCode = code;
    }

    public void HostLobby()
    {
        FileLogger.Log("Host lobby function called");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, networkManager.maxConnections);
    }


    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) return;
        CSteamID lobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        LobbyCode = lobbyId.ToString();
        Debug.Log(lobbyId);
        lobbyCodeText.text = lobbyId.ToString();
        networkManager.StartHost();
        SteamMatchmaking.SetLobbyData(lobbyId, "HostAddress", SteamUser.GetSteamID().ToString());
    }



    public void JoinLobbyByCode()
    {
        // convert the lobby code to a ulong if able
        if (ulong.TryParse(LobbyCode, out ulong code))
        {
            SteamMatchmaking.JoinLobby(new CSteamID(code));
        }
        else
        {
            Debug.LogError("Failed to parse lobby code");
        }
    }

    // Generate a random 6-character lobby code
    private string GenerateLobbyCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] code = new char[6];
        for (int i = 0; i < 6; i++)
        {
            code[i] = chars[Random.Range(0, chars.Length)];
        }
        return new string(code);
    }

    // Converts a Steam ID to a string with 6 characters
    // Each 
    private string SteamIdToCode(CSteamID steamId)
    {
        return steamId.m_SteamID.ToString().Substring(0, 6);
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) return;

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress");
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
    }
}
