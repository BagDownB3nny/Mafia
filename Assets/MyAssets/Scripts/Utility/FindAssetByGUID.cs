using UnityEditor;
using UnityEngine;

public class FindAssetByGUID : EditorWindow
{
    private string guidInput = "";

    [MenuItem("Tools/Find Asset by GUID")]
    public static void ShowWindow()
    {
        GetWindow<FindAssetByGUID>("Find Asset by GUID");
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter Asset GUID", EditorStyles.boldLabel);
        guidInput = EditorGUILayout.TextField("GUID:", guidInput);

        if (GUILayout.Button("Find Asset"))
        {
            if (string.IsNullOrEmpty(guidInput) || guidInput.Length != 32)
            {
                Debug.LogError("Invalid GUID format. Must be 32 characters.");
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath(guidInput);
            if (!string.IsNullOrEmpty(path))
            {
                Object asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (asset != null)
                {
                    EditorGUIUtility.PingObject(asset);
                    Selection.activeObject = asset;
                    Debug.Log($"Found asset: {asset.name} at {path}", asset);
                }
                else
                {
                    Debug.LogError($"Asset exists at {path} but couldn't be loaded.");
                }
            }
            else
            {
                Debug.LogError("No asset found with this GUID.");
            }
        }
    }
}