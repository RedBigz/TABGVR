using HarmonyLib;
using Landfall.Network;

namespace TABGVR.Server.Patches;

[HarmonyPatch(typeof(CurrentGameWorldCommand), nameof(CurrentGameWorldCommand.InitNewServerPlayer))]
public class PlayerNewSpawnPatch
{
    public bool Verified = false;
    
    /// <summary>
    /// Runs when a player joins the game.
    /// </summary>
    /// <param name="world">Current <see cref="ServerClient"/></param>
    /// <param name="player"><see cref="TABGPlayerServer"/> who spawned</param>
    public static void Postfix(ServerClient world, TABGPlayerServer player)
    {
        world.WaitThenDoAction(3, () =>
        {
            var store = new PlayerStore(world.GameRoomReference.FindPlayer(player.PlayerIndex)); // Create PlayerStore

            store.Identification = new(store, world);
            store.Identification.Interrogate();
        });
    }
}