using Landfall.Network;

namespace TABGVR.Server;

public class PlayerStore
{
    private readonly TABGPlayerServer playerServer;

    public static Dictionary<byte, PlayerStore> Stores = new();

    public static PlayerStore? GetPlayerStore(TABGPlayerServer playerServer) =>
        GetPlayerStore(playerServer.PlayerIndex);

    public static PlayerStore? GetPlayerStore(byte playerIndex) =>
        Stores.ContainsKey(playerIndex) ? Stores[playerIndex] : null;

    public PlayerStore(TABGPlayerServer playerServer)
    {
        this.playerServer = playerServer;

        Stores[playerServer.PlayerIndex] = this;
    }
}