using DeepSky.Haze;
using HarmonyLib;
using HighlightingSystem;
using TABGVR.Player;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SpatialTracking;

namespace TABGVR.Patches.CameraPatches;

[HarmonyPatch(typeof(PlayerCamera))]
public class CameraPatch
{
    /// <summary>
    ///     Creates VR Camera after <see cref="PlayerCamera" /> starts.
    /// </summary>
    /// <param name="__instance">
    ///     <see cref="PlayerCamera" />
    /// </param>
    [HarmonyPatch(nameof(PlayerCamera.Start))]
    [HarmonyPostfix]
    public static void Start(PlayerCamera __instance)
    {
        Plugin.Logger.LogInfo($"Camera idle: {__instance.gameObject.name}");

        var playerManager = PlayerManager.FromCamera(__instance.cam);

        var gameObject = new GameObject("VRCamera")
        {
            transform =
            {
                parent = playerManager.Player.m_cameraMovement.transform,
                position = __instance.transform.position
            },
            tag = "MainCamera",
            layer = __instance.gameObject.layer
        };

        __instance.enabled = false;

        var camera = gameObject.AddComponent<UnityEngine.Camera>();
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

        var highlight = gameObject.AddComponent<HighlightingRenderer>();
        var highlightReference = __instance.GetComponent<HighlightingRenderer>();
        highlight.downsampleFactor = highlightReference.downsampleFactor;
        highlight.iterations = highlightReference.iterations;
        highlight.blurMinSpread = highlightReference.blurMinSpread;
        highlight.blurSpread = highlightReference.blurSpread;
        highlight.blurIntensity = highlightReference.blurIntensity;

        // constrain the camera
        ConstraintSource source = new()
        {
            sourceTransform = gameObject.transform,
            weight = 1f
        };

        var constraint = __instance.gameObject.AddComponent<RotationConstraint>();
        constraint.AddSource(source);

        constraint.constraintActive = true;
    }
}