using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

// If server tells it a number, it must follow
// It can tell the server a new number 
public class RoleNumberSetter : NetworkBehaviour
{

    [Header("UI")]
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private GameObject leftArrowButton;
    [SerializeField] private GameObject rightArrowButton;

    [Header("Role")]
    [SerializeField] private RoleName role;

    public override void OnStartClient()
    {
        // Disable arrows for clients
        if (isServer) return;

        RoleSettingsMenu.instance.roleDict.OnSet += OnRoleDictSet;
        RoleSettingsMenu.instance.roleDict.OnAdd += OnRoleDictAdd;
        SyncClientUI();

        leftArrowButton.SetActive(false);
        rightArrowButton.SetActive(false);
    }
 
    public override void OnStartServer()
    {
        RoleSettingsMenu.instance.roleDict.OnSet += OnRoleDictSet;
        RoleSettingsMenu.instance.roleDict.OnAdd += OnRoleDictAdd;

        int startingNumber = RoleSettingsMenu.instance.roleDict[role];
        SetNumber(startingNumber);
    }

    [Client]
    public void SyncClientUI()
    {
        int number = RoleSettingsMenu.instance.roleDict[role];
        SetNumber(number);
    }

    public void SetNumber(int number)
    {
        numberText.text = number.ToString();
        if (isClientOnly) return;
        
        if (number == 0)
        {
            leftArrowButton.SetActive(false);
        } 
        else if (!leftArrowButton.activeSelf) {
            leftArrowButton.SetActive(true);
        }
    }

    public void OnRoleDictSet(RoleName role, int oldNumber)
    {
        if (role == this.role)
        {
            int newNumber = RoleSettingsMenu.instance.roleDict[role];
            SetNumber(newNumber);
        }
    }

    public void OnRoleDictAdd(RoleName role)
    {
        if (role == this.role)
        {
            int newNumber = RoleSettingsMenu.instance.roleDict[role];
            SetNumber(newNumber);
        }
    }

    [Server]
    public void OnLeftArrowClick()
    {
        int oldNumber = RoleSettingsMenu.instance.roleDict[role];
        int newNumber = oldNumber - 1;
        if (newNumber == 0)
        {
            leftArrowButton.SetActive(false);
        }
        RoleSettingsMenu.instance.SetRole(role, oldNumber - 1);
    }

    [Server]
    public void OnRightArrowClick()
    {
        int oldNumber = RoleSettingsMenu.instance.roleDict[role];
        int newNumber = oldNumber + 1;
        if (newNumber == 1)
        {
            leftArrowButton.SetActive(true);
        }
        RoleSettingsMenu.instance.SetRole(role, newNumber);
    }
}
