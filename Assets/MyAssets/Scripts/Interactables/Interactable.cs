using Mirror;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Interactable : NetworkBehaviour
{

    [Client]
    public void Highlight()
    {
        gameObject.GetComponent<Outline>().enabled = true;
    }

    public void Unhighlight()
    {
        gameObject.GetComponent<Outline>().enabled = false;
    }

    virtual public void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }
}
