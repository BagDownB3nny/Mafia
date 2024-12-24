using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerSigilManager : NetworkBehaviour
{

    [SerializeField] private SeeingEyeSigil seeingEyeSigil;
    [SerializeField] private ProtectionSigil protectionSigil;
    [SerializeField] private DeathSigil deathSigil;

    [Server]
    public void MarkWithSigil(Sigils sigil)
    {
        switch (sigil)
        {
            case Sigils.Death:
                deathSigil.Mark();
                break;
            // case Sigils.SeeingEye:
            //     seeingEyeSigil.Mark();
            //     break;
            // case Sigils.Protection:
            //     seeingEyeSigil.Mark();
            //     break;
            default:
                break;
        }
    }

    [Server]
    public void UnmarkWithSigil(Sigils sigil)
    {
        switch (sigil)
        {
            case Sigils.Death:
                deathSigil.Unmark();
                break;
            // case Sigils.SeeingEye:
            //     seeingEyeSigil.Mark();
            //     break;
            // case Sigils.Protection:
            //     seeingEyeSigil.Mark();
            //     break;
            default:
                break;
        }
    }
}
