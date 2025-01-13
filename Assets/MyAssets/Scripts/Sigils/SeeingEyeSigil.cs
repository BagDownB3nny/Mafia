using Mirror;
using UnityEngine;
using System;

public class SeeingEyeSigil : Sigil
{
    private static uint markedPlayerNetId = 0;
    public event Action OnDeactivate;
    private bool isActive = false;
    private float timeToDeactivation;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && timeToDeactivation <= 0 && isActive)
        {
            Deactivate();
        }
        if (timeToDeactivation > 0 && isActive)
        {
            timeToDeactivation -= Time.deltaTime;
        }
    }

    [Server]
    public override void Mark(uint playerNetId)
    {
        markedPlayerNetId = playerNetId;
        // Activating visual indicator
        gameObject.SetActive(true);
        RpcSetActive(true);
    }

    [Server]
    public override void Unmark()
    {
        markedPlayerNetId = 0;
        gameObject.SetActive(false);
        RpcSetActive(false);
    }

    [ClientRpc]
    public void RpcSetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    [Client]
    public void Activate()
    {
        FollowSeeingEyeSigil followSeeingEyeSigil = Camera.main.GetComponentInChildren<FollowSeeingEyeSigil>(includeInactive: true);
        if (followSeeingEyeSigil == null)
        {
            Debug.LogError("Camera does not have a FollowSeeingEyeSigil component");
            return;
        }
        followSeeingEyeSigil.seeingEyeSigil = transform;
        followSeeingEyeSigil.enabled = true;
        timeToDeactivation = 0.5f;

        isActive = true;
    }

    [Client]
    public void Deactivate()
    {
        Camera.main.GetComponentInChildren<FollowSeeingEyeSigil>(includeInactive: true).enabled = false;
        OnDeactivate?.Invoke();
        OnDeactivate = null;

        isActive = false;
    }
    [Server]
    public static void ResetSeeingEyeSigil()
    {
        if (markedPlayerNetId == 0) {
            return;
        }
        Debug.Log(markedPlayerNetId);
        Player player = PlayerManager.instance.GetPlayerByNetId(markedPlayerNetId);
        if (player == null)
        {
            Debug.LogError("Player not found");
            return;
        }
        SeeingEyeSigil seeingEyeSigil = player.GetComponentInChildren<SeeingEyeSigil>();
        if (seeingEyeSigil == null)
        {
            Debug.LogError("SeeingEyeSigil not found");
            return;
        }
        seeingEyeSigil.Unmark();
    }
}
