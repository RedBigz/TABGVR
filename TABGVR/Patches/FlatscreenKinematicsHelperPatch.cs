using HarmonyLib;
using Landfall.Network;
using TABGVR.Network;
using TABGVR.PatchAttributes;
using UnityEngine.SceneManagement;

namespace TABGVR.Patches;

[HarmonyPatch(typeof(Holding), nameof(Holding.Start))]
[FlatscreenPatch]
public class FlatscreenKinematicsHelperPatch
{
    private static void Postfix(Holding __instance)
    {
        if (__instance.m_player == global::Player.localPlayer) return;
        if (!PhotonServerConnector.IsNetworkMatch) return;
        if (SceneManager.GetActiveScene().name == "MainMenu") return;
        
        __instance.gameObject.AddComponent<NetKinematics>();
    }
}