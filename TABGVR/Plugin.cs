using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TABGVR.Patches;
using TABGVR.Player;
using UnityEngine.SceneManagement;

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
        harmony.PatchAll();

        SceneManager.sceneLoaded += (_, _) => Controllers.Setup();
    }
}