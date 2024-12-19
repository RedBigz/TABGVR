using Newtonsoft.Json;
using TABGVR.Server.Types;

namespace TABGVR.Server;

/// <summary>
/// Static class managing the config file.
/// </summary>
public static class ConfigManager
{
    public static readonly string ConfigFileName = "vr_config.json";
    public static Config Config { get; private set; }

    public static void LoadConfig()
    {
        if (!File.Exists(ConfigFileName)) File.WriteAllText(ConfigFileName, JsonConvert.SerializeObject(new Config(), Formatting.Indented));
        Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFileName))!;
    }
}