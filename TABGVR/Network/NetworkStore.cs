using UnityEngine;

namespace TABGVR.Network;

/// <summary>
/// Local network store for replicated players.
/// </summary>
public class NetworkStore
{
    public Vector3 HmdPosition = Vector3.zero;
    public Vector3 HmdRotation = Vector3.zero;

    public Vector3 LeftHandPosition = Vector3.zero;
    public Vector3 LeftHandRotation = Vector3.zero;

    public Vector3 RightHandPosition = Vector3.zero;
    public Vector3 RightHandRotation = Vector3.zero;
}