using Mirror;
using UnityEngine;
public abstract class Menu : NetworkBehaviour
{
    [SerializeField] protected GameObject root;

    public bool IsOpen => root.activeSelf;

    public virtual void Open()
    {
        root.SetActive(true);
        PlayerCamera.instance.EnterCursorMode();
        MenuManager.instance.RegisterOpen(this);
    }

    public virtual void Close()
    {
        Debug.Log("Close virtual called");
        root.SetActive(false);
        PlayerCamera.instance.ExitCursorMode();
        MenuManager.instance.Unregister(this);
    }
}
