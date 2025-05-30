using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class EscapeMenuManager : NetworkBehaviour
{

    public static EscapeMenuManager instance;

    [SerializeField] private GameObject escapeMenuPanel;
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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleEscapeMenu();
        }
    }

    public void ToggleEscapeMenu()
    {
        if (escapeMenuPanel.activeSelf)
        {
            CloseEscapeMenu();
        }
        else
        {
            OpenEscapeMenu();
        }
    }

    public void OpenEscapeMenu()
    {
        PlayerCamera.instance.EnterCursorMode();
        PlayerMovement.localInstance.LockPlayerMovement();
        escapeMenuPanel.SetActive(true);
    }

    public void CloseEscapeMenu()
    {
        PlayerCamera.instance.ExitCursorMode();
        PlayerMovement.localInstance.UnlockPlayerMovement();
        escapeMenuPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void OnClickBack()
    {
        CloseEscapeMenu();
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