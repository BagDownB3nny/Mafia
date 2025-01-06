using Mirror;
public static class Authority {
  public static void AssignAuthority(NetworkIdentity parent, NetworkConnectionToClient conn) {
    parent.AssignClientAuthority(conn);
    foreach (var child in parent.GetComponentsInChildren<NetworkIdentity>()) {
      if (child != parent) {
        child.AssignClientAuthority(conn);
      }
    }
  }
}