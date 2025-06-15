using UnityEngine;
public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    private Menu currentMenu;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void RegisterOpen(Menu menu)
    {
        if (currentMenu != null && currentMenu != menu)
            currentMenu.Close();
        {
            currentMenu = menu;
        }
    }

    public void Unregister(Menu menu)
    {
        if (currentMenu == menu)
        {
            currentMenu = null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentMenu != null && currentMenu.IsOpen)
            {
                currentMenu.Close();
            }
            else
            {
                OnNoMenuToClose();
            }
        }
    }

    void OnNoMenuToClose()
    {
        SettingsMenu.instance.Open();
    }
}
