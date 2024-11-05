using UnityEngine;
using UnityEngine.SpatialTracking;

namespace TABGVR.Player;

public static class Controllers
{
    public static GameObject LeftHand, RightHand, Head;

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
    }
}