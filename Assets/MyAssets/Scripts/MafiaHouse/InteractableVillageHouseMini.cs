// using UnityEngine;
// using Mirror;

// public class InteractableVillageHouseMini : Interactable
// {

//     [Header("House")]


//     [SyncVar]
//     public House house;

//     public string PlayerName => house?.player?.steamUsername;

//     [SyncVar(hook = nameof(OnIsMarkedChanged))]
//     private bool isMarked = false;

//     [Header("House status")]

//     [SyncVar]
//     public bool isOccupantDead = false;

//     [SyncVar]
//     public bool isHouseDestroyed = false;

//     [Server]
//     public void LinkHouse(House house)
//     {
//         this.house = house;
//     }

//     public override RoleName[] GetRolesThatCanInteract()
//     {
//         return new RoleName[] { RoleName.Mafia };
//     }

//     public override string GetInteractableText()
//     {
//         if (isOccupantDead)
//         {
//             return $"{PlayerName} is dead";
//         }
//         if (isHouseDestroyed)
//         {
//             return $"{PlayerName}'s house is destroyed. {PlayerName} must be hiding somewhere...";
//         }
//         if (isMarked)
//         {
//             return $"[R] Unmark {PlayerName}'s house";
//         }
//         else
//         {
//             return $"[R] Mark {PlayerName}'s house";
//         }
//     }

//     [Client]
//     public override void Interact()
//     {
//         if (isOccupantDead || isHouseDestroyed)
//         {
//             Debug.Log("Cannot interact with this house mini");
//             return;
//         }
//         if (!isMarked)
//         {
//             TargetDummyManager.instance.CmdSetSelectedTargetdummy(this, connectionToClient);
//         }
//         else
//         {
//             TargetDummyManager.instance.CmdSetSelectedTargetdummy(null, connectionToClient);
//         }
//     }

//     [Client]
//     private void OnIsMarkedChanged(bool oldMarked, bool newMarked)
//     {
//         Debug.Log($"Client marked: {newMarked}");
//         // TODO:
//         // Enable visual effect for marked house
//         // Disable visual effect for unmarked house
//     }

//     [Server]
//     public void MarkHouse()
//     {
//         isMarked = true;
//         house.Mark();
//     }

//     [Server]
//     public void UnmarkHouse()
//     {
//         isMarked = false;
//         house.Unmark();
//     }

//     [Server]
//     public void Remove()
//     {
//         NetworkServer.Destroy(gameObject);
//     }

// }
