using HarmonyLib;
using TABGVR.PatchAttributes;
using TABGVR.Scopes;
using UnityEngine;

namespace TABGVR.Patches.Scopes;

[HarmonyPatch(typeof(RedDot))]
[VRPatch, FlatscreenPatch]
public static class ScopePatch
{
    [HarmonyPatch(nameof(RedDot.Start))]
    [HarmonyPostfix]
    public static void Start(RedDot __instance)
    {
        __instance.dotTransform.gameObject.SetActive(true);
        __instance.enabled = false;
        __instance.gameObject.AddComponent<VRScope>();
    }
}