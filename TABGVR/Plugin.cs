using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using TABGVR.PatchAttributes;
using TABGVR.Patches.Misc;
using TABGVR.Player;
using TABGVR.Util;
using UnityEngine.SceneManagement;

#if DEBUG
using HarmonyLib.Tools;
#endif

namespace TABGVR;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;

    public static ConfigEntry<bool> SnapTurnEnabled;
    public static ConfigEntry<bool> AskOnStartup;
    public static ConfigEntry<bool> VREnabled;

    private void Awake()
    {
        SnapTurnEnabled = Config.Bind("Input", "SnapTurn", true, "Use snap turn instead of smooth turn.");
        AskOnStartup = Config.Bind("Input", "AskOnStartup", true, "Ask to use VR mode on startup.");
        VREnabled = Config.Bind("Input", "VREnabled", true,
            "If enabled, will always use VR mode if AskOnStartup is false.");

        Logger = base.Logger;

        Logger.LogInfo("TABGVR plugin loaded.");

        AntiCheatBypass.Bypass();

        Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);

#if DEBUG
        HarmonyFileLog.Enabled = true;
#endif

        if (AskOnStartup.Value)
            switch (Native.ShellMessageBox(IntPtr.Zero, IntPtr.Zero,
                        "Would you like to launch TABG in VR?\n\nMake sure your headset is connected and your launcher (Oculus Dash or SteamVR) is configured as the default OpenXR API.\nLaunching without VR will still let you see VR players' hand movements.",
                        "TABGVR", 0x1043))
            {
                case 2:
                    Process.GetCurrentProcess().Kill();
                    break;
                case 7:
                    VREnabled.Value = false;
                    break;
                case 6:
                    VREnabled.Value = true;
                    break;
            }

        var allTypes = Assembly.GetExecutingAssembly().GetTypes();

        if (VREnabled.Value)
        {
            // patch vr types
            foreach (var type in allTypes.Where(type => type.IsDefined(typeof(VRPatchAttribute))))
                harmony.PatchAll(type);
            
            XRLoader.LoadXR();
        }
        else
        {
            // patch flatscreen types
            foreach (var type in allTypes.Where(type => type.IsDefined(typeof(FlatscreenPatchAttribute))))
                harmony.PatchAll(type);
        }

        SceneManager.sceneLoaded += (_, _) => Controllers.Setup();
        SceneManager.sceneUnloaded += (_) =>
        {
            UIPorter.UILeftHand = null;
            UIPorter.UIRightHand = null;
        };
    }
}