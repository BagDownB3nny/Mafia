using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;
    [SerializeField] private TMP_Text roleText;

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
}
