using Mirror;
using UnityEngine;

public class PlayerDeath : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;

    [Header("Internal Parameters")]

    [SyncVar(hook = nameof(OnDeathChange))]
    public bool isDead = false;

    [Header("Corpse Settings")]
    [SerializeField] private GameObject corpsePrefab;

    [Header("Player visuals")]
    [SerializeField] private GameObject ghostVisual;
    [SerializeField] public GameObject aliveVisual;

    [Server]
    public void KillPlayer()
    {
        if (isDead)
        {
            Debug.LogWarning("Player is already dead");
            return;
        }
        ServerKillPlayer();
    }

    [Server]
    public void ServerKillPlayer()
    {
        isDead = true;
        GameObject corpse = Instantiate(corpsePrefab, transform.position, transform.rotation);
        corpse.GetComponent<DeathHandler>().playerName = player.steamUsername;
        NetworkServer.Spawn(corpse);
        PubSub.Publish(PubSubEvent.PlayerDeath, player);
    }

    [Client]
    public void OnDeathChange(bool oldValue, bool isPlayerDead)
    {
        if (isPlayerDead && isLocalPlayer) OnLocalPlayerDeath();
        if (isPlayerDead) OnPlayerDeath();
        else if (!isPlayerDead && isLocalPlayer) OnLocalPlayerRevive();
        else if (!isPlayerDead) OnPlayerRevive();
    }

    [Client]
    private void OnLocalPlayerDeath()
    {
        CameraCullingMaskManager.instance.SetGhostLayerVisible();
        DissonanceRoomManager.instance.OnPlayerDeath();
        PlayerCamera.instance.EnterSpectatorMode();
        PlayerUIManager.instance.SetGhostTexts();
    }

    [Client]
    private void OnPlayerDeath()
    {
        Player player = GetComponentInParent<Player>();
        Layer.SetLayerChildren(player.gameObject, LayerMask.NameToLayer("Ghost"));
        ghostVisual.SetActive(true);
        aliveVisual.SetActive(false);
    }

    // These revive methods are not implemented yet because there's no revive mechanic yet
    [Client]
    private void OnLocalPlayerRevive()
    {
        // Do client-side stuff here
    }

    [Client]
    private void OnPlayerRevive()
    {
        // Do client-side stuff here
    }
}
