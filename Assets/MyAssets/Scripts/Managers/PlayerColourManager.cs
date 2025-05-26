using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

public class PlayerColourManager : NetworkBehaviour
{
    public static PlayerColourManager instance;

    // https://mokole.com/palette.html
    // This website helps to generate a distinct set of colours
    // which I used to generate 16 distinct colours
    public static Color[] allColours = new Color[]
        {
            HexToColor("#2f4f4f"),    // darkslategray
            HexToColor("#191970"),    // midnightblue
            HexToColor("#006400"),     // darkgreen
            HexToColor("#bdb76b"),     // darkkhaki
            HexToColor("#ff0000"),     // red
            HexToColor("#ffa500"),     // orange
            HexToColor("#ffff00"),     // yellow
            HexToColor("#c71585"),     // mediumvioletred
            HexToColor("#0000cd"),     // mediumblue
            HexToColor("#00ff00"),     // lime
            HexToColor("#00fa9a"),     // mediumspringgreen
            HexToColor("#00ffff"),     // aqua
            HexToColor("#d8bfd8"),     // thistle
            HexToColor("#ff00ff"),     // fuchsia
            HexToColor("#1e90ff"),     // dodgerblue
            HexToColor("#fa8072")      // salmon
        };

    public readonly SyncDictionary<int, Color> playerColours = new SyncDictionary<int, Color>();

    // This dictionary is used to store player colours across scenes
    public static Dictionary<int, Color> playerColoursDict = new Dictionary<int, Color>();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            PopulateSyncDictionary();
        }
        else
        {
            Debug.LogWarning("More than one instance of PlayerColourManager found!");
            Destroy(gameObject);
            return;
        }
    }

    private void PopulateSyncDictionary()
    {
        playerColours.Clear();
        foreach (var playerColour in playerColoursDict)
        {
            playerColours.Add(playerColour.Key, playerColour.Value);
        }
    }

    private static Color HexToColor(string hex)
    {
        hex = hex.TrimStart('#');
        float r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        float b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
        return new Color(r, g, b);
    }

    [Server]
    public void OnPlayerJoinedLobby(Player player, int connectionId)
    {
        Color colour;
        if (playerColoursDict.ContainsKey(connectionId))
        {
            colour = playerColoursDict[connectionId];
        }
        else
        {
            colour = GetUnusedColour();
        }

        PlayerColour playerColour = player.GetComponent<PlayerColour>();
        playerColour.SetColor(colour);
    }

    [Server]
    public void OnPlayerJoinedGame(Player player, int connectionId)
    {
        Color colour = playerColoursDict[connectionId];
        PlayerColour playerColour = player.GetComponent<PlayerColour>();
        playerColour.SetColor(colour);
    }

    public void DebugLogPlayerColours()
    {
        Debug.Log("Debug logging player colours");
        foreach (var playerColour in playerColours)
        {
            Debug.Log($"Player {playerColour.Key} has colour {playerColour.Value}");
        }
    }

    [Server]
    public void OnPlayerLeftLobby(NetworkConnection conn)
    {
        int connectionId = conn.connectionId;
        if (playerColours.ContainsKey(connectionId))
        {
            playerColours.Remove(connectionId);
        }
    }

    [Server]
    public void OnPlayerChangedColour(Player player, Color newColour)
    {
        int connectionId = player.connectionToClient.connectionId;
        if (playerColours.ContainsKey(connectionId))
        {
            if (playerColours[connectionId] == newColour)
            {
                Debug.Log("Player colour is already set to this colour, not changing");
                return;
            }
            playerColours[connectionId] = newColour;
            playerColoursDict[connectionId] = newColour;
        }
        else
        {
            Debug.Log("Assigning new colour to player");
            playerColours.Add(connectionId, newColour);
            playerColoursDict.Add(connectionId, newColour);
        }
    }

    [Server]
    public Color GetColour(int connectionId)
    {
        if (!playerColours.ContainsKey(connectionId))
        {
            // Im using white as a null value since I cant return null
            return Color.white;
        }
        return playerColours[connectionId];
    }


    // Util methods
    public Color GetUnusedColour()
    {
        foreach (Color colour in allColours)
        {
            bool isUsed = false;
            foreach (var playerColour in playerColours)
            {
                if (playerColour.Value == colour)
                {
                    isUsed = true;
                    break; // Colour is already used
                }
            }
            if (!isUsed)
            {
                return colour; // Return the first unused colour
            }
        }
        return Color.white; // Return white if no colours are available
    }
}
