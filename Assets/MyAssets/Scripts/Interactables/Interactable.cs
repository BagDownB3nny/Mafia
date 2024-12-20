using Mirror;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : NetworkBehaviour
{

    public abstract void OnHover();
    public abstract void OnUnhover();

    public string GetRat()
    {
        return "Rat";
    }

    // [Client]
    public void Highlight()
    {
        Debug.Log("Highlighting");
        Debug.Log(gameObject.GetComponent<Outline>().enabled);
        gameObject.GetComponent<Outline>().enabled = true;
        Debug.Log(gameObject.GetComponent<Outline>().enabled);
    }

    // [Client]
    public void Unhighlight()
    {
        Debug.Log("Unhighlighting");
        gameObject.GetComponent<Outline>().enabled = false;
    }


    virtual public void Interact()
    {
        Debug.Log("Interacting with " + gameObject.name);
    }
}
