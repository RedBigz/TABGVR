using HarmonyLib;
using TABGVR.Player;
using UltimateIK;
using UnityEngine;

namespace TABGVR.Patches;

[HarmonyPatch(typeof(Holding), nameof(Holding.LateUpdate))]
class KinematicsPatch
{
    static void Postfix(Holding __instance)
    {
        if (!Controllers.LeftHand || !Controllers.RightHand || !Controllers.Head) return;
        if (__instance.player != global::Player.localPlayer) return;

        // Plugin.Logger.LogInfo(
        //     $"KP {__instance.player} / Head: {Controllers.Head.transform.position} / Left: {Controllers.LeftHand.transform.position} / Right: {Controllers.RightHand.transform.position}");

        __instance.rightHand.GetComponentInChildren<Collider>().enabled = false;
        __instance.leftHand.GetComponentInChildren<Collider>().enabled = false;

        __instance.rightHand.MovePosition(__instance.rightHand.position + Controllers.RightHand.transform.position -
                                          Controllers.Head.transform.position +
                                          Camera.current.transform.position -
                                          __instance.rightHand.transform.GetChild(0).position);

        __instance.leftHand.MovePosition(__instance.leftHand.position + Controllers.LeftHand.transform.position -
                                         Controllers.Head.transform.position +
                                         Camera.current.transform.position -
                                         __instance.leftHand.transform.GetChild(0).position);
    }
}