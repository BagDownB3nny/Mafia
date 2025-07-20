using UnityEngine;

public class Microphone : Interactable
{
    public override RoleName[] GetRolesThatCanInteract() {
        return GetAllRoles();
    }

    public override void Interact() 
    {
        //TODO: Increase voiceplayback range in all other clients
    }
}
