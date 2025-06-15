using UnityEngine;
using Mirror;
using System.Linq;

public class SeerActions : RoleActions
{
    [SerializeField] private Seer seer;

    [Header("Seer internal params")]
    private float timeToDeactivation;

    public void Update()
    {
        if (!isLocalPlayer) return;

        HandleSeerActions();
    }

    [Client]
    protected override void OnLookingAt(Interactable interactable)
    {
        bool isInteractable = interactable != null && interactable.GetRolesThatCanInteract().Contains(RoleName.Seer);
        if (!isInteractable) return;

        if (interactable is InteractablePlayer playerInteractable)
        {
            string interactableText = "[R] Mark with Seeing-Eye Sigil";
            playerInteractable.Highlight();
            PlayerUIManager.instance.AddInteractableText(playerInteractable, interactableText);
        }
        else if (interactable is SeerCrystalBall interactableCrystalBall)
        {
            HandleCrystalBallHover(interactableCrystalBall);
        }
    }
    [Client]
    protected override void OnPlayerDeath(Player player)
    {
        if (player.isLocalPlayer)
        {
            // If the local player dies, reset the Seer state
            CmdClearMarkedPlayer();
            seer.isLookingThroughCrystalBall = false;
        }
        if (player == seer.markedPlayer && isLocalPlayer)
        {
            // If the marked player dies, unmark them
            CmdClearMarkedPlayer();
            seer.isLookingThroughCrystalBall = false;
            PlayerUIManager.instance.ClearControlsText();
            Camera.main.GetComponent<PlayerCamera>().ExitCrystalBallMode();
            // TODO: Set informative text about the death of the marked player
        }
        base.OnPlayerDeath(player);
    }

    [Client]
    public void HandleCrystalBallHover(SeerCrystalBall interactableCrystalBall)
    {
        if (seer.markedPlayer == null)
        {
            interactableCrystalBall.Highlight();
            PlayerUIManager.instance.AddInteractableText(interactableCrystalBall, "No player marked");
        }
        else
        {
            string interactableText = "[R] Use crystal ball";
            PlayerUIManager.instance.AddInteractableText(interactableCrystalBall, interactableText);
        }
    }

    [Client]
    private void HandleSeerActions()
    {
        if (seer.isLookingThroughCrystalBall)
        {
            HandleInsideCrystalBallActions();
        }
        else
        {
            HandleOutsideCrystalBallActions();
        }
    }

    [Client]
    public void HandleInsideCrystalBallActions()
    {
        if (timeToDeactivation > 0)
        {
            timeToDeactivation -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R) && timeToDeactivation <= 0)
        {
            seer.isLookingThroughCrystalBall = false;
            PlayerUIManager.instance.ClearControlsText();
            Camera.main.GetComponent<PlayerCamera>().ExitCrystalBallMode();
        }
    }

    public void HandleOutsideCrystalBallActions()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Interactable interactable = playerCamera.GetInteractable();
            bool isInteractable = interactable != null && interactable.GetRolesThatCanInteract().Contains(RoleName.Seer);
            if (!isInteractable) return;

            if (interactable is SeerCrystalBall interactableCrystalBall)
            {
                HandleCrystalBallInteraction(interactableCrystalBall);
            }
            else if (interactable is InteractablePlayer playerInteractable)
            {
                HandlePlayerInteraction(playerInteractable);
            }
        }
    }

    [Client]
    private void HandleCrystalBallInteraction(SeerCrystalBall interactableCrystalBall)
    {
        if (interactableCrystalBall == null) { Debug.LogError("Interacting with null crystal ball"); return; }
        if (seer.markedPlayer == null) return;

        SeeThroughCrystalBall();
    }

    [Client]
    public void SeeThroughCrystalBall()
    {
        Player markedPlayer = seer.markedPlayer;
        if (markedPlayer == null) { Debug.LogError("Marked player is null"); return; }

        SeeingEyeSigil markedPlayerSeeingEyeSigil = markedPlayer.GetComponentInChildren<SeeingEyeSigil>(includeInactive: true);
        if (markedPlayerSeeingEyeSigil == null) { Debug.LogError("Player does not have a seeing-eye sigil"); return; }

        seer.isLookingThroughCrystalBall = true;
        timeToDeactivation = 0.5f;
        Camera.main.GetComponent<PlayerCamera>().EnterCrystalBallMode(markedPlayerSeeingEyeSigil.transform);
        PlayerUIManager.instance.SetControlsText("[R] Exit Crystal Ball");
    }

    [Client]
    private void HandlePlayerInteraction(InteractablePlayer playerInteractable)
    {
        if (playerInteractable == null) { Debug.LogError("Interacting with null player"); return; }

        CmdInteractWithPlayer(playerInteractable);
    }

    [Command]
    private void CmdInteractWithPlayer(InteractablePlayer playerInteractable)
    {
        if (playerInteractable == null) { Debug.LogError("[Server] Interacting with null player"); return; }

        Player player = playerInteractable.GetComponentInParent<Player>();
        SeeingEyeSigil sigil = player.GetComponentInChildren<SeeingEyeSigil>(includeInactive: true);

        if (sigil.isMarked)
        {
            sigil.Unmark();
            seer.markedPlayer = null;
        }
        else if (!sigil.isMarked)
        {
            seer.markedPlayer = player;
            seer.RemovePreviouslyPlacedSigils();
            sigil.Mark(player.netId);
        }
    }
    [Command]
    private void CmdClearMarkedPlayer()
    {
        seer.markedPlayer = null;
        SeeingEyeSigil.ResetSeeingEyeSigil();
    }
}
