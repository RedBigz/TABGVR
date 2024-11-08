using HarmonyLib;
using TABGVR.Player;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace TABGVR.Patches;

[HarmonyPatch(typeof(global::Player))]
public class PlayerPatch
{
    private static Transform _rotationTarget;
    private static PlayerManager _playerManager;

    [HarmonyPatch(nameof(global::Player.Start))]
    [HarmonyPostfix]
    public static void Start(global::Player __instance)
    {
        _playerManager = new(__instance.gameObject);

        if (!_playerManager.playerIsClient) return; // run the rest if we are the player

        // run code on player load here

        _rotationTarget = _playerManager.playerRoot.transform.Find("CameraMovement").Find("CameraRotationX");
        // .Find("RotationTarget");

        var playerDriver = _rotationTarget.gameObject.AddComponent<RotationTargetDriver>();

        playerDriver.trackingType = TrackedPoseDriver.TrackingType.RotationOnly;
        playerDriver.poseSource = TrackedPoseDriver.TrackedPose.Head;

        _playerManager.playerRoot.AddComponent<VRControls>();
    }

    [HarmonyPatch(nameof(global::Player.Update))]
    [HarmonyPostfix]
    public static void Update(global::Player __instance)
    {
        if (!_playerManager.playerIsClient) return;

        var rigidBody = _playerManager.player.Torso.GetComponent<Rigidbody>();

        rigidBody?.MoveRotation(Quaternion.Euler(0, _rotationTarget.eulerAngles.y, 0));
    }
}