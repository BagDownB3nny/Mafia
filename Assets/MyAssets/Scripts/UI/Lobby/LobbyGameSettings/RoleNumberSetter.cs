using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

// If server tells it a number, it must follow
// It can tell the server a new number 
public class RoleNumberSetter : NetworkBehaviour
{

    [Header("UI")]
    [SerializeField] public RoleSettingsUI roleSettingsUI;
    [SerializeField] public TMP_Text numberText;
    [SerializeField] public GameObject leftArrowButton;
    [SerializeField] public GameObject rightArrowButton;

    [Header("Role")]
    public RoleName role;

    public override void OnStartClient()
    {
        // Disable arrows for clients
        if (isServer) return;

        roleSettingsUI.roleDict.OnSet += OnRoleDictSet;
        roleSettingsUI.roleDict.OnAdd += OnRoleDictAdd;
        SyncClientUI();

        leftArrowButton.SetActive(false);
        rightArrowButton.SetActive(false);
    }
 
    public override void OnStartServer()
    {
        roleSettingsUI.roleDict.OnSet += OnRoleDictSet;
        roleSettingsUI.roleDict.OnAdd += OnRoleDictAdd;

        int startingNumber = roleSettingsUI.roleDict[role];
        SetNumber(startingNumber);
    }

    [Client]
    public void SyncClientUI()
    {
        int number = roleSettingsUI.roleDict[role];
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
            int newNumber = roleSettingsUI.roleDict[role];
            SetNumber(newNumber);
        }
    }

    public void OnRoleDictAdd(RoleName role)
    {
        if (role == this.role)
        {
            int newNumber = roleSettingsUI.roleDict[role];
            SetNumber(newNumber);
        }
    }

    [Server]
    public void OnLeftArrowClick()
    {
        int oldNumber = roleSettingsUI.roleDict[role];
        int newNumber = oldNumber - 1;
        if (newNumber == 0)
        {
            leftArrowButton.SetActive(false);
        }
        roleSettingsUI.SetRole(role, oldNumber - 1);
    }

    [Server]
    public void OnRightArrowClick()
    {
        int oldNumber = roleSettingsUI.roleDict[role];
        int newNumber = oldNumber + 1;
        if (newNumber == 1)
        {
            leftArrowButton.SetActive(true);
        }
        roleSettingsUI.SetRole(role, newNumber);
    }
}
