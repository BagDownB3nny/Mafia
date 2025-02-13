using UnityEngine;
using Mirror;
using DG.Tweening;

public class Lever : Interactable
{
    public bool isOpenPosition = false;
    [SerializeField] private GameObject leverHandle;
    [SerializeField] private Transform leverOpenPosition;
    [SerializeField] private Transform leverClosePosition;
    [SerializeField] private Trapdoor trapdoor;

    public override void OnHover()
    {
        Highlight();
    }

    public override void OnUnhover()
    {
        Unhighlight();
    }

    public override void Interact()
    {
        CmdInteract();
    }

    [Command(requiresAuthority = false)]
    private void CmdInteract()
    {
        if (isOpenPosition)
        {
            isOpenPosition = false;
            RpcMoveToClose();
        }
        else
        {
            isOpenPosition = true;
            RpcMoveToOpen();
        }
    }

    [ClientRpc]
    private void RpcMoveToOpen()
    {
        isOpenPosition = true;
        leverHandle.transform.DOLocalRotate(leverOpenPosition.localEulerAngles, 0.6f).SetEase(Ease.OutBounce);
        trapdoor.OpenTrapdoor();
    }

    [ClientRpc]
    private void RpcMoveToClose()
    {
        isOpenPosition = false;
        leverHandle.transform.DOLocalRotate(leverClosePosition.localEulerAngles, 0.5f).SetEase(Ease.Linear);
        trapdoor.CloseTrapdoor();
    }
}
