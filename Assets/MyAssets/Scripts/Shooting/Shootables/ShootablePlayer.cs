using Mirror;
using TMPro;
using UnityEngine;

public class ShootablePlayer : Shootable
{
    [SerializeField] private Player player;

    [Server]
    public override void OnShot(NetworkConnectionToClient shooter)
    {
        // Send a message to the player client that shot
        PlayerUIManager.instance.RpcSetTemporaryInteractableText(shooter, "You killed a player!", 1.5f);
        // Send a message to the client that the player was shot
        PlayerUIManager.instance.RpcSetTemporaryInteractableText(connectionToClient, "You were shot!", 1.5f);
        
        // Get the player's Mafia role component and remove gun if they are Mafia
        Player shotPlayer = gameObject.GetComponentInParent<Player>();
        Role roleScript = shotPlayer.GetRoleScript();
        if (roleScript is Mafia mafiaRole)
        {
            mafiaRole.UnequipGun();
        }
        
        // Mark the player as dead
        GetComponentInParent<PlayerDeath>().KillPlayer();
    }
}
