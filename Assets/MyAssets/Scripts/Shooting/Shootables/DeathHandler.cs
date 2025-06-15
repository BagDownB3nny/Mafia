using Mirror;
using TMPro;
using UnityEngine;

public class CorpseHandler : NetworkBehaviour
{

    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    [SerializeField] private TMP_Text playerNameText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Apply a random rotation to simulate a death animation
        transform.Rotate(new Vector3(Random.Range(0, 180), Random.Range(0, 90), Random.Range(0, 180)));
        // Apply a force to simulate knockback
        GetComponent<Rigidbody>().AddForce(Vector3.up * 0.4f, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddForce(Vector3.back * 3, ForceMode.Impulse);
        // Set material color to red
        GetComponent<MeshRenderer>().material.color = Color.red;
        playerNameText.text = playerName;
    }

    [Client]
    public void OnNameChanged(string oldName, string newName)
    {
        playerNameText.text = newName;
    }
}
