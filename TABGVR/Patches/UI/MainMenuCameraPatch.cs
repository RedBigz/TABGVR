using DeepSky.Haze;
using HarmonyLib;
using TABGVR.Player;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SpatialTracking;

namespace TABGVR.Patches.UI;

[HarmonyPatch(typeof(CameraIdleMovement), nameof(CameraIdleMovement.Start))]
public static class MainMenuCameraPatch
{
    public static void Postfix(CameraIdleMovement instance)
    {
        var gameObject = new GameObject("VRCameraRoot")
        {
            transform =
            {
                parent = instance.transform.parent,
                position = Vector3.Scale(instance.transform.position, Vector3.forward + Vector3.right)
            }
        };

        var cameraObject = new GameObject("VRCamera")
        {
            transform = { parent = gameObject.transform },
            tag = "MainCamera",
            layer = instance.gameObject.layer
        };

        var oldCamera = instance.GetComponent<Camera>();
        oldCamera.enabled = false;
        instance.enabled = false;

        var camera = cameraObject.AddComponent<Camera>();
        camera.stereoTargetEye = StereoTargetEyeMask.Both;
        camera.enabled = true;
        camera.nearClipPlane = 0.01f;

        var driver = cameraObject.AddComponent<TrackedPoseDriver>();
        driver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
        driver.poseSource = TrackedPoseDriver.TrackedPose.Head;

        cameraObject.AddComponent<FlareLayer>();
        cameraObject.AddComponent<DS_HazeView>();

        var postProcessing = cameraObject.AddComponent<PostProcessLayer>();
        postProcessing.volumeTrigger = cameraObject.transform;
        postProcessing.volumeLayer = LayerMask.NameToLayer("Post");
        postProcessing.m_Resources = instance.GetComponent<PostProcessLayer>().m_Resources;

        camera.cullingMask = oldCamera.cullingMask;
        Controllers.VRFloor.transform.parent = gameObject.transform;

        // constrain the camera
        // ConstraintSource source = new()
        // {
        //     sourceTransform = gameObject.transform,
        //     weight = 1f
        // };
        //
        // var constraint = __instance.gameObject.AddComponent<RotationConstraint>();
        // constraint.AddSource(source);
        //
        // constraint.constraintActive = true;
    }
}