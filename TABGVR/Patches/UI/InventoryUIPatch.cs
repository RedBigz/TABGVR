using HarmonyLib;
using TABGVR.PatchAttributes;
using TABGVR.Util;
using UnityEngine;
using UnityEngine.UI;

namespace TABGVR.Patches.UI;

[HarmonyPatch(typeof(InventoryUI), nameof(InventoryUI.Start))]
[VRPatch]
public static class InventoryUIPatch
{
    /// <summary>
    /// Patches the inventory to make it visible in world space.
    /// </summary>
    public static bool Prefix(InventoryUI __instance)
    {
        __instance.transform.SetParent(
            global::Player.localPlayer.m_cameraMovement.transform.Find("CameraRotationX"));

        __instance.transform.localPosition = Vector3.forward;
        __instance.transform.localRotation = Quaternion.identity;

        // *inventory*
        
        // shebang it
        UIPorter.Shebang(__instance.transform.Find("Inventory").gameObject);
        
        // fix header being weird af
        __instance.characterRT.transform.parent.Find("Header").GetComponent<RectTransform>().localPosition =
            new Vector3(0, 620, 0);
        
        // *game ui*
        __instance.gameUI.Start();
        var gameUI = __instance.transform.Find("GameUI").gameObject;
        gameUI.AddComponent<RectMask2D>(); // hide stuff that's normally offscreen
        UIPorter.Shebang(gameUI);
        
        // *screen space canvas*
        var ssc = __instance.transform.Find("ScreenSpaceCanvas").Find("Canvas").gameObject;
        ssc.AddComponent<RectMask2D>();
        UIPorter.Shebang(ssc);
        
        return true;
    }
}