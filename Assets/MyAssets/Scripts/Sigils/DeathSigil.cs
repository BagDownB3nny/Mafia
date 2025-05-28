using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class DeathSigil : Sigil
{
    private readonly Player player;
    // public override string Layer => LayerName.DeathSigil.ToString();
    public override LayerName Layer => LayerName.Mafia;

    [SyncVar]
    private int marksReceived = 0;
    public static List<DeathSigil> deathSigils = new();

    private void Start()
    {
        if (isServer)
        {
            deathSigils.Add(this);
        }
    }

    [Server]
    public override void Mark(uint markedPlayerNetId)
    {
        marksReceived++;
        if (marksReceived > 0 && !gameObject.activeSelf)
        {
            Debug.Log("Death sigil placed on player");
            gameObject.SetActive(true);
            RpcSetActive(true);
        }
        if (IsMarkedForDeath())
        {
            // TODO: Visual effect showing that the player is marked for death
            // Also need another visual effect to show 
        }
    }

    [Server]
    public override void Unmark()
    {
        marksReceived--;
        if (marksReceived == 0)
        {
            gameObject.SetActive(false);
            RpcSetActive(false);
        }
        else if (!IsMarkedForDeath())
        {
            // Careful to unmark death sigil before marking death sigil, or else there is a chance
            // no player will be marked for death
        }
    }

    [ClientRpc]
    public void RpcSetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    [Server]
    public static void ResetAllDeathSigils()
    {
        foreach (DeathSigil deathSigil in deathSigils)
        {
            deathSigil.Reset();
        }
    }

    [Server]
    public void Reset()
    {
        marksReceived = 0;
        gameObject.SetActive(false);
        RpcSetActive(false);
    }

    private bool IsMarkedForDeath()
    {
        int mafiaCount = PlayerManager.instance.GetMafiaCount();
        float minimumMarksNeeded = (float)mafiaCount / 2;
        Debug.Log("Marks received: " + marksReceived + ", minimum marks needed: " + minimumMarksNeeded);
        return marksReceived >= minimumMarksNeeded;
    }

    [Server]
    public static Player GetPlayerMarkedForDeath()
    {
        Debug.Log("Length of deathSigils: " + deathSigils.Count);
        foreach (DeathSigil deathSigil in deathSigils)
        {
            Debug.Log(deathSigil.name + " is marked for death: " + deathSigil.IsMarkedForDeath());
            if (deathSigil.IsMarkedForDeath())
            {
                return deathSigil.player;
            }
        }
        return null;
    }

    [Server]
    public static void ActivateAtNight()
    {
        Player playerMarkedForDeath = GetPlayerMarkedForDeath();
        Debug.Log("Player marked for death: " + playerMarkedForDeath);
        if (playerMarkedForDeath != null)
        {
            Debug.Log(playerMarkedForDeath.steamUsername + " is marked for death");
            playerMarkedForDeath.house.Unmark();
        }
    }
}
