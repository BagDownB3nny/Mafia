using UnityEngine;
using Mirror;
using TMPro;

public class PlayerCounter : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnPlayerCountChanged))]
    public string playerCount;

    [SyncVar(hook = nameof(OnPlayerCountColorChanged))]
    public Color playerCountColor;

    [SerializeField] private TMP_Text playerCountText;

    [Server]
    public void SetPlayerCount(string newPlayerCount, Color color)
    {
        playerCount = newPlayerCount;
        playerCountColor = color;

        playerCountText.text = newPlayerCount;
        playerCountText.color = color;
    }

    public void OnPlayerCountChanged(string oldText, string newText)
    {
        playerCountText.text = newText;
    }

    public void OnPlayerCountColorChanged(Color oldColor, Color newColor)
    {
        playerCountText.color = newColor;
    }
}
