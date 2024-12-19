using HarmonyLib;
using Landfall.Network;

namespace TABGVR.Server.Patches;

[HarmonyPatch(typeof(RoomInitRequestCommand), nameof(RoomInitRequestCommand.Run))]
public class PlayerJoinPatch
{
    public bool Verified = false;
    
    /// <summary>
    /// Runs when a player joins the game.
    /// </summary>
    /// <param name="world">Current <see cref="ServerClient"/></param>
    /// <param name="playerID">ID of <see cref="TABGPlayerServer"/> who joined</param>
    public static void Postfix(ServerClient world, byte playerID)
    {
        var store = new PlayerStore(world.GameRoomReference.FindPlayer(playerID)); // Create PlayerStore

        store.Identification = new(store, world);
        store.Identification.Interrogate();
    }
}