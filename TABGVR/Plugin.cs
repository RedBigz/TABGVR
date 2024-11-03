using BepInEx;
using BepInEx.Logging;

namespace TABGVR;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;

        Logger.LogInfo("TABGVR plugin loaded.");
    }
}
