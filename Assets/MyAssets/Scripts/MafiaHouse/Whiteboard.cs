using UnityEngine;

public class Whiteboard : MonoBehaviour
{

    [SerializeField] private TMPro.TextMeshProUGUI text;

    public void SetNewMarkedPlayer(string newPlayer)
    {
        text.text = $"Marked house: \n\n {newPlayer}";
        Debug.Log("Whiteboard change");
    }

    public void ClearWhiteboard()
    {
        text.text = "Marked house: \n\n None";
        Debug.Log("Whiteboard change");
    }
}
