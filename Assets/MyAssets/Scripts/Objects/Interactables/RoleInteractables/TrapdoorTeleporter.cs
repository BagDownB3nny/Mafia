using UnityEngine;
using Mirror;

// For now the teleporter rune is only used for mafia <-> mafiahouse travel
// If use cases expand we need to refactor this code
public class TrapdoorTeleporter : Interactable
{
    [Header("Teleportation locations")]
    public Transform teleportLocation;
    public Transform returnLocation;

    [Header("Mafia house teleporter rune")]
    public static TrapdoorTeleporter mafiaHouseTrapdoorTeleporter;
    public bool isMafiaHouseTrapdoorTeleporter;
    public bool isUsable;

    public void Start()
    {
        if (isMafiaHouseTrapdoorTeleporter)
        {
            mafiaHouseTrapdoorTeleporter = this;
        }
        TimeManagerV2.instance.hourlyClientEvents[0].AddListener(ActivateTrapdoorTeleporter);
        TimeManagerV2.instance.hourlyClientEvents[8].AddListener(DeactivateTrapdoorTeleporter);
    }

    public void OnDestroy()
    {
        TimeManagerV2.instance.hourlyClientEvents[0].RemoveListener(ActivateTrapdoorTeleporter);
        TimeManagerV2.instance.hourlyClientEvents[8].RemoveListener(DeactivateTrapdoorTeleporter);
    }

    public override RoleName[] GetRolesThatCanInteract()
    {
        return new RoleName[] { RoleName.Mafia };
    }

    public void DeactivateTrapdoorTeleporter()
    {
        isUsable = false;
    }

    public void ActivateTrapdoorTeleporter()
    {
        isUsable = true;
    }

    public override string GetInteractableText()
    {
        // For now let's allow mafia to teleport using the mafia house trapdoor teleporter
        // even if it's not usable so that they can at least return home
        if (!isUsable && !isMafiaHouseTrapdoorTeleporter) return notInteractableText;

        if (isMafiaHouseTrapdoorTeleporter)
        {
            return "[R] Teleport";
        }
        else
        {
            return "[R] Teleport to Mafia House";
        }
    }

    [Client]
    public override void Interact()
    {
        // For now let's allow mafia to teleport using the mafia house trapdoor teleporter
        // even if it's not usable so that they can at least return home
        if (!isUsable && !isMafiaHouseTrapdoorTeleporter) return;

        if (isMafiaHouseTrapdoorTeleporter)
        {
            TeleportFromMafiaHouse();
        }
        else
        {
            TeleportToMafiaHouse();
        }
    }

    [Client]
    public void TeleportToMafiaHouse()
    {
        Player player = NetworkClient.localPlayer.GetComponent<Player>();
        PlayerTeleporter playerTeleporter = player.GetComponent<PlayerTeleporter>();
        Transform mafiaHouseTeleportLocation = mafiaHouseTrapdoorTeleporter.teleportLocation;
        playerTeleporter.ClientTeleportPlayer(mafiaHouseTeleportLocation.position);
        mafiaHouseTrapdoorTeleporter.returnLocation = teleportLocation;
    }

    [Client]
    public void TeleportFromMafiaHouse()
    {
        Player player = NetworkClient.localPlayer.GetComponent<Player>();
        PlayerTeleporter playerTeleporter = player.GetComponent<PlayerTeleporter>();
        if (returnLocation)
        {
            playerTeleporter.ClientTeleportPlayer(returnLocation.position);
        }
        else
        {
            House house = player.house;
            playerTeleporter.ClientTeleportPlayer(house.spawnPoint.position);
        }
    }

}
