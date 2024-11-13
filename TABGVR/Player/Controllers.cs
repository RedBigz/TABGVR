using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;

namespace TABGVR.Player;

public static class Controllers
{
    public static GameObject LeftHand, RightHand, Head;


    /// <summary>
    ///     <see cref="InputDevice" />s of each hand.
    /// </summary>
    public static InputDevice LeftHandXR, RightHandXR;

    /// <summary>
    ///     Left hand Vector3 in relative space.
    /// </summary>
    public static Vector3 LeftHandFromGameCamera =>
        LeftHand.transform.position - Head.transform.position + Camera.current.transform.position;

    /// <summary>
    ///     Right hand Vector3 in relative space.
    /// </summary>
    public static Vector3 RightHandFromGameCamera =>
        RightHand.transform.position - Head.transform.position + Camera.current.transform.position;

    /// <summary>
    ///     Sets up VR Controllers.
    /// </summary>
    public static void Setup()
    {
        Head = new GameObject("TABGVR_HMD");
        LeftHand = new GameObject("TABGVR_LeftHand");
        RightHand = new GameObject("TABGVR_RightHand");

        var headDriver = Head.AddComponent<TrackedPoseDriver>();
        headDriver.deviceType = TrackedPoseDriver.DeviceType.GenericXRDevice;
        headDriver.poseSource = TrackedPoseDriver.TrackedPose.Head;
        headDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;

        var leftHandDriver = LeftHand.AddComponent<TrackedPoseDriver>();
        leftHandDriver.deviceType = TrackedPoseDriver.DeviceType.GenericXRController;
        leftHandDriver.poseSource = TrackedPoseDriver.TrackedPose.LeftPose;
        leftHandDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;

        var rightHandDriver = RightHand.AddComponent<TrackedPoseDriver>();
        rightHandDriver.deviceType = TrackedPoseDriver.DeviceType.GenericXRController;
        rightHandDriver.poseSource = TrackedPoseDriver.TrackedPose.RightPose;
        rightHandDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;

        var rightControllers = new List<InputDevice>();
        var leftControllers = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.HeldInHand |
            InputDeviceCharacteristics.Controller, rightControllers);

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Left | InputDeviceCharacteristics.HeldInHand |
            InputDeviceCharacteristics.Controller, leftControllers);

        RightHandXR = rightControllers[0];
        LeftHandXR = leftControllers[0];
    }
}