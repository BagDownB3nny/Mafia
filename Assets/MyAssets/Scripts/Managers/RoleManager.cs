using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

public class RoleManager : MonoBehaviour
{

    public static Dictionary<RoleName, int> roleDict = new();
    public static RoleManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    [Server]
    public void AssignRoles()
    {
        List<Player> players = PlayerManager.instance.GetAllPlayers();
        List<RoleName> rolesToAssign = new();

        // Create list of roles based on roleDict quantities
        foreach (var kvp in roleDict)
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                rolesToAssign.Add(kvp.Key);
            }
        }

        // Shuffle the roles
        rolesToAssign = rolesToAssign.OrderBy(x => Random.value).ToList();

        Debug.Log("Roles to assign: " + string.Join(", ", rolesToAssign));

        // Assign roles to players
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetRole(rolesToAssign[i]);
        }
    }
}
