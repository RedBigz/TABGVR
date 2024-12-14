using Epic.OnlineServices.Auth;
using HarmonyLib;
using Landfall.Network;

namespace TABGVR.Server;

[HarmonyPatch(typeof(RoomInitRequestCommand), nameof(RoomInitRequestCommand.Run))]
public class PlayerJoinPatch
{
    public static void Postfix(ServerClient world, byte playerID)
    {
        new PlayerStore(world.GameRoomReference.FindPlayer(playerID)); // Create PlayerStore
    }
}