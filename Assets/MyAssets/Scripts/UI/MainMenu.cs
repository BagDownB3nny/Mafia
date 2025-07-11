using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

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

    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadHostLobby()
    {
        SceneManager.LoadScene("HostLobby");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void LoadJoinLobby()
    {
        SceneManager.LoadScene("JoinLobby");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // This code ONLY COMPILES in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
                // This code ONLY COMPILES in builds
                Application.Quit();
#endif
    }
}
