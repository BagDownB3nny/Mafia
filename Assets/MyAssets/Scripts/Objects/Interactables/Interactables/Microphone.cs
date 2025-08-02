using Dissonance;
using Mirror;

public class Microphone : Interactable
{
    public override RoleName[] GetRolesThatCanInteract() {
        return GetAllRoles();
    }

    [Client]
    public override void Interact() 
    {
        // Toggle between normal voice and loudspeaker mode (3x volume)
        // DissonanceRoomManager.instance.CmdToggleLoudspeakerVoice();
    }
}
