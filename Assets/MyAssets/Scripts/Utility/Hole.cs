using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] private int PlayerLayer, GoThroughGroundPlayerLayer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerName.Player.Index())
        {
            other.gameObject.layer = LayerName.GoThroughGroundPlayer.Index();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerName.GoThroughGroundPlayer.Index())
        {
            other.gameObject.layer = LayerName.Player.Index();
        }
    }
}
