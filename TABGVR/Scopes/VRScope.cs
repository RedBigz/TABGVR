using System;
using System.Collections.Generic;
using DeepSky.Haze;
using Unity.XR.CoreUtils;
using UnityEngine;
using Object = System.Object;

namespace TABGVR.Scopes;

public class VRScope : MonoBehaviour
{
    private RedDot redDot;
    private bool success;
    private const float Multiplier = 1 / (0.4f * 3.5f * 0.12f * 0.7f);

    // scope sizes (magnification -> fov)
    private static readonly Dictionary<double, int> FOV = new()
    {
        { 8, 15 },
        { 4, 27 },
        { 2, 40 },
        { 0.5, 125 }
    };

    private void Start()
    {
        redDot = GetComponent<RedDot>();

        Plugin.Logger.LogInfo(transform.parent.name);
        if (!transform.parent.name.StartsWith("RedDot"))
        {
            var rt = new RenderTexture(768, 768, 32, RenderTextureFormat.ARGB32);
            rt.Create();

            var stringMagnification = transform.parent.name.Split("x")[0];
            var doubleMagnification = double.Parse(stringMagnification);

            var diff = Instantiate(
                AssetBundle.Bundle0.LoadAsset<GameObject>($"{stringMagnification}xDiff"), transform);

            diff.transform.localPosition = Vector3.zero;
            diff.transform.localRotation = Quaternion.identity;
            diff.transform.localScale = Vector3.one;

            var scopeMaterial = diff.transform.Find("ScopeLens").GetComponent<MeshRenderer>().material;
            var cam = diff.transform.Find("Camera").GetComponent<Camera>();

            scopeMaterial.SetTexture("_RenderTexture", rt);

            cam.targetTexture = rt;
            cam.gameObject.AddComponent<FlareLayer>();
            cam.gameObject.AddComponent<DS_HazeView>();
            cam.fieldOfView = FOV[doubleMagnification];

            success = true;
            return;
        }

        var lens = transform.Find("GameObject").Find("Model").Find("W_RedDot").Find("W_RedDot_Glass").gameObject;
        lens.SetActive(true); // enable lens glass

        lens.GetComponent<MeshRenderer>().material =
            AssetBundle.Bundle0.LoadAsset<Material>("Assets/TABGVR/Scopes/RedDot/Glass.mat");
        redDot.dotTransform.GetComponent<MeshRenderer>().material =
            AssetBundle.Bundle0.LoadAsset<Material>("Assets/TABGVR/Scopes/RedDot/RedDot.mat");
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