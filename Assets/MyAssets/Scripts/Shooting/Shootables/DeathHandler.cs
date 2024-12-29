using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Apply a random rotation to the GameObject
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        // Apply a force to the GameObject to make it fall
        GetComponent<Rigidbody>().AddForce(Vector3.down * 10, ForceMode.Impulse);
        // Set material color to red
        GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
