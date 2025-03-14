using UnityEngine;

public class Debugger : MonoBehaviour
{
    public static Debugger instance;
    [SerializeField] TMPro.TextMeshProUGUI text;

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
        Log("Debugger started");
    }

    public void Log(string message)
    {
        text.text += message + "\n";
        // If more than 3 lines, remove the first line
        if (text.text.Split('\n').Length > 3)
        {
            text.text = text.text.Substring(text.text.IndexOf('\n') + 1);
        }
    }
}
