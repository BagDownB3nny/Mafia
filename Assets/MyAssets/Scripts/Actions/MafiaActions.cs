using UnityEngine;
using Mirror;

public class MafiaActions : RoleActions
{
    private readonly KeyCode equipGunKey = KeyCode.Q;

    [Client]
    private void Update()
    {
        if (!isLocalPlayer) return;
        HandleGunEquipping();
    }

    [Client]
    private void HandleGunEquipping()
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
        if (roleScript == null)
        {
            Debug.LogError("Role script is null");
            return;
        }
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
        Shootable shootable = GetShootable(lookingAt);
        if (lookingAt != null && shootable != null)
        {
            if (connectionToClient == null)
            {
                Debug.LogError("Connection to client is null");
                return;
            }
            shootable.OnShot(connectionToClient);
        }
    }

    private Shootable GetShootable(GameObject containsShootable)
    {
        if (containsShootable == null)
        {
            Debug.LogError("Contains shootable is null");
            return null;
        }
        Shootable shootable = containsShootable.GetComponentInParent<Shootable>();
        if (shootable == null)
        {
            shootable = containsShootable.GetComponentInChildren<Shootable>();
        }
        return shootable;
    }
}