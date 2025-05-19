using Mirror;
using UnityEngine;

public class MafiaRoom : NetworkBehaviour
{
    [SerializeField] private GameObject teleporterRune;

    public void OnEnable()
    {
        if (isServer)
        {
            TimeManagerV2.instance.hourlyServerEvents[0].AddListener(EnableTeleporterRune);
            TimeManagerV2.instance.hourlyServerEvents[8].AddListener(DisableTeleporterRune);
        }
    }

    public void OnDisable()
    {
        if (isServer)
        {
            TimeManagerV2.instance.hourlyServerEvents[0].RemoveListener(EnableTeleporterRune);
            TimeManagerV2.instance.hourlyServerEvents[8].RemoveListener(DisableTeleporterRune);
        }
    }

    [Server]
    public void EnableTeleporterRune()
    {
        teleporterRune.gameObject.SetActive(true);
        RpcTeleporterRuneSetActive(true);
    }

    [Server]
    public void DisableTeleporterRune()
    {
        teleporterRune.gameObject.SetActive(false);
        RpcTeleporterRuneSetActive(false);
    }

    [ClientRpc]
    public void RpcTeleporterRuneSetActive(bool isActive)
    {
        teleporterRune.gameObject.SetActive(isActive);
    }
}
