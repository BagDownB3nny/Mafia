using Mirror;
using UnityEngine;

public class MafiaRoom : NetworkBehaviour
{
    [SerializeField] private GameObject teleporterRune;
    [SerializeField] private GameObject trapdoorTeleporter;
    [SerializeField] private GameObject fakeTrapdoor;

    public void OnEnable()
    {
        if (isClient)
        {
            // TimeManagerV2.instance.hourlyServerEvents[0].AddListener(EnableTeleporterRune);
            // TimeManagerV2.instance.hourlyServerEvents[8].AddListener(DisableTeleporterRune);
            TrapdoorTeleporterSetActive(true);
        }
    }

    public void OnDisable()
    {
        if (isClient)
        {
            // TimeManagerV2.instance.hourlyServerEvents[0].RemoveListener(EnableTeleporterRune);
            // TimeManagerV2.instance.hourlyServerEvents[8].RemoveListener(DisableTeleporterRune);
            TrapdoorTeleporterSetActive(false);
        }
    }

    [Server]
    public void EnableTeleporterRune()
    {
        teleporterRune.SetActive(true);
        RpcTeleporterRuneSetActive(true);
    }

    [Server]
    public void DisableTeleporterRune()
    {
        teleporterRune.SetActive(false);
        RpcTeleporterRuneSetActive(false);
    }

    [ClientRpc]
    public void RpcTeleporterRuneSetActive(bool isActive)
    {
        teleporterRune.SetActive(isActive);
    }

    [Client]
    public void TrapdoorTeleporterSetActive(bool isActive)
    {
        trapdoorTeleporter.SetActive(isActive);
        fakeTrapdoor.SetActive(!isActive);
    }
}
