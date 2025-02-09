using System;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace TABGVR.Scopes;

public class VRRedDot : MonoBehaviour
{
    private RedDot redDot;
    private bool success;
    private const float Multiplier = 1 / (0.4f * 3.5f * 0.12f * 0.7f);

    private void Start()
    {
        redDot = GetComponent<RedDot>();
        
        var lens = transform.Find("GameObject").Find("Model").Find("W_RedDot").Find("W_RedDot_Glass").gameObject;
        lens.SetActive(true); // enable lens glass

        Debug.Log(1);
        var glassMaterial = AssetBundle.Bundle0.LoadAsset<Material>("Assets/TABGVR/Scopes/RedDot/Glass.mat");
        Debug.Log(2);
        var redDotMaterial = AssetBundle.Bundle0.LoadAsset<Material>("Assets/TABGVR/Scopes/RedDot/RedDot.mat");

        Debug.Log(3);
        lens.GetComponent<MeshRenderer>().material = glassMaterial;
        Debug.Log(3);
        redDot.dotTransform.GetComponent<MeshRenderer>().material = redDotMaterial;
    }

    private void Update()
    {
        // this method just bruteforces the positioning until it doesn't error :|
        if (success) return;

        try
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            var shootPosition = redDot.camMove.weaponHandler.m_holding.heldObject.rig.GetComponent<Gun>()
                .shootPositions;

            redDot.dotTransform.localPosition =
                Vector3.forward * (redDot.transform.InverseTransformPoint(shootPosition[0].position).z * Multiplier);

            redDot.dotTransform.localScale = Vector3.one * (0.006f * redDot.dotTransform.localPosition.z);

            success = true;
        }
        catch
        {
            // ignored
        }
    }
}