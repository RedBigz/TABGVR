using System.Collections;
using System.IO;
using HarmonyLib;
using Landfall.Network;
using TABGVR.Input;
using TABGVR.Network;
using TABGVR.Player;
using TABGVR.Player.Mundanities;
using UnityEngine;
using UnityEngine.XR;

namespace TABGVR.Patches.Interactions;

[HarmonyPatch(typeof(Holding))]
internal class KinematicsPatch
{
    internal static bool GripAvailable;
    internal static bool Gripping;

    /// <summary>
    ///     Sets up hand connection for joint by removing properties that cause issues.
    /// </summary>
    /// <param name="joint">Hand Joint</param>
    private static void SetupConnection(Rigidbody joint)
    {
        // joint.GetComponentInChildren<Collider>().enabled = false;

        var arm = joint.transform.parent.Find(joint.gameObject.name.Replace("Hand", "Arm")).gameObject;

        // disable gravity in arms
        joint.useGravity = false;
        arm.GetComponent<Rigidbody>().useGravity = false;

        joint.isKinematic = false;
        arm.GetComponent<Rigidbody>().isKinematic = false;

        foreach (var animationObject in joint.GetComponents<AnimationObject>()) Object.Destroy(animationObject);
        foreach (var animationObject in arm.GetComponents<AnimationObject>()) Object.Destroy(animationObject);

        foreach (var collisionChecker in joint.GetComponents<CollisionChecker>()) Object.Destroy(collisionChecker);
        foreach (var collisionChecker in arm.GetComponents<CollisionChecker>()) Object.Destroy(collisionChecker);
    }

    /// <summary>
    ///     Updates <paramref name="joint" />'s position to <paramref name="controller" />.
    /// </summary>
    /// <param name="joint">Hand Joint</param>
    /// <param name="controller">VR Controller made by <see cref="Controllers" /></param>
    private static void UpdateConnection(Rigidbody joint, GameObject controller)
    {
        var controllerRelativeToGameCamera = controller.transform.position - Controllers.Head.transform.position +
                                             Camera.current.transform.position;

        // if (Vector3.Distance(joint.position, controllerRelativeToGameCamera) < 0.1f) return;

        joint.MovePosition(joint.position + controllerRelativeToGameCamera -
                           joint.transform.GetChild(0).position);
    }

    /// <summary>
    ///     Position left hand to left hand attachment point on gun (handguard) instead of the VR controller to avoid visual
    ///     weirdness.
    /// </summary>
    /// <param name="holding">Holding Script</param>
    private static void PositionLeftHandToHandguard(Holding holding)
    {
        holding.leftHand.MovePosition(holding.leftHand.position +
                                      (holding.heldObject.leftHandPos ?? holding.heldObject.rightHandPos).position -
                                      holding.leftHand.transform.GetChild(0).position);
    }

    /// <summary>
    ///     Sets up hands for VR IK after Holding.Start.
    /// </summary>
    /// <param name="__instance">Holding Script</param>
    [HarmonyPatch(nameof(Holding.Start))]
    [HarmonyPostfix]
    private static void StartPostfix(Holding __instance)
    {
        if (__instance.m_player == global::Player.localPlayer)
        {
            SetupConnection(__instance.rightHand);
            SetupConnection(__instance.leftHand);
        }
        else if (PhotonServerConnector.IsNetworkMatch)
            __instance.gameObject.AddComponent<NetKinematics>();
    }

    /// <summary>
    ///     Runs aiming and positioning for arms and guns after <see cref="Holding.Update" />.
    /// </summary>
    /// <param name="__instance">Holding Script</param>
    [HarmonyPatch(nameof(Holding.Update))]
    [HarmonyPostfix]
    private static void UpdatePostfix(Holding __instance)
    {
        if (!Controllers.LeftHand || !Controllers.RightHand || !Controllers.Head) return;
        if (__instance.player != global::Player.localPlayer) return;

        // Plugin.Logger.LogInfo(
        //     $"KP {__instance.player} / Head: {Controllers.Head.transform.position} / Left: {Controllers.LeftHand.transform.position} / Right: {Controllers.RightHand.transform.position}");

        // held will have hand positions which will be exploited here
        var heldObject = Grenades.SelectedGrenade?.GetComponent<HoldableObject>() ?? __instance.heldObject;

        UpdateConnection(__instance.rightHand, Controllers.RightHand);

        if (__instance.heldObject)
        {
            var rightHold = heldObject.rightHandPos;
            var leftHold = heldObject.leftHandPos;

            Controllers.LeftHandXR.TryGetFeatureValue(CommonUsages.grip, out var leftGrip);

            var otherHold = leftHold ?? rightHold;

            if (leftGrip > VRControls.TriggerDeadZone)
            {
                if (GripAvailable) Gripping = true;
            }
            else
            {
                Gripping = false;
            }

            GripAvailable =
                !Gripping && Vector3.Distance(otherHold.position, Controllers.LeftHandFromGameCamera) < 0.1f;

            // look at left controller if gripping, or else just use the right controller rotation
            heldObject.gameObject.transform.rotation = Gripping && leftHold
                ? Quaternion.LookRotation(
                    Controllers.LeftHand.transform.position - Controllers.RightHand.transform.position)
                : Controllers.RightHand.transform.rotation *
                  Quaternion.Euler(90f + rightHold.localRotation.x, rightHold.localRotation.y, 0f);

            var toMove = Controllers.RightHandFromGameCamera +
                heldObject.gameObject.transform.position - rightHold.position;

            var rigidBody = heldObject.GetComponent<Rigidbody>();

            rigidBody.isKinematic = false;
            rigidBody.useGravity = false;

            heldObject.transform.position = toMove;
        }
        else
        {
            Gripping = false;
        }

        if (Gripping) PositionLeftHandToHandguard(__instance);
        else UpdateConnection(__instance.leftHand, Controllers.LeftHand);
    }

    private static float _updateCounter;

    /// <summary>
    ///     Sends aiming and positioning for arms and guns over the network after <see cref="Holding.FixedUpdate" />.
    /// </summary>
    /// <param name="__instance">Holding Script</param>
    [HarmonyPatch(nameof(Holding.FixedUpdate))]
    [HarmonyPostfix]
    private static void FixedUpdatePostfix(Holding __instance)
    {
        _updateCounter++;
        _updateCounter %= 50f / 20f;

        if (_updateCounter != 0) return;

        if (!PhotonServerConnector.IsNetworkMatch) return;

        var packet = new byte[8 * 18];

        using (MemoryStream stream = new(packet))
        {
            using (BinaryWriter writer = new(stream))
            {
                void WriteVector(Vector3 vector)
                {
                    writer.Write((double)vector.x);
                    writer.Write((double)vector.y);
                    writer.Write((double)vector.z);
                }

                var heldObject = Grenades.SelectedGrenade?.GetComponent<HoldableObject>() ?? __instance.heldObject;

                WriteVector(Controllers.Head.transform.position);

                WriteVector(Controllers.Head.transform.rotation.eulerAngles);

                WriteVector(Gripping
                    ? (heldObject.leftHandPos ?? heldObject.rightHandPos).position -
                    Camera.current.transform.position + Controllers.Head.transform.position
                    : Controllers.LeftHand.transform.position);

                WriteVector(Controllers.LeftHand.transform.rotation.eulerAngles);

                WriteVector(Controllers.RightHand.transform.position);

                if (heldObject)
                    WriteVector(Gripping && heldObject.leftHandPos
                        ? Quaternion.LookRotation(Controllers.LeftHand.transform.position -
                                                  Controllers.RightHand.transform.position).eulerAngles
                        : (Controllers.RightHand.transform.rotation * Quaternion.Euler(
                            90f + heldObject.rightHandPos.localRotation.x,
                            heldObject.rightHandPos.localRotation.y, 0f)).eulerAngles);
                else
                    WriteVector(Vector3.zero);
            }
        }

        ServerConnector.m_ServerHandler.SendMessageToServer((EventCode)PacketCodes.ControllerMotion, packet, false);
    }

    /// <summary>
    ///     Cancels Holding.ReachForPoint from running.
    /// </summary>
    [HarmonyPatch(nameof(Holding.ReachForPoint))]
    [HarmonyPrefix]
    private static bool ReachForPointCanceller()
    {
        return false;
    }

    /// <summary>
    ///     Method that does nothing.
    /// </summary>
    /// <returns>IEnumerator that breaks on start</returns>
    private static IEnumerator DoNothing()
    {
        yield break;
    }

    /// <summary>
    ///     Cancels Holding.HoldWeaponStill from running.
    /// </summary>
    /// <param name="__result">
    ///     <see cref="DoNothing" />
    /// </param>
    [HarmonyPatch(nameof(Holding.HoldweaponStill))]
    [HarmonyPrefix]
    private static bool HoldWeaponStillCanceller(ref IEnumerator __result)
    {
        __result = DoNothing();
        return false;
    }

    /// <summary>
    ///     Cancels PlayerIKHandler.LateUpdate from running.
    /// </summary>
    [HarmonyPatch(typeof(PlayerIKHandler), nameof(PlayerIKHandler.LateUpdate))]
    [HarmonyPrefix]
    private static bool IKCanceller()
    {
        return false;
    }
}