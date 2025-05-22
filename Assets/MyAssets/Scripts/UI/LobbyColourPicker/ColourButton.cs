using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ColourButton : MonoBehaviour
{
    public Color color;

    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void SetColour(Color newColor)
    {
        color = newColor;
        GetComponent<Image>().color = color;
    }

    [Client]
    public void OnClick()
    {
        Player player = PlayerManager.instance.localPlayer;
        player.GetComponent<PlayerColour>().CmdSetColour(color);
    }
}
