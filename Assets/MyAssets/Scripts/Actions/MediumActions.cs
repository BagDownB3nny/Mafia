using UnityEngine;
using Mirror;
using System.Linq;

public class MediumActions : RoleActions
{
    [SerializeField] private Medium medium;

    [Header("Medium internal params")]
    private float timeToDeactivation;
    private bool isCommunicating;

    public void Update()
    {
        if (!isLocalPlayer) return;

        HandleMediumActions();
    }

    [Client]
    protected override void OnLookingAt(Interactable interactable)
    {
        Debug.Log("Looking at interactable: " + interactable.name);
        bool isInteractable = interactable != null && interactable.GetRolesThatCanInteract().Contains(RoleName.Medium);
        if (!isInteractable) return;

        if (interactable is OuijaBoard ouijaBoard)
        {
            string interactableText = ouijaBoard.GetInteractableText();
            if (interactableText == Interactable.notInteractableText) return;
            ouijaBoard.Highlight();
            PlayerUIManager.instance.AddInteractableText(ouijaBoard, interactableText);
        }
    }
    [Client]
    protected override void OnPlayerDeath(Player player)
    {
        if (player.isLocalPlayer)
        {
            isCommunicating = false;
            DissonanceRoomManager.instance.OnMediumDeactivation();
            CameraCullingMaskManager.instance.SetGhostLayerInvisible();
            CmdInformGhostsAboutMediumDeactivation();
        }
        base.OnPlayerDeath(player);
    }

    [Client]
    private void HandleMediumActions()
    {
        if (timeToDeactivation > 0)
        {
            timeToDeactivation -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Interactable interactable = playerCamera.GetInteractable();
            bool isInteractable = interactable != null && interactable.GetRolesThatCanInteract().Contains(RoleName.Medium);
            if (isInteractable)
            {
                if (interactable is OuijaBoard ouijaBoard && timeToDeactivation <= 0)
                {
                    // Extra checks in case
                    bool isBetweenMidnightAndMorning = TimeManagerV2.instance.IsBetweenMidnightAndMorning();
                    if (!ouijaBoard.canBeUsed || !isBetweenMidnightAndMorning)
                    {
                        PlayerUIManager.instance.SetInformativeText("Ouija board can only be used at night");
                        return;
                    }
                    if (!isCommunicating)
                    {
                        ActivateMediumAbility();
                        ouijaBoard.isActivated = true;
                    }
                    else if (isCommunicating)
                    {
                        DeactivateMediumAbility();
                        ouijaBoard.isActivated = false;
                    }
                }
            }
        }
    }

    [Client]
    public void ActivateMediumAbility()
    {
        isCommunicating = true;
        timeToDeactivation = 0.5f;
        DissonanceRoomManager.instance.OnMediumActivation();
        CameraCullingMaskManager.instance.SetGhostLayerVisible();

        PlayerUIManager.instance.SetControlsText("[Passive] You are now communicating with the dead...");
        CmdInformGhostsAboutMediumActivation();
    }

    [Command]
    public void CmdInformGhostsAboutMediumActivation()
    {
        HouseManager.instance.HighlightMediumHouseForGhosts();
        PlayerUIManager.instance.InformGhostsAboutMediumActivation();
    }

    [Client]
    public void DeactivateMediumAbility()
    {
        isCommunicating = false;
        DissonanceRoomManager.instance.OnMediumDeactivation();
        CameraCullingMaskManager.instance.SetGhostLayerInvisible();

        PlayerUIManager.instance.ClearControlsText();
        CmdInformGhostsAboutMediumDeactivation();
    }

    [Command]
    public void CmdInformGhostsAboutMediumDeactivation()
    {
        HouseManager.instance.UnhighlightMediumHouseForGhosts();
        PlayerUIManager.instance.InformGhostsAboutMediumDeactivation();
    }
}
