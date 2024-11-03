using HarmonyLib;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace TABGVR.Patches;

[HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.Start))]
public class CameraPatch
{
    public static void Postfix(PlayerCamera __instance)
    {
        Plugin.Logger.LogInfo($"Camera idle: {__instance.gameObject.name}");
        
        var gameObject = __instance.gameObject;
        gameObject.transform.localPosition = new Vector3(0, -2, 0);
        
        var camera = gameObject.GetComponent<Camera>();
        camera.stereoTargetEye = StereoTargetEyeMask.Both;
        // camera.targetDisplay = 1;
        
        var driver = gameObject.AddComponent<TrackedPoseDriver>();
        driver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
        driver.poseSource = TrackedPoseDriver.TrackedPose.Head;
    }
}