using UnityEngine;

public class ExecutionSpot : MonoBehaviour
{
    public static ExecutionSpot instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
