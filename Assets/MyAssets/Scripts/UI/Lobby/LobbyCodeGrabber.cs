using UnityEngine;
using TMPro;
public class LobbyCodeGrabber : MonoBehaviour
{

    private string lobbyCode;
    private TMP_Text lobbyCodeText;
    [SerializeField] private GameObject copiedUI;

    public void Awake()
    {
        lobbyCodeText = GetComponent<TMP_Text>();
        if (SteamLobby.instance == null)
        {
            lobbyCodeText.text = "";
            return; // Dev build
        }
        lobbyCode = SteamLobby.instance.LobbyCode;
        lobbyCodeText.text = $"Lobby Code: {lobbyCode}";
    }

    public void CopyCodeToClipboard()
    {
        if (SteamLobby.instance == null)
        {
            Debug.Log("No lobby code to copy");
            return;
        }
        GUIUtility.systemCopyBuffer = lobbyCode;
        ShowCopiedUI();
    }

    private void ShowCopiedUI()
    {
        if (copiedUI != null)
        {
            copiedUI.SetActive(true);
            Invoke(nameof(HideCopiedUI), 2f); // Hide after 2 seconds
        }
    }
    private void HideCopiedUI()
    {
        if (copiedUI != null)
        {
            copiedUI.SetActive(false);
        }
    }
}
