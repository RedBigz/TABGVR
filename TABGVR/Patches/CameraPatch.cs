using DeepSky.Haze;
using HarmonyLib;
using TABGVR.Player;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SpatialTracking;

namespace TABGVR.Patches;

[HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.Start))]
public class CameraPatch
{
    public static void Postfix(PlayerCamera __instance)
    {
        Plugin.Logger.LogInfo($"Camera idle: {__instance.gameObject.name}");

        var playerManager = PlayerManager.FromCamera(__instance.cam);

        var gameObject = new GameObject("VRCamera")
        {
            transform =
            {
                parent = playerManager.playerRoot.transform.Find("CameraMovement"),
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

        gameObject.AddComponent<FlareLayer>();
        gameObject.AddComponent<DS_HazeView>();

        var postProcessing = gameObject.AddComponent<PostProcessLayer>();
        postProcessing.volumeTrigger = gameObject.transform;
        postProcessing.volumeLayer = LayerMask.NameToLayer("Post");
        postProcessing.m_Resources = __instance.GetComponent<PostProcessLayer>().m_Resources;

        // var playerDriver = playerManager.playerRoot.AddComponent<PlayerDriver>();
        // playerDriver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
        // playerDriver.poseSource = TrackedPoseDriver.TrackedPose.Head;
    }
}