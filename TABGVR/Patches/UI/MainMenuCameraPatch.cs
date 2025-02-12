using System.Linq;
using DeepSky.Haze;
using HarmonyLib;
using TABGVR.PatchAttributes;
using TABGVR.Patches.CameraPatches;
using TABGVR.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.SpatialTracking;

namespace TABGVR.Patches.UI;

[HarmonyPatch(typeof(CameraIdleMovement), nameof(CameraIdleMovement.Start))]
[VRPatch]
public static class MainMenuCameraPatch
{
    public static void Postfix(CameraIdleMovement __instance)
    {
        var gameObject = new GameObject("VRCameraRoot")
        {
            transform =
            {
                parent = __instance.transform.parent,
                position = Vector3.Scale(__instance.transform.position, Vector3.forward + Vector3.right)
            }
        };

        var cameraObject = new GameObject("VRCamera")
        {
            transform = { parent = gameObject.transform },
            tag = "MainCamera",
            layer = __instance.gameObject.layer
        };

        var oldCamera = __instance.GetComponent<Camera>();
        // __instance.enabled = false;

        var camera = cameraObject.AddComponent<Camera>();
        camera.stereoTargetEye = StereoTargetEyeMask.Both;
        camera.enabled = true;
        camera.nearClipPlane = 0.01f;

        CameraPatch.TransferCommandBuffers(oldCamera, camera, CameraEvent.BeforeImageEffectsOpaque);
        CameraPatch.TransferCommandBuffers(oldCamera, camera, CameraEvent.BeforeImageEffects);

        cameraObject.AddComponent<FlareLayer>();
        cameraObject.AddComponent<DS_HazeView>();

        camera.cullingMask = oldCamera.cullingMask;
        Controllers.VRFloor.transform.parent = gameObject.transform;

        // var postProcessing = cameraObject.AddComponent<PostProcessLayer>();
        // postProcessing.volumeTrigger = gameObject.transform;
        // postProcessing.volumeLayer = LayerMask.NameToLayer("Post");
        // postProcessing.m_Resources = __instance.GetComponent<PostProcessLayer>().m_Resources;

        var driver = cameraObject.AddComponent<TrackedPoseDriver>();
        driver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
        driver.poseSource = TrackedPoseDriver.TrackedPose.Head;
            
        var volume = Object.FindObjectsOfType<PostProcessVolume>().First(vol => vol.name == "POST");
        volume.sharedProfile.RemoveSettings<DepthOfField>(); // pesky dof ruining the menu

        volume.StartCoroutine(CameraPatch.WaitToDisableCamera(oldCamera, 0.5f)); // we have a monobehaviour here so its easy to call the disable camera routine

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