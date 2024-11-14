using System.Collections;
using HarmonyLib;
using Landfall.TABG.UI;
using TABGVR.Player;
using UnityEngine;
using UnityEngine.UI;

namespace TABGVR.Patches;

[HarmonyPatch(typeof(InventoryUI), nameof(InventoryUI.Start))]
public class UIPatch
{
    /// <summary>
    /// Patches the inventory to make it visible in world space.
    /// </summary>
    /// <param name="__instance"><see cref="InventoryUI"/></param>
    [HarmonyPrefix]
    public static bool FixInventoryUI(InventoryUI __instance)
    {
        Plugin.Logger.LogInfo($"UI Patched: {__instance.gameObject.name}");

        __instance.transform.parent = global::Player.localPlayer.m_cameraMovement.transform.Find("CameraRotationX");

        __instance.transform.localPosition = Vector3.forward;
        __instance.transform.localRotation = Quaternion.identity;

        var inventoryObject = __instance.transform.Find("Inventory").gameObject;
        var canvas = inventoryObject.GetComponent<Canvas>();

        canvas.renderMode = RenderMode.WorldSpace; // make world space

        inventoryObject.GetComponent<CanvasScaler>().enabled = false;

        var rectTransform = inventoryObject.GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one * 0.0008f;

        return true;
    }
}