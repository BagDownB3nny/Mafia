using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEndScreen : MonoBehaviour
{
    [SerializeField] private GameObject gameEndScreen;
    [SerializeField] private TMP_Text winText;

    public static GameEndScreen instance;

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

    public void ShowMafiaWin()
    {
        winText.text = "Mafia Wins";
        // Show list of mafia members

        // List<Player> mafiaPlayers = PlayerManager.instance.GetMafiaPlayers();
        // foreach (Player player in mafiaPlayers)
        // {
        //     player.steamUsername
        // }
        gameEndScreen.SetActive(true);
    }

    public void ShowVillagerWin()
    {
        winText.text = "Villagers Win";

        // Show list of villagers

        // List<Player> villagers = PlayerManager.instance.GetAllVillagers();
        // foreach (Player player in villagers)
        // {
        // player.steamUsername
        // }
        gameEndScreen.SetActive(true);
    }
}
