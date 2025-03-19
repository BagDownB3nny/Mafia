using Mirror;
using TMPro;
using UnityEngine;

public class ShootablePlayer : Shootable
{
    [Header("CorpseSettings")]
    public GameObject corpsePrefab;
    private GameObject corpse;
    [SerializeField] private Player player;

    [Server]
    public override void OnShot(NetworkConnectionToClient shooter)
    {
        // Send a message to the player client that shot
        PlayerUIManager.instance.RpcSetTemporaryInteractableText(shooter, "You killed a player!", 1.5f);
        // Send a message to the client that the player was shot
        PlayerUIManager.instance.RpcSetTemporaryInteractableText(connectionToClient, "You were shot!", 1.5f);
        gameObject.GetComponentInParent<Player>().UnequipGun();
        // Mark the player as dead
        SetDeath();
    }

    [Server]
    public void SetDeath()
    {
        if (player.isDead)
        {
            Debug.LogWarning("Player is already dead");
            return;
        }
        player.isDead = true;
        RpcSetDeath();
        corpse = Instantiate(corpsePrefab, transform.position, transform.rotation);
        NetworkServer.Spawn(corpse);
        PubSub.Publish<Player>(PubSubEvent.PlayerDeath, player);
    }

    [ClientRpc]
    public void RpcSetDeath()
    {
        // Get the player object which is the parent of the shootable player object
        var player = gameObject.GetComponentInParent<Player>();
        // Recursively change all children layers to ghost
        Layer.SetLayerChildren(player.gameObject, LayerMask.NameToLayer("Ghost"));
    }

}
