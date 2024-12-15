using System;
using Landfall.Network;
using UnityEngine;

namespace TABGVR.Network;

public class NetKinematics : MonoBehaviour
{
    NetworkPlayer netPlayer;
    private Holding holding;

    private void Start()
    {
        netPlayer = GetComponent<NetworkPlayer>();
        holding = GetComponent<Holding>();
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

        holding.heldObject.gameObject.transform.rotation = Quaternion.Euler(store.RightHandRotation);

        holding.heldObject.transform.position = rightHandPosition + holding.heldObject.gameObject.transform.position -
                                                holding.heldObject.rightHandPos.position;
    }
}