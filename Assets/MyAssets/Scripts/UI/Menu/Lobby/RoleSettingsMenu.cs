using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class RoleSettingsMenu : Menu
{

    [Header("UI")]
    [SerializeField] private TMP_Text expectedPlayerCountText;
    [SerializeField] private PlayerNumberSetter playerNumberSetter;
    [SerializeField] private GameObject setDefaultRoleSettingsButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private PlayerCounter playerCounter;

    [Header("Role Settings")]
    // Key is the role, value is the number of players with that role
    public readonly SyncDictionary<RoleName, int> roleDict = new();

    [SyncVar(hook = nameof(OnExpectedPlayerCountChanged))]
    public int expectedPlayerCount;

    [Header ("Internal params")]
    public static bool expectedPlayerCountIncreased = false;
    public static bool roleSettingsChanged = false;

    // For testing purposes
    // public int fakePlayerCount;
    public static RoleSettingsMenu instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        } 

    }

    public override void OnStartServer()
    {

        if (RoleManager.roleDict.Count == 0)
        {
            Debug.Log("RoleManager.roleDict is empty");
            int playerCount = 1;
            SetExpectedPlayerCount(playerCount);
            SetDefaultRoleDict();
            SaveRoleSetting();
        } else {
            Debug.Log("RoleManager.roleDict is not empty");
            foreach (var item in RoleManager.roleDict)
            {
                roleDict[item.Key] = item.Value;
            }
            int playerCount = GetRoleCount();
            SetExpectedPlayerCount(playerCount);
        }
    }

    public override void OnStartClient()
    {
        if (isServer) return;

        confirmButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(true);
        setDefaultRoleSettingsButton.SetActive(false);
    }

    [Server]
    public void OnPlayerLeftLobby(NetworkConnectionToClient conn)
    {
        if (expectedPlayerCountIncreased) return;
        else if (roleSettingsChanged) {
            SetExpectedPlayerCount(expectedPlayerCount - 1);
        } else {
            SetExpectedPlayerCount(expectedPlayerCount - 1);
            SetDefaultRoleDict();
            SaveRoleSetting();
        }
    }

    [Client]
    public void OnExitClick()
    {
        base.Close();
    }

    [Client]
    public void OnExpectedPlayerCountChanged(int oldNumber, int newNumber)
    {
        expectedPlayerCountText.text = newNumber.ToString();
    }

    [Server]
    public void SetExpectedPlayerCount(int playerCount)
    {
        expectedPlayerCount = playerCount;
        expectedPlayerCountText.text = playerCount.ToString();
        CheckForValidRoleSettings();
        SetLobbyUIPlayerCount();

        int lobbyPlayerCount = PlayerManager.instance.GetPlayerCount();
        if (playerCount == lobbyPlayerCount)
        {
            playerNumberSetter.leftArrowButton.SetActive(false);
        }
    }

    public void SetLobbyUIPlayerCount()
    {
        int playerCount = PlayerManager.instance.GetPlayerCount();
        if (playerCount == 0) {
            playerCount = 1;
        }
        // int playerCount = fakePlayerCount;
        string text = playerCount.ToString() + " / " + expectedPlayerCount.ToString() + "\nplayers";
        Color color = playerCount < expectedPlayerCount ? Color.red : Color.white;
        playerCounter.SetPlayerCount(text, color);
    }


    // If expectedPlayerCount has been increased
    [ContextMenu ("Add player")]
    public void OnPlayerJoin()
    {
        if (IsChangingSettings())
        {
            OnPlayerJoinWhileHostChangingSettings();
        }
        else
        {
            OnPlayerJoinWhileHostNotChangingSettings();
        }
        SetLobbyUIPlayerCount();
    }

    public void OnPlayerJoinWhileHostNotChangingSettings()
    {
        // int playerCount = fakePlayerCount;
        int playerCount = PlayerManager.instance.GetPlayerCount();

        // Case 1: Lobby expects more players than current player count
        // Action: Do nothing since host expects the player
        if (playerCount <=  expectedPlayerCount) return;

        // Case 2: Lobby host has not changed any settings
        // Action: Increase expected player count and set default role distribution
        if (!roleSettingsChanged && !expectedPlayerCountIncreased)
        {
            SetExpectedPlayerCount(playerCount);
            SetDefaultRoleDict();
            SaveRoleSetting();
            return;
        } 

        // Case 3: Host set role settings that did not account for a higher player count
        // Action: Increase villager count by 1
        if (roleSettingsChanged && playerCount > expectedPlayerCount)
        {
            roleDict[RoleName.Villager] = roleDict[RoleName.Villager] + 1;
            SetExpectedPlayerCount(playerCount);
            SaveRoleSetting();
            return;
        }

        // Case 4: Role settings are default, but host did not expect more players
        if (!roleSettingsChanged && expectedPlayerCountIncreased)
        {
            SetExpectedPlayerCount(playerCount);
            SetDefaultRoleDict();
            SaveRoleSetting();
            return;
        }
    }

    [Server]
    public void OnPlayerJoinWhileHostChangingSettings()
    {
        // int playerCount = fakePlayerCount;
        int playerCount = PlayerManager.instance.GetPlayerCount();
        if (expectedPlayerCount < playerCount)
        {
            SetExpectedPlayerCount(playerCount);
        }
    }

    [Server]
    public bool IsChangingSettings()
    {
        return base.IsOpen;
    }

    [Server]
    public void SetDefaultRoleDict()
    {
        SyncDictionary<RoleName, int> newRoleDict = GetDefaultRoleDict(expectedPlayerCount);
        roleDict.Clear();
        foreach (var item in newRoleDict)
        {
            roleDict[item.Key] = item.Value;
        }
        CheckForValidRoleSettings();
    }

    public SyncDictionary<RoleName, int> GetDefaultRoleDict(int playerCount)
    {
        SyncDictionary<RoleName, int> roleDict = new();

        // Initialize all roles to 0
        foreach (RoleName role in System.Enum.GetValues(typeof(RoleName)))
        {
            roleDict[role] = 0;
        }

        for (int i = 0; i < playerCount; i++)
        {
            if (i == 0 || i == 7 || i == 11)
            {
                roleDict[RoleName.Mafia]++;
            }
            else if (i == 1 || i == 9 || i == 15)
            {
                roleDict[RoleName.Seer]++;
            }
            else if (i == 2 || i == 14)
            {
                roleDict[RoleName.Guardian]++;
            }
            else if (i == 3 || i == 13)
            {
                roleDict[RoleName.Medium]++;
            }
            else if (i == 4 || i == 10)
            {
                roleDict[RoleName.SixthSense]++;
            }
            else if (i == 5 || i == 6 || i == 8 || i == 12)
            {
                roleDict[RoleName.Villager]++;
            }
        }
        return roleDict;
    }

    [Server]
    public void SetRole(RoleName role, int number)
    {
        roleDict[role] = number;
        CheckForValidRoleSettings();
    }

    [Server]
    public void CheckForValidRoleSettings()
    {
        int roleCount = GetRoleCount();
        if (roleCount != expectedPlayerCount)
        {
           confirmButton.interactable = false;
           errorText.text = "Number of roles must match number of players";
        }
        else
        {
            confirmButton.interactable = true;
            errorText.text = "";
        }
    }

    // Gets total number of players in the game
    public int GetRoleCount()
    {
        int total = 0;
        foreach (int count in roleDict.Values)
        {
            total += count;
        }
        return total;
    }

    [Server]
    public void SaveRoleSetting()
    {
        foreach (var item in roleDict)
        {
            RoleManager.roleDict[item.Key] = item.Value;
        }
    }

    [Server]
    public void OnDefaultRoleSettingsClick()
    {
        SetDefaultRoleDict();
        SaveRoleSetting();
    }

    [Server]
    public void OnConfirmClick()
    {
        TimeSettingsMenu.instance.SaveTimeSetting();
        SaveRoleSetting();

        int playerCount = PlayerManager.instance.GetPlayerCount();

        // If player count is less than expected player count, it means the host increased the player count
        if (playerCount < expectedPlayerCount)
        {
            expectedPlayerCountIncreased = true;
        } else {
            expectedPlayerCountIncreased = false;
        }

        if (DictEquals(roleDict, GetDefaultRoleDict(expectedPlayerCount)))
        {
            roleSettingsChanged = false;
        }
        else
        {
            roleSettingsChanged = true;
        }
        base.Close();
    }

    [Client]
    public override void Close()
    {
        Debug.Log("Closing RoleSettingsMenu");
        CmdResetRoleMenu();
        base.Close();
    }

    [Command(requiresAuthority = false)]
    public void CmdResetRoleMenu()
    {
        Debug.Log("Resetting RoleSettingsMenu");
        roleDict.Clear();
        // Set roleDict to initial values from RoleManager
        foreach (var item in RoleManager.roleDict)
        {
            Debug.Log($"Setting role {item.Key} to {item.Value}");
            roleDict[item.Key] = item.Value;
        }
        expectedPlayerCount = PlayerManager.instance.GetPlayerCount();
    }

    public bool DictEquals(SyncDictionary<RoleName, int> dict1, SyncDictionary<RoleName, int> dict2)
    {
        foreach (var item in dict1)
        {
            if (dict2[item.Key] != item.Value) return false;
        }
        return true;
    }
}
