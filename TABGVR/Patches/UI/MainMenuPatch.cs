using HarmonyLib;
using Landfall.TABG.UI.MainMenu;
using TABGVR.PatchAttributes;
using TABGVR.Util;
using UnityEngine;
using UnityEngine.UI;

namespace TABGVR.Patches.UI;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
[VRPatch]
public static class MainMenuPatch
{
    public static void Postfix(MainMenuManager __instance)
    {
        var globalCanvas = GameObject.Find("/GlobalCanvas");
        globalCanvas.transform.SetParent(__instance.transform.parent);
        
        var matchmakingCanvas = GameObject.Find("/MatchmakingCanvas");
        matchmakingCanvas.transform.SetParent(__instance.transform.parent);
        
        UIPorter.Shebang(__instance.gameObject);
        UIPorter.Shebang(globalCanvas);
        UIPorter.Shebang(matchmakingCanvas);
        
        __instance.gameObject.AddComponent<RectMask2D>();
        globalCanvas.AddComponent<RectMask2D>();
        matchmakingCanvas.AddComponent<RectMask2D>();
        
        var parent = __instance.transform.parent;
        parent.localPosition = Vector3.forward;
        parent.localRotation = Quaternion.identity;
    }
}