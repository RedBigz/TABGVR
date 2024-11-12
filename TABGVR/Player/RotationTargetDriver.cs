using UnityEngine;
using UnityEngine.SpatialTracking;

namespace TABGVR.Player;

public class RotationTargetDriver : TrackedPoseDriver
{
    public override void SetLocalTransform(Vector3 newPosition, Quaternion newRotation, PoseDataFlags poseFlags)
    {
        base.SetLocalTransform(newPosition, Quaternion.Euler(0, newRotation.eulerAngles.y, 0), poseFlags);
    }
}