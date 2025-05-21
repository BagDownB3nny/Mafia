using UnityEngine;

public class FakePlayerModel : MonoBehaviour
{
    public static FakePlayerModel instance;

    public void Awake()
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

    public void SetColour(Color color)
    {
        // Assuming you have a Renderer component to change the color of the model
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(renderer.material);
            material.color = color;
            renderer.material = material;
        }
    }
}
