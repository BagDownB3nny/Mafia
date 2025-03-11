using Mirror;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : NetworkBehaviour
{

    public abstract void OnHover();
    public abstract void OnUnhover();

    // [Client]
    public void Highlight()
    {
        gameObject.GetComponent<Outline>().enabled = true;
    }

    // [Client]
    public void Unhighlight()
    {
        gameObject.GetComponent<Outline>().enabled = false;
    }


    virtual public void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }
}
