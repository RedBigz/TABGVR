using HarmonyLib;
using TABGVR.Util;
using UnityEngine;
using UnityEngine.UI;

namespace TABGVR.Patches.UI;

[HarmonyPatch(typeof(InventoryUI), nameof(InventoryUI.Start))]
public static class InventoryUIPatch
{
    /// <summary>
    /// Patches the inventory to make it visible in world space.
    /// </summary>
    public static bool Prefix(InventoryUI instance)
    {
        instance.transform.SetParent(
            global::Player.localPlayer.m_cameraMovement.transform.Find("CameraRotationX"));

        instance.transform.localPosition = Vector3.forward;
        instance.transform.localRotation = Quaternion.identity;

        // *inventory*
        
        // shebang it
        UIPorter.Shebang(instance.transform.Find("Inventory").gameObject);
        
        // fix header being weird af
        instance.characterRT.transform.parent.Find("Header").GetComponent<RectTransform>().localPosition =
            new Vector3(0, 620, 0);
        
        // *game ui*
        instance.gameUI.Start();
        var gameUI = instance.transform.Find("GameUI").gameObject;
        gameUI.AddComponent<RectMask2D>(); // hide stuff that's normally offscreen
        UIPorter.Shebang(gameUI);
        
        // *screen space canvas*
        var ssc = instance.transform.Find("ScreenSpaceCanvas").Find("Canvas").gameObject;
        ssc.AddComponent<RectMask2D>();
        UIPorter.Shebang(ssc);
        
        return true;
    }
}