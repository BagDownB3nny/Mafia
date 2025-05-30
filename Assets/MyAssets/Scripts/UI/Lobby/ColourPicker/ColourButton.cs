using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ColourButton : MonoBehaviour
{

    public GameObject selectedColourImage;
    public GameObject unselectableColourImage;
    public Color colour;

    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void SetColour(Color newColor)
    {
        colour = newColor;
        GetComponent<Image>().color = colour;
    }

    [Client]
    public void OnClick()
    {
        if (unselectableColourImage.activeSelf)
        {
            return;
        }
        Player player = PlayerManager.instance.localPlayer;
        player.GetComponent<PlayerColour>().CmdSetColour(colour);
    }
}
