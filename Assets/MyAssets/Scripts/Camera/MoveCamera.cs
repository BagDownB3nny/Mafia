using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform playerDefaultCameraPosition;
    public Transform currentCameraPosition = null;

    public void SetCameraPosition(Transform newCameraPosition)
    {
        currentCameraPosition = newCameraPosition;
    }

    public void SetToPlayerCameraPosition()
    {
        currentCameraPosition = playerDefaultCameraPosition;
    }

    void Update()
    {
        if (currentCameraPosition == null) return;
        transform.position = currentCameraPosition.position;
    }
}
