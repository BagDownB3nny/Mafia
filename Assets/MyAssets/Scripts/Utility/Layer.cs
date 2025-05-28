using UnityEngine;

// Single authority for all layer <> mask conversions.
// Update the strings if your Project Settings › Tags & Layers use different spellings.
public static class Layer
{
    private static readonly string[] names =
    {
        "Default",                 // 0
        "TransparentFX",           // 1
        "Ignore Raycast",          // 2
        "Ground",                  // 3
        "Water",                   // 4
        "UI",                      // 5
        "Seer",                    // 6
        "Guardian",                // 7
        "Mafia",                   // 8
        "Ghost",                   // 9
        "Player",                  // 10
        "GoThroughGroundPlayer",   // 11
        "Corpse",                  // 12
        "GhostPassable",          // 13
        "NameTag",                // 14
        "CharacterRenderTexture" // 15
    };
    // For assignment of singular layers to objects
    public static int Index(this LayerName layer) =>
        LayerMask.NameToLayer(names[(int)layer]);

    // For masking of existing layer
    public static int Mask(this LayerName layer) =>
        1 << layer.Index();
    // For masking of existing layers
    public static int Mask(params LayerName[] layers)
    {
        int m = 0;
        foreach (var l in layers) m |= 1 << l.Index();
        return m;
    }

    public static bool Contains(this int mask, LayerName layer) =>
        (mask & layer.Mask()) != 0;

    public static void SetLayerRecursively(GameObject gameObject, LayerName layer)
    {
        int index = layer.Index();
        foreach (Transform child in gameObject.transform)
        {
            SetLayerRecursively(child.gameObject, index);
        }
    }

    public static void SetLayerRecursively(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in gameObject.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
