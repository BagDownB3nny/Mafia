using UnityEngine;
using Mirror;

public class LocalPlayerGun : NetworkBehaviour
{

    [SerializeField] private AudioSource pistolShotSound;
    float timePerShot = 0.6f;
    float shotCooldown = 0.0f;
    private PlayerCamera playerCamera;
    public void Awake()
    {
        playerCamera = PlayerCamera.instance;
    }

    public void Update()
    {
        HandleGunShooting();
    }

    [Client]
    private void HandleGunShooting()
    {
        shotCooldown -= Time.deltaTime;
        // Handle shooting
        if (Input.GetMouseButtonDown(0))
        {
            if (shotCooldown > 0.0f)
            {
                return;
            }
            shotCooldown = timePerShot;
            Vector3 currentLookingAtDirection = playerCamera.GetLookingAtDirection();
            CmdShoot(currentLookingAtDirection, playerCamera.transform.position);
        }
    }

    [Command]
    private void CmdShoot(Vector3 lookingAtDirection, Vector3 playerPosition)
    {
        GameObject lookingAt = PlayerCamera.GetLookingAt(lookingAtDirection, playerPosition, 40.0f);
        NetworkConnectionToClient connectionToPlayer = PlayerManager.instance.localPlayer.connectionToClient;
        Shootable shootable = GetShootable(lookingAt);
        if (lookingAt != null && shootable != null)
        {
            if (connectionToPlayer == null)
            {
                Debug.LogError("Connection to client is null");
                return;
            }
            bool didShotGoThrough = shootable.OnShot(connectionToPlayer);
            if (didShotGoThrough)
            {
                RpcRemotePlayerShot();
            }
        }
        else
        {
            RpcRemotePlayerShot();
        }
    }

    [ClientRpc]
    private void RpcRemotePlayerShot()
    {
        pistolShotSound.Play();
    }

    private Shootable GetShootable(GameObject containsShootable)
    {
        if (containsShootable == null)
        {
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
