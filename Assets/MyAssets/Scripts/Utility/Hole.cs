using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] private int PlayerLayer, GoThroughGroundPlayerLayer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerName.LocalPlayer.Index())
        {
            Layer.SetLayerRecursively(other.gameObject, LayerName.GoThroughGroundPlayer.Index());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerName.GoThroughGroundPlayer.Index())
        {
            Layer.SetLayerRecursively(other.gameObject, LayerName.LocalPlayer.Index());
        }
    }
}
