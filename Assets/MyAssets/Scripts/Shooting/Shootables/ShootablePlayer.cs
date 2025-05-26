using Mirror;
using UnityEngine;

public class ShootablePlayer : Shootable
{
    [SerializeField] private Player player;

    [Server]
    public override bool OnShot(NetworkConnectionToClient shooter)
    {
        if (player.GetComponentInChildren<PlayerProtectionSigil>(includeInactive: true).isMarked)
        {
            // If the player is protected, do not shoot
            PlayerUIManager.instance.RpcSetTemporaryInteractableText(shooter, "Player is protected by the guardian!", 1.5f);
            return true;
        }
        // Get the player's Mafia role component and remove gun if they are Mafia
        Player shotPlayer = gameObject.GetComponentInParent<Player>();
        Role roleScript = shotPlayer.GetRoleScript();
        if (roleScript is Mafia mafiaRole)
        {
            mafiaRole.UnequipGun();
        }

        // Mark the player as dead
        GetComponentInParent<PlayerDeath>().KillPlayer();
        return true;
    }
}
