using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TABGVR.Patches;
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

    private void Awake()
    {
        Logger = base.Logger;

        Logger.LogInfo("TABGVR plugin loaded.");

        AntiCheatBypass.Bypass();

        XRLoader.LoadXR();

        Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);
        
#if DEBUG
        HarmonyFileLog.Enabled = true;

        Logger.LogInfo("Patched methods:");
        foreach (var patchedMethod in harmony.GetPatchedMethods())
        {
            Logger.LogInfo(patchedMethod.DeclaringType?.ToString());
        }
#endif
        
        harmony.PatchAll();

        SceneManager.sceneLoaded += (_, _) => Controllers.Setup();
        SceneManager.sceneUnloaded += (_) =>
        {
            UIPorter.UILeftHand = null;
            UIPorter.UIRightHand = null;
        };
    }
}