using Epic.OnlineServices.AntiCheatClient;
using HarmonyLib;
using Landfall;

namespace TABGVR.Patches.Misc;

public class AntiCheatBypass
{
    /// <summary>
    ///     Bypasses the anticheat.<br />
    ///     <b>WARNING: THIS KICKS YOU OFF OFFICIAL SERVERS!</b>
    /// </summary>
    internal static void Bypass()
    {
        Harmony.CreateAndPatchAll(typeof(AntiCheatBypass), "com.redbigz.AntiCheatBypass");

        Plugin.Logger.LogInfo(
            "Just so you know, bypassing the Anti-Cheat will cause issues on official servers. Matchmaking is strongly discouraged.");
    }

    /// <summary>
    ///     Patches <see cref="ACInterface" /> to disable AC.
    /// </summary>
    /// <param name="ACInterface">
    ///     <see cref="ACInterface" />
    /// </param>
    [HarmonyPatch(typeof(Easy_AC_Client), nameof(Easy_AC_Client.SetACInterface))]
    [HarmonyPrefix]
    private static bool SetACInterface(AntiCheatClientInterface ACInterface)
    {
        Easy_AC_Client.m_Interface = ACInterface;
        return false;
    }

    /// <summary>
    ///     Patches <see cref="MatchmakingHandler" /> to raise a notification if you try to matchmake, as well as blocking the
    ///     matchmaking process.
    /// </summary>
    [HarmonyPatch(typeof(MatchmakingHandler), nameof(MatchmakingHandler.StartSearch))]
    [HarmonyPostfix]
    private static void StartSearch()
    {
        GlobalCanvasSingleton.Instance.UIMessageBox.QueueMessage(
            "Just a heads-up, you *will* get kicked from official servers after a couple of seconds due to your Anti-Cheat not running. Matchmaking is strongly discouraged. Please play on community servers instead.");
    }
}