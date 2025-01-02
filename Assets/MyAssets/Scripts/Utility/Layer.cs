using UnityEngine;
public static class Layer {
  public static void SetLayerChildren(GameObject gameObject, int layer) {
    gameObject.layer = layer;
    foreach (Transform child in gameObject.transform) {
      SetLayerChildren(child.gameObject, layer);
    }
  }
}