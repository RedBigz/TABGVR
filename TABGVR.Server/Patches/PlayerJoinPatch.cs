using Epic.OnlineServices.Auth;
using HarmonyLib;
using Landfall.Network;

namespace TABGVR.Server;

[HarmonyPatch(typeof(RoomInitRequestCommand), nameof(RoomInitRequestCommand.Run))]
public class PlayerJoinPatch
{
    public bool Verified = false;
    
    public static void Postfix(ServerClient world, byte playerID)
    {
        var store = new PlayerStore(world.GameRoomReference.FindPlayer(playerID)); // Create PlayerStore

        store.Identification = new(store, world);
        store.Identification.Interrogate();
    }
}