using Mirror;
using UnityEngine;

public class ColourPickerUI : MonoBehaviour
{
    [SerializeField] private GameObject colourButtons;
    [SerializeField] private GameObject colourWindow;
    private bool ColourButtonUIsInitialised = false;

    public static ColourPickerUI instance;

    public void Start()
    {
        SubscribeToPlayerColourManager();
        InitialiseColourButtons();
    }

    public void InitialiseColourButtons()
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
                Debug.LogError("Not enough colors for all buttons");
                break;
            }
        }
    }

    public void SetUIToColourButtons()
    {
        // Set other buttons to unselectable
        int localPlayerConnId = PlayerManager.instance.LocalPlayerConnId();

        foreach (var keyValuePair in PlayerColourManager.instance.playerColours)
        {
            ColourButton button = GetColourButton(keyValuePair.Value);
            if (keyValuePair.Key != localPlayerConnId)
            {
                button.unselectableColourImage.SetActive(true);
            }
            else if (keyValuePair.Key == localPlayerConnId)
            {
                button.selectedColourImage.SetActive(true);
            }
        }
    }

    public void OnPlayerColourRemoved(int playerConnId, Color oldColour)
    {
        ColourButton button = GetColourButton(oldColour);
        if (button != null)
        {
            button.unselectableColourImage.SetActive(false);
        }
    }

    public void EnterWindow()
    {
        if (!ColourButtonUIsInitialised)
        {
            SetUIToColourButtons();
            ColourButtonUIsInitialised = true;
        }

        colourWindow.SetActive(true);
        PlayerCamera.instance.EnterCursorMode();
    }

    public void ExitWindow()
    {
        colourWindow.SetActive(false);
        PlayerCamera.instance.ExitCursorMode();
    }

    public void SubscribeToPlayerColourManager()
    {
        PlayerColourManager.instance.playerColours.OnAdd += OnPlayerColourAdded;
        PlayerColourManager.instance.playerColours.OnSet += OnPlayerColourChanged;
        PlayerColourManager.instance.playerColours.OnRemove += OnPlayerColourRemoved;
    }


    private void OnPlayerColourAdded(int playerConnId)
    {
        Color newColour = PlayerColourManager.instance.playerColours[playerConnId];
        int localPlayerConnId = PlayerManager.instance.LocalPlayerConnId();
        ColourButton button = GetColourButton(newColour);
        if (button == null) return;

        if (localPlayerConnId == playerConnId)
        {
            button.selectedColourImage.SetActive(true);
        }
        else
        {
            button.selectedColourImage.SetActive(false);
            button.unselectableColourImage.SetActive(true);
        }
    }

    public void OnPlayerColourChanged(int playerConnId, Color oldColour)
    {
        Color newColour = PlayerColourManager.instance.playerColours[playerConnId];
        ColourButton oldButton = GetColourButton(oldColour);
        ColourButton newButton = GetColourButton(newColour);
        int localPlayerConnId = PlayerManager.instance.LocalPlayerConnId();

        if (localPlayerConnId == playerConnId)
        {
            oldButton.selectedColourImage.SetActive(false);
            oldButton.unselectableColourImage.SetActive(false);
            newButton.selectedColourImage.SetActive(true);
            newButton.unselectableColourImage.SetActive(false);
        }
        else
        {
            oldButton.selectedColourImage.SetActive(false);
            oldButton.unselectableColourImage.SetActive(false);
            newButton.selectedColourImage.SetActive(false);
            newButton.unselectableColourImage.SetActive(true);
        }
    }

    public ColourButton GetColourButton(Color color)
    {
        foreach (Transform child in colourButtons.transform)
        {
            ColourButton button = child.GetComponent<ColourButton>();
            if (button != null && button.colour == color)
            {
                return button;
            }
        }
        Debug.LogError($"No ColourButton found for color: {color}");
        return null;
    }
}
