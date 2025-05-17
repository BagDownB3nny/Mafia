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

    public override void Interact()
    {
        CmdInteract();
    }

    public override RoleName[] GetRolesThatCanInteract()
    {
        return GetAllRoles();
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

    [Server]
    private void ServerMoveToOpen()
    {
        MoveToOpen();
        RpcMoveToOpen();
    }

    [ClientRpc]
    private void RpcMoveToOpen()
    {
        MoveToOpen();
    }

    private void MoveToOpen()
    {
        isOpenPosition = true;
        leverHandle.transform.DOLocalRotate(leverOpenPosition.localEulerAngles, 0.6f).SetEase(Ease.OutBounce);
        trapdoor.OpenTrapdoor();
    }

    [Server]
    private void ServerMoveToClose()
    {
        MoveToClose();
        RpcMoveToClose();
    }

    [ClientRpc]
    private void RpcMoveToClose()
    {
        MoveToClose();
    }

    private void MoveToClose()
    {
        isOpenPosition = false;
        leverHandle.transform.DOLocalRotate(leverClosePosition.localEulerAngles, 0.5f).SetEase(Ease.Linear);
        trapdoor.CloseTrapdoor();
    }
}
