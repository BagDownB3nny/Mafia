using UnityEngine;
using TMPro;
using Unity.VisualScripting;

// Should only be called for the client
public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text interactableText;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("More than one instance of PlayerUIManager found!");
            return;
        }
    }

    public void SetRoleText(Roles role)
    {
        roleText.text = role.ToString();
    }

    public void SetInteractableText(string text)
    {
        interactableText.text = text;
    }

    public void ClearInteractableText()
    {
        interactableText.text = "";
    }
}
