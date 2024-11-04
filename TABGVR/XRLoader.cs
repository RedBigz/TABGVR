using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;

namespace TABGVR;

public static class XRLoader
{
    internal static void LoadXR()
    {
        // a lot of this is borrowed (stolen) from daxcess' LCVR
        var generalSettings = ScriptableObject.CreateInstance<XRGeneralSettings>();
        var managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();

        var xrLoader = ScriptableObject.CreateInstance<OpenXRLoader>();
        
        OpenXRSettings.Instance.renderMode = OpenXRSettings.RenderMode.MultiPass;
        
        generalSettings.Manager = managerSettings;
        
        managerSettings.loaders.Clear();
        managerSettings.loaders.Add(xrLoader);

        XRGeneralSettings.AttemptInitializeXRSDKOnLoad();
        XRGeneralSettings.AttemptStartXRSDKOnBeforeSplashScreen();
        
        managerSettings.InitializeLoaderSync();
        
        List<XRDisplaySubsystem> displays = new();
        
        SubsystemManager.GetInstances(displays);
        
        var myDisplay = displays[0];
        myDisplay.Start();
    }
}