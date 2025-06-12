using UnityEngine;

public class Whiteboard : MonoBehaviour
{

    [SerializeField] private TMPro.TextMeshProUGUI text;
    public static Whiteboard instance;

    public void Awake()
    {
        instance = this;
    }

    public void SetNewMarkedPlayer(string newPlayer)
    {
        text.text = $"Marked house: \n\n {newPlayer}";
    }

    public void ClearWhiteboard()
    {
        text.text = "Curse a house by using a voodoo dummy";
    }
}
