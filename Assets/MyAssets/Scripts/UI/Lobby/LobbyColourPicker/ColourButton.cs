using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ColourButton : MonoBehaviour
{

    [SerializeField] public GameObject selectedColourImage;
    [SerializeField] public GameObject unselectableColourImage;
    [SerializeField] private ColourPickerUI colourPickerUI;
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
        Player player = NetworkClient.localPlayer.GetComponent<Player>();
        player.GetComponent<PlayerColour>().CmdSetColour(colour);
    }
}
