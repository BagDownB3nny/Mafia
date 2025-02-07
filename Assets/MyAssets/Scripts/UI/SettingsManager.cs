using UnityEngine;

public class SettingsManager : MonoBehaviour
{

    public static SettingsManager instance;

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
        gameObject.SetActive(false);
    }
    public void OnEnable()
    {
        Camera.main.gameObject.GetComponent<PlayerCamera>().EnterCursorMode();
        PlayerMovement.instance.LockPlayerMovement();
    }

    public void OnDisable()
    {
        Camera.main.gameObject.GetComponent<PlayerCamera>().EnterFPSMode();
        PlayerMovement.instance.UnlockPlayerMovement();
    }

    public void OnClickBack()
    {
        gameObject.SetActive(false);
    }
}