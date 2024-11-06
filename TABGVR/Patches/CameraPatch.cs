using HarmonyLib;
using TABGVR.Player;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace TABGVR.Patches;

[HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.Start))]
public class CameraPatch
{
    public static void Postfix(PlayerCamera __instance)
    {
        Plugin.Logger.LogInfo($"Camera idle: {__instance.gameObject.name}");
        
        var playerManager = PlayerManager.FromCamera(__instance.GetComponent<Camera>());

        var gameObject = new GameObject("VRCamera")
        {
            transform =
            {
                parent = __instance.transform.parent.parent.parent.parent.parent.parent.parent,
                position = __instance.transform.position,
            },
            tag = "MainCamera",
            layer = __instance.gameObject.layer
        };

        __instance.enabled = false;

        var camera = gameObject.AddComponent<Camera>();
        camera.stereoTargetEye = StereoTargetEyeMask.Both;
        camera.enabled = true;
        camera.nearClipPlane = 0.01f;
        // camera.targetDisplay = 1;

        var driver = gameObject.AddComponent<TrackedPoseDriver>();
        driver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
        driver.poseSource = TrackedPoseDriver.TrackedPose.Head;
        
        // var playerDriver = playerManager.playerRoot.AddComponent<PlayerDriver>();
        // playerDriver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
        // playerDriver.poseSource = TrackedPoseDriver.TrackedPose.Head;

        // Object.Destroy(gameObject.GetComponent<HighlightingRenderer>());
    }
}