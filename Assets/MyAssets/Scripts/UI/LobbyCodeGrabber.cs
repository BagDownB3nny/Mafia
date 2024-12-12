using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
public class LobbyCodeGrabber : MonoBehaviour
{

    private TMP_Text lobbyCodeText;

    public void Awake()
    {
        lobbyCodeText = GetComponent<TMP_Text>();
        if (SteamLobby.instance == null)
        {
            lobbyCodeText.text = "";
            return; // Dev build
        }
        lobbyCodeText.text = $"Lobby Id: {SteamLobby.instance.LobbyCode}";
    }
}
