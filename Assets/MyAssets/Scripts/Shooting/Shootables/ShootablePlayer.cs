using Mirror;
using UnityEngine;

public class ShootablePlayer : NetworkBehaviour
{
    MeshRenderer meshRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the MeshRenderer component of the GameObject
        meshRenderer = GetComponent<MeshRenderer>();
    }

    [Server]
    public void OnShot(NetworkConnectionToClient shooter)
    {
        Debug.Log($"{name} was shot!");
        // Set the material of the MeshRenderer to red
        RpcSetMaterial();
        // Send a message to the player client that shot
        PlayerUIManager.instance.RpcSetTemporaryInteractableText(shooter, "You killed a player!", 1.5f);
        // Send a message to the client that the player was shot
        PlayerUIManager.instance.RpcSetTemporaryInteractableText(connectionToClient, "You were shot!", 1.5f);
        // Mark the player as dead
        RpcSetMaterial();
    }

    [ClientRpc]
    public void RpcSetMaterial()
    {
        meshRenderer.material.color = Color.red;
    }

}
