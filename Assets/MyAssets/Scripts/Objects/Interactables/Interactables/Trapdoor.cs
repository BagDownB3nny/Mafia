using DG.Tweening;
using UnityEngine;

public class Trapdoor : MonoBehaviour
{
    [SerializeField] private GameObject leftTrapdoor;
    [SerializeField] private GameObject rightTrapdoor;
    [SerializeField] private Transform rightTrapdoorOpenPosition;
    [SerializeField] private Transform rightTrapdoorClosedPosition;
    [SerializeField] private Transform leftTrapdoorOpenPosition;
    [SerializeField] private Transform leftTrapdoorClosedPosition;

    public void OpenTrapdoor()
    {
        leftTrapdoor.transform.DORotateQuaternion(leftTrapdoorOpenPosition.rotation, 0.6f).SetEase(Ease.OutBounce);
        rightTrapdoor.transform.DORotateQuaternion(rightTrapdoorOpenPosition.rotation, 0.6f).SetEase(Ease.OutBounce);
    }

    public void CloseTrapdoor()
    {
        leftTrapdoor.transform.DORotateQuaternion(leftTrapdoorClosedPosition.rotation, 0.5f).SetEase(Ease.Linear);
        rightTrapdoor.transform.DORotateQuaternion(rightTrapdoorClosedPosition.rotation, 0.5f).SetEase(Ease.Linear);
    }
}
