using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigator : MonoBehaviour
{
    public static MenuNavigator instance;

    public void Awake()
    {
        if (instance == null)
        {
            Debug.Log("CREATING NEW INSTANCE");
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("DESTROYING INSTANCE");
            Destroy(gameObject);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
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
}
