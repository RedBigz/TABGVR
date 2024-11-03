using System.Reflection;
using Epic.OnlineServices.AntiCheatClient;
using HarmonyLib;
using Landfall;

namespace TABGVR.Patches;

public class AntiCheatBypass
{
    internal static void Bypass()
    {
        Harmony.CreateAndPatchAll(typeof(AntiCheatBypass), "com.redbigz.AntiCheatBypass");
        
        Plugin.Logger.LogInfo("Just so you know, bypassing the Anti-Cheat cuts you off from official servers. Matchmaking has been disabled for your safety.");
    }
    
    [HarmonyPatch(typeof(Easy_AC_Client), nameof(Easy_AC_Client.SetACInterface))]
    [HarmonyPrefix]
    private static bool SetACInterface(AntiCheatClientInterface ACInterface)
    {
        Easy_AC_Client.m_Interface = ACInterface;
        return false;
    }

    [HarmonyPatch(typeof(MatchmakingHandler), nameof(MatchmakingHandler.StartSearch))]
    [HarmonyPrefix]
    private static bool StartSearch()
    {
        GlobalCanvasSingleton.Instance.UIMessageBox.QueueMessage("Matchmaking disabled due to Anti-Cheat safety. Please use a community server when playing TABGVR.");
        return false;
    }
}