using Mirror;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Interactable : NetworkBehaviour
{

    [Client]
    public void Highlight()
    {
        Debug.Log("Highlighting " + gameObject.name);
        gameObject.GetComponent<Outline>().enabled = true;
    }

    public void Unhighlight()
    {
        Debug.Log("Unhighlighting " + gameObject.name);
        gameObject.GetComponent<Outline>().enabled = false;
    }

    virtual public void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }
}
