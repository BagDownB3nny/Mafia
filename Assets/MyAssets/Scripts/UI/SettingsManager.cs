using UnityEngine;

public class SettingsManager : MonoBehaviour
{

    public static SettingsManager instance;

    [SerializeField] private GameObject settingsPanel;

    public void Awake()
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

    public void ToggleSettings()
    {
        if (settingsPanel.activeSelf)
        {
            CloseSettings();
        }
        else
        {
            OpenSettings();
        }
    }

    public void OpenSettings()
    {
        PlayerCamera.instance.EnterCursorMode();
        PlayerMovement.localInstance.LockPlayerMovement();
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayerCamera.instance.ExitCursorMode();
        PlayerMovement.localInstance.UnlockPlayerMovement();
        settingsPanel.SetActive(false);
    }

    public void OnClickBack()
    {
        CloseSettings();
    }
}