using UnityEngine;
using Mirror;
using DG.Tweening;

public class Lever : Interactable
{
    public bool isOn = false;
    public bool isBroken = false;

    public GameObject leverHandle;
    public Transform leverOpenPosition;
    public Transform leverClosePosition;

    public override void OnHover()
    {
        Highlight();
        string interactableText = isOn ? "Turn off the lever" : "Turn on the lever";
        PlayerUIManager.instance.SetInteractableText(interactableText);
    }

    public override void OnUnhover()
    {
        Unhighlight();
        PlayerUIManager.instance.ClearInteractableText();
    }

    public override void Interact()
    {
        CmdInteract();
    }

    [Command(requiresAuthority = false)]
    private void CmdInteract()
    {
        if (isOn)
        {
            isOn = false;
            RpcTurnOff();
        }
        else
        {
            isOn = true;
            RpcTurnOn();
        }
    }

    [ClientRpc]
    private void RpcTurnOn()
    {
        isOn = true;
        leverHandle.transform.DOLocalRotate(leverOpenPosition.localEulerAngles, 0.5f);
    }

    [ClientRpc]
    private void RpcTurnOff()
    {
        isOn = false;
        leverHandle.transform.DOLocalRotate(leverClosePosition.localEulerAngles, 0.5f);
    }
}
