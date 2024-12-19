using System.Collections.Generic;

namespace TABGVR.Network;

/// <summary>
/// Static class that contains a dictionary of NetworkStores.
/// </summary>
public static class NetworkStoreList
{
    public static Dictionary<byte, NetworkStore> NetworkStores = new();
}