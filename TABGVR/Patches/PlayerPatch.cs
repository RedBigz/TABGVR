using HarmonyLib;
using TABGVR.Player;

namespace TABGVR.Patches;

[HarmonyPatch(typeof(global::Player), nameof(global::Player.Start))]
public class PlayerPatch
{
    public static void Postfix(global::Player __instance)
    {
        PlayerManager playerManager = new(__instance.gameObject);

        if (!playerManager.playerIsClient) return; // run the rest if we are the player
        
        // run code on player load here
    }
}