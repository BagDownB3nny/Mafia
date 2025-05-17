using Mirror;
using UnityEngine;


public enum InteractableType
{
    AllRoles,
    Seer,
    Mafia,
}

[RequireComponent(typeof(Outline))]
public abstract class Interactable : NetworkBehaviour
{
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

    abstract public RoleName[] GetRolesThatCanInteract();

    protected RoleName[] GetAllRoles()
    {
        return new RoleName[]
        {
            RoleName.Villager,
            RoleName.Mafia,
            RoleName.Seer,
            RoleName.Guardian,
            RoleName.Medium,
            RoleName.SixthSense
        };
    }

    virtual public string GetInteractableText()
    {
        return null;
    }
}
