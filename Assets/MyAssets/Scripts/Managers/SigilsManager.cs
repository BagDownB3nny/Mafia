using Mirror;

public class SigilsManager : NetworkBehaviour
{
    public static SigilsManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Server]
    public void ResetAllSigils()
    {
        DeathSigil.ResetAllDeathSigils();
        SeeingEyeSigil.ResetSeeingEyeSigil();
        ProtectionSigil.ResetProtectionSigil();
    } 
}
