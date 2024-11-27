using HarmonyLib;

#if TASM
using TASM.Common;
using TASM.Common.Helpers;
#elif CITRUSLIB
using BepInEx;
using CitrusLib;
#endif

#if TASM
[assembly: GamePlugin(name: "TABGVR", version: "1.0.0", author: "RedBigz")]
#endif

namespace TABGVR.Server;

#if CITRUSLIB
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("citrusbird.tabg.citruslib", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BaseUnityPlugin
#elif TASM
public class Plugin
#endif
{
#if CITRUSLIB
    internal static CitLog ModLogger;
#endif

#if TASM
    [PluginMain]
    public static void Main()
#elif CITRUSLIB
    private void Awake()
#endif
    {
#if CITRUSLIB
        // citrus init stuff
        ModLogger = new("TABGVR", ConsoleColor.Magenta);
#endif

#if TASM
        Logging.Log(Logging.LogLevel.Info, "TABGVR", "Loaded!");
#elif CITRUSLIB
        ModLogger.Log("Loaded!");
#endif

        Harmony harmony = new("com.redbigz.TABGVRServer");
        harmony.PatchAll();
    }
}