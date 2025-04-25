using UnityEngine;
using Mirror;

public class MafiaActions : RoleActions
{
    private readonly KeyCode equipGunKey = KeyCode.Q;

    public override void HandleRoleSpecificActions()
    {
        HandleGunActions();
    }

    [Client]
    private void HandleGunActions()
    {
        // Only allow gun actions during night time (between 12 AM and 8 AM)
        int currentHour = TimeManagerV2.instance.currentHour;
        if (currentHour >= 8 && currentHour < 24)
        {
            // It's daytime, no gun actions allowed
            return;
        }

        // Toggle gun equip state
        if (Input.GetKeyDown(equipGunKey))
        {
            CmdToggleGun();
        }

        // Handle shooting
        if (Input.GetMouseButtonDown(0) && player.GetRoleScript() is Mafia mafiaRole && mafiaRole.HasGun())
        {
            Vector3 currentLookingAtDirection = playerCamera.GetLookingAtDirection();
            CmdShoot(currentLookingAtDirection, transform.position);
        }
    }

    [Command]
    public void CmdToggleGun()
    {
        // Double check on server side that it's night time
        int currentHour = TimeManagerV2.instance.currentHour;
        if (currentHour >= 8 && currentHour < 24)
        {
            // It's daytime, don't allow gun toggling
            return;
        }

        Role roleScript = player.GetRoleScript();
        if (roleScript is Mafia mafiaRole)
        {
            if (mafiaRole.HasGun())
            {
                mafiaRole.UnequipGun();
            }
            else
            {
                mafiaRole.EquipGun();
            }
        }
    }

    [Command]
    private void CmdShoot(Vector3 lookingAtDirection, Vector3 playerPosition)
    {
        GameObject lookingAt = PlayerCamera.GetLookingAt(lookingAtDirection, playerPosition, 40.0f);
        if (lookingAt != null && lookingAt.GetComponentInParent<Shootable>() != null)
        {
            Shootable shootable = lookingAt.GetComponentInParent<Shootable>();
            if (connectionToClient == null)
            {
                Debug.LogError("Connection to client is null");
                return;
            }
            shootable.OnShot(connectionToClient);
        }
    }
}