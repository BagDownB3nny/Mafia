using UnityEngine;
using Mirror;

public class LocalPlayerGun : NetworkBehaviour
{

    [SerializeField] private AudioSource pistolShotSound;
    readonly float timePerShot = 0.6f;
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
            GameObject shotAt = PlayerCamera.GetLookingAt(currentLookingAtDirection, playerCamera.transform.position, 40.0f);
            Shootable shootable = GetShootable(shotAt);
            CmdShoot(shootable);
        }
    }

    [Command]
    private void CmdShoot(Shootable shootable)
    {
        if (shootable == null)
        {
            RpcRemotePlayerShot();
            return;
        }

        bool didShotGoThrough = shootable.OnShot(connectionToClient);
        if (didShotGoThrough)
        {
            RpcRemotePlayerShot();
        }
    }

    [ClientRpc]
    private void RpcRemotePlayerShot()
    {
        Debug.Log("BANG");
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
