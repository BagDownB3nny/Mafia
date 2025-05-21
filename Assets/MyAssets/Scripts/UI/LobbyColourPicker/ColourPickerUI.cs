using UnityEngine;

public class ColourPickerUI : MonoBehaviour
{
    [SerializeField] private GameObject colourButtons;

    public void Start()
    {
        Color[] colors = PlayerColourManager.allColours;
        int colorIndex = 0;
        int childCount = colourButtons.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = colourButtons.transform.GetChild(i);

            // Get the ColourButton component
            ColourButton button = child.GetComponent<ColourButton>();
            if (button != null && colorIndex < colors.Length)
            {
                // Set the color and increment index
                button.SetColour(colors[colorIndex]);
                colorIndex++;
            }
            else if (colorIndex >= colors.Length)
            {
                Debug.LogWarning("Not enough colors for all buttons");
                break;
            }
        }
    }
}
