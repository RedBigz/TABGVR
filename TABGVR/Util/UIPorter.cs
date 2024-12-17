using System;
using System.Linq;
using JetBrains.Annotations;
using TABGVR.Patches;
using TABGVR.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace TABGVR.Util;

internal static class UIPorter
{
    private static bool _eventSystemSetUp;

    [CanBeNull] internal static GameObject UISnapTurnBase;
    
    internal static GameObject UILeftHand;
    internal static GameObject UIRightHand;

    internal static XRRayInteractor UILeftHandInteractor;
    internal static XRRayInteractor UIRightHandInteractor;

    [CanBeNull] private static XRInteractorLineVisual _uiLeftHandVisual;
    [CanBeNull] private static XRInteractorLineVisual _uiRightHandVisual;

    internal static Vector2 DragPosition
    {
        get
        {
            RaycastResult hit;

            if (PlayerPatch.CurrentVRControls.RightTriggered)
                UIRightHandInteractor.TryGetCurrentUIRaycastResult(out hit);
            else if (PlayerPatch.CurrentVRControls.LeftTriggered)
                UILeftHandInteractor.TryGetCurrentUIRaycastResult(out hit);
            else return Vector2.zero;

            return hit.screenPosition;
        }
    }

    internal static bool InteractorVisuals
    {
        get
        {
            if (_uiLeftHandVisual is null || _uiRightHandVisual is null) return false;

            return _uiLeftHandVisual.enabled;
        }

        set
        {
            if (_uiLeftHandVisual is null || _uiRightHandVisual is null) return;

            _uiLeftHandVisual.enabled = value;
            _uiRightHandVisual.enabled = value;
        }
    }

    internal static void SetupInteractors(XRNode node)
    {
        if (!UISnapTurnBase)
        {
            UISnapTurnBase = new GameObject("TABGVR_UISnapTurnBase");
            UISnapTurnBase.transform.SetParent(Controllers.VRFloor.transform, false);
        }
        
        if (UILeftHand && node == XRNode.LeftHand) return;
        if (UIRightHand && node == XRNode.RightHand) return;

        GameObject interactionController;

        switch (node)
        {
            case XRNode.LeftHand:
                UILeftHand ??= new GameObject("TABGVR_UILeftHand");
                interactionController = UILeftHand;
                break;
            case XRNode.RightHand:
                UIRightHand ??= new GameObject("TABGVR_UIRightHand");
                interactionController = UIRightHand;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(node), node, null);
        }
        
        interactionController.transform.SetParent(UISnapTurnBase.transform);

        var controller = interactionController.AddComponent<XRController>();
        var interactor = interactionController.AddComponent<XRRayInteractor>();
        var lineVisual = interactionController.AddComponent<XRInteractorLineVisual>();
        var lineRenderer = interactionController.GetComponent<LineRenderer>();

        switch (node)
        {
            case XRNode.LeftHand:
                _uiLeftHandVisual = lineVisual;
                UILeftHandInteractor = interactor;
                break;
            case XRNode.RightHand:
                _uiRightHandVisual = lineVisual;
                UIRightHandInteractor = interactor;
                break;
        }

        interactor.rayOriginTransform.localEulerAngles = node switch
        {
            XRNode.LeftHand => new Vector3(60, 347, 90),
            XRNode.RightHand => new Vector3(60, 347, 270),
            _ => throw new ArgumentOutOfRangeException(nameof(node), node, null)
        };

        lineVisual.lineBendRatio = 1;
        lineVisual.invalidColorGradient = new Gradient()
        {
            mode = GradientMode.Blend,
            alphaKeys =
                new[] { new GradientAlphaKey(0.1f, 0), new GradientAlphaKey(0.1f, 1) },
            colorKeys =
                new[]
                {
                    new GradientColorKey(Color.white, 0),
                    new GradientColorKey(Color.white, 1)
                }
        };

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        controller.controllerNode = node;
        // InteractorMover.Interactors.Add(interactor);
    }

    internal static void SetupEventSystem()
    {
        if (_eventSystemSetUp) return;

        var eventSystem = SceneManager.GetActiveScene().GetRootGameObjects().First(o =>
                o.name == (SceneManager.GetActiveScene().name == "MainMenu" ? "NetworkClientMenu" : "MapObjects"))
            .transform.Find("EventSystem").gameObject;

        eventSystem.AddComponent<XRUIInputModule>();
        eventSystem.GetComponent<StandaloneInputModule>().enabled = false;
        
        UISnapTurnBase = null; // remove UISnapTurnBase if there's already one

        _eventSystemSetUp = true;
    }

    internal static void SetupCanvas(GameObject canvas)
    {
        canvas.layer = LayerMask.NameToLayer("Default");

        var canvasComponent = canvas.GetComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.WorldSpace;
        canvasComponent.worldCamera = Camera.main;

        var rectTransform = canvas.GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale =
            Vector3.one * 0.0008f; // i don't know why this specific number works, but it does ¯\_(ツ)_/¯

        canvas.AddComponent<TrackedDeviceGraphicRaycaster>();
    }

    internal static void Shebang(GameObject canvas)
    {
        SetupEventSystem();
        SetupInteractors(XRNode.LeftHand);
        SetupInteractors(XRNode.RightHand);
        SetupCanvas(canvas);
    }
}