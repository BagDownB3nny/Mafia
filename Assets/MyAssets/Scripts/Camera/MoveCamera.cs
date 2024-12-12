using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition = null;

    public void SetCameraPosition(Transform newCameraPosition)
    {
        cameraPosition = newCameraPosition;
    }

    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
