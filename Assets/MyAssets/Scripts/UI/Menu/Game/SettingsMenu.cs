using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class SettingsMenu : Menu
{

    public static SettingsMenu instance;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject quitToLobbyButton;
    [SerializeField] private GameObject leaveGameButton;

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

    public override void OnStartServer()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (isServer && sceneName == "Game")
        {
            quitToLobbyButton.SetActive(true);
            leaveGameButton.SetActive(false);
        }
    }

    public override void Open()
    {
        PlayerMovement.localInstance.LockPlayerMovement();
        base.Open();
    }

    public override void Close()
    {
        Debug.Log("Closing SettingsMenu");
        PlayerMovement.localInstance.UnlockPlayerMovement();
        base.Close();
    }

    public void OnClickBack()
    {
        Close();
    }

    [Client]
    public void OnClickLeaveGame()
    {
        NetworkClient.Disconnect();
    }

    [Server]
    public void OnClickExitToLobby()
    {
        GameEndManager.instance.EndGame();
    }
}