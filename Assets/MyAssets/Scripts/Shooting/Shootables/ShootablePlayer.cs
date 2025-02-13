using Mirror;
using TMPro;
using UnityEngine;

public class ShootablePlayer : Shootable
{
    [Header("CorpseSettings")]
    public GameObject corpsePrefab;
    private GameObject corpse;

    [Server]
    public override void OnShot(NetworkConnectionToClient shooter)
    {
        Debug.Log($"{name} was shot!");
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
        RpcSetDeath();
        corpse = Instantiate(corpsePrefab, transform.position, transform.rotation);
        NetworkServer.Spawn(corpse);
    }

    [ClientRpc]
    public void RpcSetDeath()
    {
        Debug.Log("Player died");
        // Get the player object which is the parent of the shootable player object
        var player = gameObject.GetComponentInParent<Player>();
        // Recursively change all children layers to ghost
        Layer.SetLayerChildren(player.gameObject, LayerMask.NameToLayer("Ghost"));
    }

}
