using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerSigilManager : NetworkBehaviour
{

    [SerializeField] private GameObject SeeingEyeSigil;
    [SerializeField] private GameObject ProtectionSigil;
    [SerializeField] private GameObject DeathSigil;


    [SyncVar]
    public List<Sigils> receivedSigils = new List<Sigils>();

    [Server]
    public void MarkWithSigil(Sigils sigil)
    {
        receivedSigils.Add(sigil);
        activateSigil(sigil);
        RpcActivateSigil(sigil);
    }

    [ClientRpc]
    public void RpcActivateSigil(Sigils sigil)
    {
        activateSigil(sigil);
    }


    [Client]
    private void activateSigil(Sigils sigil)
    {
        switch (sigil)
        {
            case Sigils.SeeingEye:
                SeeingEyeSigil.SetActive(true);
                break;
            case Sigils.Protection:
                ProtectionSigil.SetActive(true);
                break;
            case Sigils.Death:
                DeathSigil.SetActive(true);
                break;
        }
    }
}
