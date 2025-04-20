// using UnityEditor;
// using UnityEngine;

// public static class FindAssetByFileID
// {
//     [MenuItem("Tools/Find Asset by FileID")]
//     public static void FindAsset()
//     {
//         // Explicitly declare as ulong to match targetObjectId
//         ulong targetFileID = 2095401278; // Note: 'ulong' instead of 'long'

//         string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

//         foreach (string assetPath in allAssetPaths)
//         {
//             Object mainAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);

//             if (mainAsset != null)
//             {
//                 GlobalObjectId globalId = GlobalObjectId.GetGlobalObjectIdSlow(mainAsset);

//                 // Now both sides are ulong (no ambiguity)
//                 if (globalId.targetObjectId == targetFileID)
//                 {
//                     Debug.Log($"Found asset: {assetPath}", mainAsset);
//                     EditorGUIUtility.PingObject(mainAsset);
//                     return;
//                 }
//             }
//         }

//         Debug.LogWarning($"No asset found with fileID: {targetFileID}");
//     }
// }