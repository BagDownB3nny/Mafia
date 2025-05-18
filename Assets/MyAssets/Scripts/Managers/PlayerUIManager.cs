using UnityEngine;
using TMPro;
using System.Collections;
using Mirror;
using System.Collections.Generic;
using System;

// Should only be called for the client
public class PlayerUIManager : NetworkBehaviour
{
    public static PlayerUIManager instance;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text informativeText;
    [SerializeField] private TMP_Text controlsText;

    [Header("Interactable Texts")]
    private List<(Interactable Interactable, string Text)> interactableTexts = new();
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
    public void SetInformativeText(string text)
    {
        informativeText.text = text;
        CancelInvoke(nameof(ClearInformativeText));
    }

    [Client]
    public void SetInformativeText(string text, float duration)
    {
        informativeText.text = text;
        CancelInvoke(nameof(ClearInformativeText));
        Invoke(nameof(ClearInformativeText), duration);
    }

    [Client]
    public void ClearInformativeText()
    {
        informativeText.text = "";
    }

    public void SetControlsText(string text)
    {
        controlsText.text = text;
    }

    public void ClearControlsText()
    {
        controlsText.text = "";
    }

    [Client]
    public void SetRolePromptText(RoleName role)
    {
        string roleInformationText = "";
        string controlsText = "";

        switch (role)
        {
            case RoleName.Villager:
                roleInformationText = "Find the Mafia!";
                break;
            case RoleName.Mafia:
                roleInformationText = "Kill the Villagers!";
                controlsText = "At midnight, decide on a player to kill at the mafia house's basement.\nShoot down their door and kill them!";
                break;
            case RoleName.Seer:
                roleInformationText = "Find the Mafia! Use your seeing powers wisely!";
                controlsText = "Interact [E] with a player to mark them! \nLook through the crystal ball [E] to see the marked player!";
                break;
            case RoleName.Guardian:
                roleInformationText = "Protect the Villagers! Use your protection powers wisely!";
                controlsText = "Interact [E] with a player/door to protect them for the night!";
                break;
            case RoleName.SixthSense:
                roleInformationText = "Find the Mafia! Use your sixth-sense wisely!";
                controlsText = "[Passive] You have the ability to see sigils placed on players!";
                break;
            case RoleName.Medium:
                roleInformationText = "Find the Mafia! Use your ability to talk to the dead wisely!";
                controlsText = "At night, the dead will visit you! \nTalk to them to find the mafia!";
                break;
            default:
                roleInformationText = "Unknown Role!";
                break;
        }

        SetInformativeText(roleInformationText, 10f);
        SetControlsText(controlsText);
    }

    [Client]
    public void AddInteractableText(Interactable interactable, string text)
    {
        if (text == null || text == "") return;

        interactableTexts.Add((interactable, text));
        SetInteractableTextV2();
    }

    [Client]
    public void RemoveInteractableText(Interactable interactable)
    {
        interactableTexts.RemoveAll(tuple => tuple.Interactable == interactable);
        SetInteractableTextV2();
    }

    [Client]
    private void SetInteractableTextV2()
    {
        string text = "";
        foreach (var tuple in interactableTexts)
        {
            text += tuple.Text;
            text += "\n";
        }
        interactableText.text = text;
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

    [Client]
    public void SetVillagerTexts()
    {
        string tenPmText = "The mafia will launch an attack at midnight! Time to hide in the safety of your own home!";
        string twelveAmText = "[12AM] The mafia are attacking! Hide in your house and wait for the sun to rise!";
        string eightAmText = "[8AM] The attack has stopped for now... Vote out the mafia before any more villagers die!";
        string fourPmText = "Voting ends at 6pm! Finalise your votes!";

        TimeManagerV2.instance.hourlyClientEvents[22].AddListener(() => SetInformativeText(tenPmText, 10f));
        TimeManagerV2.instance.hourlyClientEvents[0].AddListener(() => SetInformativeText(twelveAmText, 10f));
        TimeManagerV2.instance.hourlyClientEvents[8].AddListener(() => SetInformativeText(eightAmText, 10f));
        TimeManagerV2.instance.hourlyClientEvents[16].AddListener(() => SetInformativeText(fourPmText, 10f));
    }

    [Client]
    public void SetMafiaTexts()
    {
        string midnightText = "[12AM] Night has fallen! Plan an attack on the villagers at the mafia house!";
        string sixAmText = "The sun rises at 8AM! Quickly complete your attack before your disguises wear off!";
        string eightAmText = "[8AM] Voting time";

        // Midnight - Show gun controls and initial text
        TimeManagerV2.instance.hourlyClientEvents[0].AddListener(() =>
        {
            SetControlsText("Press Q to equip/unequip gun");
            SetInformativeText(midnightText);
        });

        // 6AM - Update warning text
        TimeManagerV2.instance.hourlyClientEvents[6].AddListener(() =>
        {
            SetInformativeText(sixAmText);
        });

        // 8AM - Clear all UI elements
        TimeManagerV2.instance.hourlyClientEvents[8].AddListener(() =>
        {
            ClearControlsText();
            ClearInformativeText();
            SetInformativeText(eightAmText, 10f);
        });
    }

    [Client]
    public void SetAllPlayerTexts()
    {
        string sixPmText = "[6PM] A player has been voted out! Decide on their fate!";
        TimeManagerV2.instance.hourlyClientEvents[18].AddListener(() => SetInformativeText(sixPmText, 10f));
    }

    [Client]
    public void SetGhostTexts()
    {
        TimeManagerV2.instance.hourlyClientEvents[22].RemoveAllListeners();
        TimeManagerV2.instance.hourlyClientEvents[0].RemoveAllListeners();

        string twelveAmText = "[12AM] The medium is calling for you in his house... Talk to him...";
        TimeManagerV2.instance.hourlyClientEvents[0].AddListener(() => SetInformativeText(twelveAmText, 10f));

        string controlsText = "You are dead! But you can still help your team by talking to the medium at night...";
        SetControlsText(controlsText);
    }
}
