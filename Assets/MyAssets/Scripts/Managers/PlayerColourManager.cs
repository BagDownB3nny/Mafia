using UnityEngine;
using Mirror;

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

    SyncDictionary<int, Color> playerColours = new SyncDictionary<int, Color>();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("More than one instance of PlayerColourManager found!");
            Destroy(gameObject);
            return;
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
    public void OnPlayerJoinedLobby(NetworkConnection conn)
    {
        int connectionId = conn.connectionId;
        Color colour = GetUnusedColour();
        playerColours[connectionId] = colour;
        PlayerColour playerColour = conn.identity.GetComponent<PlayerColour>();
        playerColour.SetColor(colour);
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
    public void OnPlayerChangedColour(NetworkConnection conn, Color newColour)
    {
        int connectionId = conn.connectionId;
        if (playerColours.ContainsKey(connectionId))
        {
            playerColours[connectionId] = newColour;
            PlayerColour playerColour = conn.identity.GetComponent<PlayerColour>();
            playerColour.SetColor(newColour);
        }
    }

    [Server]
    public void OnGameStart()
    {
        // Set all players to their assigned colours
        foreach (var playerColour in playerColours)
        {
            int connectionId = playerColour.Key;
            Color colour = playerColour.Value;
            PlayerColour player = NetworkServer.connections[connectionId].identity.GetComponent<PlayerColour>();
            player.SetColor(colour);
        }
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
