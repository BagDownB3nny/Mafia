using UnityEngine;
using Mirror;

public class MafiaActions : RoleActions
{
    private readonly KeyCode equipGunKey = KeyCode.Q;
    
    protected override void HandleRoleSpecificActions()
    {
        HandleGunActions();
    }

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
            ToggleGun();
        }

        // Handle shooting
        if (Input.GetMouseButtonDown(0) && player.IsAbleToShoot())
        {
            Vector3 currentLookingAtDirection = playerCamera.GetLookingAtDirection();
            player.CmdShoot(currentLookingAtDirection, transform);
        }
    }

    private void ToggleGun()
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

    // private void Shoot(Vector3 lookingAtDirection, Transform playerTransform)
    // {
    //     GameObject lookingAt = PlayerCamera.GetLookingAt(lookingAtDirection, playerTransform, 40.0f);
    //     if (lookingAt != null && lookingAt.GetComponentInParent<Shootable>() != null)
    //     {
    //         Shootable shootable = lookingAt.GetComponentInParent<Shootable>();
    //         if (connectionToClient == null)
    //         {
    //             Debug.LogError("Connection to client is null");
    //             return;
    //         }
    //         shootable.OnShot(connectionToClient);
    //     }
    // }
}