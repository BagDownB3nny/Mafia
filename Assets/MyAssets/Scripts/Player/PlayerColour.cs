using Mirror;
using UnityEngine;

public class PlayerColour : NetworkBehaviour
{

    [Header("References")]
    public GameObject playerModel;


    [SyncVar(hook = nameof(OnColorChanged))]
    public Color originalPlayerColour;


    public override void OnStartServer()
    {
        int connectionId = connectionToClient.connectionId;
        Color color = PlayerColourManager.instance.GetColour(connectionId);

        // Im using white as a null value since I cant return null
        if (color == Color.white) return;

        originalPlayerColour = color;
    }

    [Server]
    public void SetColor(Color newColor)
    {
        originalPlayerColour = newColor;

        Player player = GetComponent<Player>();
        PlayerColourManager.instance.OnPlayerChangedColour(player, newColor);
    }

    [Command]
    public void CmdSetColour(Color newColour)
    {
        SetColor(newColour);
    }

    [Client]
    public void OnColorChanged(Color oldColour, Color newColour)
    {
        if (playerModel != null)
        {
            playerModel.GetComponent<Renderer>().material.color = newColour;
        }
        if (isLocalPlayer && FakePlayerModel.instance != null)
        {
            FakePlayerModel.instance.SetColour(newColour);
        }
    }
}
