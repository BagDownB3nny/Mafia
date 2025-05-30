using Mirror;
using UnityEngine;
using DG.Tweening;

public class InteractableDoor : Interactable
{

    [SerializeField] private Door door;

    [SyncVar]
    public bool isOpen = false;

    public readonly SyncList<uint> authorisedPlayers = new SyncList<uint>();
    [SerializeField] private Transform doorOpenPosition;
    [SerializeField] private Transform doorClosedPosition;

    [Header("Knock")]
    private AudioSource knockingAudioSource;

    public void Start()
    {
        Unhighlight();
        knockingAudioSource = GetComponent<AudioSource>();
    }

    public override RoleName[] GetRolesThatCanInteract()
    {
        return GetAllRoles();
    }

    [Server]
    public void AssignAuthority(Player player)
    {
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }
        Debug.Log("Assigning authority to player: " + player.netId);
        authorisedPlayers.Add(player.netId);
    }

    [Server]
    public void RemoveAuthority(Player player)
    {
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }
        authorisedPlayers.Remove(player.netId);
    }

    [Client]
    public override string GetInteractableText()
    {
        if (door.isKnockedDown) return null;
        if (!door.isEnabled) return null;

        uint playerNetId = NetworkClient.localPlayer.netId;
        if (authorisedPlayers.Contains(playerNetId))
        {
            return isOpen ? "[E] Close" : "[E] Open";
        }
        else
        {
            return "[E] Knock";
        }
    }

    [Client]
    public void ClientKnock()
    {
        if (knockingAudioSource.isPlaying) return;
        // Play knock sound effect
        // Play knock animation
        knockingAudioSource.Play();
        CmdKnock();
    }

    [Command(requiresAuthority = false)]
    public void CmdKnock()
    {
        if (door.isKnockedDown) return;
        if (!door.isEnabled) return;
        RpcKnock();
    }

    [ClientRpc]
    public void RpcKnock()
    {
        if (door.isKnockedDown) return;
        if (!door.isEnabled) return;
        if (knockingAudioSource.isPlaying) return;
        // Play knock sound effect
        // Play knock animation
        knockingAudioSource.Play();
    }

    [Client]
    public void ClientOpen()
    {

    }

    [Client]
    public void ClientClose()
    {

    }

    [Client]
    public override void Interact()
    {
        if (door.isKnockedDown) return;
        if (!door.isEnabled) return;
        uint playerNetId = NetworkClient.localPlayer.netId;
        if (authorisedPlayers.Contains(playerNetId))
        {
            CmdInteract();
        }
        else
        {
            CmdKnock();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdInteract()
    {
        if (door.isKnockedDown) return;
        if (!door.isEnabled) return;
        if (isOpen)
        {

            RpcCloseDoor();
        }
        else
        {
            Debug.Log("Opening door");
            RpcOpenDoor();
        }
    }

    [Server]
    public void OpenDoor()
    {
        RpcOpenDoor();
    }

    [ClientRpc]
    public void RpcOpenDoor()
    {
        isOpen = true;
        float animationDuration = 0.7f;

        if (doorOpenPosition == null)
        {
            Debug.LogError("Door open position is null");
            return;
        }
        transform.DOMove(doorOpenPosition.position, animationDuration).SetEase(Ease.InQuad);
        transform.DORotateQuaternion(doorOpenPosition.rotation, animationDuration).SetEase(Ease.InQuad);
    }

    [Server]
    public void CloseDoor()
    {
        isOpen = false;
        float animationDuration = 0.7f;
        transform.DOMove(doorClosedPosition.position, animationDuration).SetEase(Ease.InQuad);
        transform.DORotateQuaternion(doorClosedPosition.rotation, animationDuration).SetEase(Ease.InQuad);
        RpcCloseDoor();
    }

    [ClientRpc]
    public void RpcCloseDoor()
    {
        isOpen = false;
        float animationDuration = 0.7f;

        if (doorClosedPosition == null)
        {
            Debug.LogError("Door closed position is null");
            return;
        }
        transform.DOMove(doorClosedPosition.position, animationDuration).SetEase(Ease.InQuad);
        transform.DORotateQuaternion(doorClosedPosition.rotation, animationDuration).SetEase(Ease.InQuad);
    }
}
