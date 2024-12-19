using System;
using Landfall.Network;
using UnityEngine;

namespace TABGVR.Network;

public class NetKinematics : MonoBehaviour
{
    NetworkPlayer netPlayer;
    private Holding holding;

    /// <summary>
    /// Sets up joint for kinematics
    /// </summary>
    /// <param name="joint">Hand <see cref="Rigidbody"/> from <see cref="holding"/></param>
    private void Setup(Rigidbody joint)
    {
        var arm = joint.transform.parent.Find(joint.gameObject.name.Replace("Hand", "Arm")).gameObject;

        // disable gravity in arms
        joint.useGravity = false;
        arm.GetComponent<Rigidbody>().useGravity = false;

        joint.isKinematic = false;
        arm.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void Start()
    {
        netPlayer = GetComponent<NetworkPlayer>();
        holding = GetComponent<Holding>();
        
        Setup(holding.leftHand);
        Setup(holding.rightHand);
    }

    private void FixedUpdate()
    {
        if (!NetworkStoreList.NetworkStores.TryGetValue(netPlayer.Index, out var store)) return;

        var leftHandPosition = store.LeftHandPosition - store.HmdPosition + netPlayer.m_Camera.position;
        var rightHandPosition = store.RightHandPosition - store.HmdPosition + netPlayer.m_Camera.position;

#if DEBUG
        Plugin.Logger.LogInfo($"{leftHandPosition} / {rightHandPosition} / {store.HmdPosition}");
#endif

        var leftHandJoint = holding.leftHand;

        leftHandJoint.MovePosition(leftHandJoint.position + leftHandPosition -
                                   leftHandJoint.transform.GetChild(0).position);

        var rightHandJoint = holding.rightHand;

        rightHandJoint.MovePosition(rightHandJoint.position + rightHandPosition -
                                    rightHandJoint.transform.GetChild(0).position);

        if (holding.heldObject is null) return;
        
        var heldObject = holding.heldObject;
        
        heldObject.rig.isKinematic = false;
        heldObject.rig.useGravity = false;

        heldObject.gameObject.transform.rotation = Quaternion.Euler(store.RightHandRotation);

        heldObject.transform.position = rightHandPosition + holding.heldObject.gameObject.transform.position -
                                                holding.heldObject.rightHandPos.position;
    }
}