using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

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

    public void SetTemporaryInteractableText(string text, float time)
    {
        interactableText.text = text;
        StartCoroutine(ClearInteractableTextAfterTime(time, text));
    }

    private IEnumerator ClearInteractableTextAfterTime(float time, string text)
    {
        yield return new WaitForSeconds(time);
        if (interactableText.text == text)
        {
            ClearInteractableText();
        }
    }

    public void ClearInteractableText()
    {
        interactableText.text = "";
    }
}
