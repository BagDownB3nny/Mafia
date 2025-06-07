using UnityEngine;
using Mirror;

// For now the teleporter rune is only used for mafia <-> mafiahouse travel
// If use cases expand we need to refactor this code
public class TeleporterRune : Interactable, IHideable
{
    public LayerName Layer => LayerName.Mafia;

    [Header("Teleportation locations")]
    public Transform teleportLocation;
    public Transform returnLocation;

    [Header("Mafia house teleporter rune")]
    public static TeleporterRune mafiaHouseTeleportRune;
    public bool isMafiaHouseTeleportRune;


    public void Start()
    {
        if (isMafiaHouseTeleportRune)
        {
            mafiaHouseTeleportRune = this;
        }
    }

    public override RoleName[] GetRolesThatCanInteract()
    {
        return new RoleName[] { RoleName.Mafia };
    }

    public override string GetInteractableText()
    {
        if (isMafiaHouseTeleportRune)
        {
            return "[R] Teleport";
        }
        else
        {
            return "[R] Teleport to Mafia House";
        }
    }

    [Client]
    public void TeleportToMafiaHouse()
    {
        Player player = NetworkClient.localPlayer.GetComponent<Player>();
        PlayerTeleporter playerTeleporter = player.GetComponent<PlayerTeleporter>();
        Transform mafiaHouseTeleportLocation = mafiaHouseTeleportRune.teleportLocation;
        playerTeleporter.ClientTeleportPlayer(mafiaHouseTeleportLocation.position, mafiaHouseTeleportLocation.rotation);
        mafiaHouseTeleportRune.returnLocation = teleportLocation;
    }

    [Client]
    public void TeleportFromMafiaHouse()
    {
        Player player = NetworkClient.localPlayer.GetComponent<Player>();
        PlayerTeleporter playerTeleporter = player.GetComponent<PlayerTeleporter>();
        if (returnLocation)
        {
            playerTeleporter.ClientTeleportPlayer(returnLocation.position, returnLocation.rotation);
        }
        else
        {
            playerTeleporter.TeleportToSpawn();
        }
    }

    [Client]
    public override void Interact()
    {
        if (isMafiaHouseTeleportRune)
        {
            TeleportFromMafiaHouse();
        }
        else
        {
            TeleportToMafiaHouse();
        }
    }
}
