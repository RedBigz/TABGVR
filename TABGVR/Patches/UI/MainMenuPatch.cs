using HarmonyLib;
using Landfall.TABG.UI.MainMenu;
using TABGVR.Util;
using UnityEngine;
using UnityEngine.UI;

namespace TABGVR.Patches.UI;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
public static class MainMenuPatch
{
    public static void Postfix(MainMenuManager instance)
    {
        var globalCanvas = GameObject.Find("/GlobalCanvas");
        globalCanvas.transform.SetParent(instance.transform.parent);
        
        var matchmakingCanvas = GameObject.Find("/MatchmakingCanvas");
        matchmakingCanvas.transform.SetParent(instance.transform.parent);
        
        UIPorter.Shebang(instance.gameObject);
        UIPorter.Shebang(globalCanvas);
        UIPorter.Shebang(matchmakingCanvas);
        
        instance.gameObject.AddComponent<RectMask2D>();
        globalCanvas.AddComponent<RectMask2D>();
        matchmakingCanvas.AddComponent<RectMask2D>();
        
        var parent = instance.transform.parent;
        parent.localPosition = Vector3.forward;
        parent.localRotation = Quaternion.identity;
    }
}