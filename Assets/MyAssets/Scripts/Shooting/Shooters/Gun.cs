using UnityEngine;
using Mirror;

public class LocalPlayerGun : MonoBehaviour
{

    private Player equippedPlayer;

    public void Start()
    {
        equippedPlayer = PlayerManager.localPlayer;
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && equippedPlayer.isLocalPlayer)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Debug.Log("Shots fired!");
        GameObject lookingAt = Camera.main.GetComponent<PlayerCamera>().GetLookingAt(40.0f);
        if (lookingAt != null && lookingAt.GetComponent<Shootable>() != null)
        {
            Shootable shootable = lookingAt.GetComponent<Shootable>();
            shootable.CmdOnShot();
        }
    }
}
