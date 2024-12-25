using Mirror;
using UnityEngine;
using System;

public class SeeingEyeSigil : NetworkBehaviour
{

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
    public void Mark()
    {
        // Activating visual indicator
        gameObject.SetActive(true);
        RpcSetActive(true);
    }

    [Server]
    public void Unmark()
    {
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
}
