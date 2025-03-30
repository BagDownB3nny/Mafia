using UnityEngine;
using TMPro;
using System.Collections;
using Mirror;

// Should only be called for the client
public class PlayerUIManager : NetworkBehaviour
{
    public static PlayerUIManager instance;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text interactableText;

    public void Awake()
    {
        instance = this;
    }

    [Client]
    public void SetRoleText(RoleName role)
    {
        roleText.text = role.ToString();
    }

    [Client]
    public void SetRolePromptText(RoleName role)
    {
        string text = "";

        switch (role)
        {
            case RoleName.Villager:
                text = "Find the Mafia!";
                break;
            case RoleName.Mafia:
                text = "Kill the Villagers!";
                break;
            case RoleName.Seer:
                text = "Find the Mafia!";
                break;
            case RoleName.Guardian:
                text = "Protect the Villagers!";
                break;
            case RoleName.SixthSense:
                text = "Find the Mafia!";
                break;
            default:
                text = "Unknown Role!";
                break;
        }

        SetTemporaryInteractableText(text, 10f);
    }

    [Client]
    public void SetInteractableText(string text)
    {
        interactableText.text = text;
    }

    [Client]
    public void SetTemporaryInteractableText(string text, float time)
    {
        interactableText.text = text;
        StartCoroutine(ClearInteractableTextAfterTime(time, text));
    }

    [TargetRpc]
    public void RpcSetTemporaryInteractableText(NetworkConnectionToClient target, string text, float time)
    {
        SetTemporaryInteractableText(text, time);
    }

    [Client]
    private IEnumerator ClearInteractableTextAfterTime(float time, string text)
    {
        yield return new WaitForSeconds(time);
        if (interactableText.text == text)
        {
            ClearInteractableText();
        }
    }

    [Client]
    public void ClearInteractableText()
    {
        interactableText.text = "";
    }
}
