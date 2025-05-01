using UnityEngine;

public class EscapeMenuManager : MonoBehaviour
{

    public static EscapeMenuManager instance;

    [SerializeField] private GameObject escapeMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("EscapeMenuManager instance created");
        }
        else
        {
            Debug.LogWarning("EscapeMenuManager instance already exists, destroying this one.");
            Debug.LogWarning(instance);
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleEscapeMenu();
        }
    }

    public void ToggleEscapeMenu()
    {
        if (escapeMenuPanel.activeSelf)
        {
            CloseEscapeMenu();
        }
        else
        {
            OpenEscapeMenu();
        }
    }

    public void OpenEscapeMenu()
    {
        PlayerCamera.instance.EnterCursorMode();
        PlayerMovement.localInstance.LockPlayerMovement();
        escapeMenuPanel.SetActive(true);
    }

    public void CloseEscapeMenu()
    {
        PlayerCamera.instance.ExitCursorMode();
        PlayerMovement.localInstance.UnlockPlayerMovement();
        escapeMenuPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void OnClickBack()
    {
        CloseEscapeMenu();
    }
}