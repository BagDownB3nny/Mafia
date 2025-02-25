using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] private int PlayerLayer, GoThroughGroundPlayerLayer;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);
        if (other.gameObject.layer == PlayerLayer)
        {
            other.gameObject.layer = GoThroughGroundPlayerLayer;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == GoThroughGroundPlayerLayer)
        {
            other.gameObject.layer = PlayerLayer;
        }
    }
}
