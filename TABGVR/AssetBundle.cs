using System.Reflection;
using UnityEngine;

namespace TABGVR;

public static class AssetBundle
{
    public static void Load()
    {
        using var bundle0Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TABGVR.Assets.vr0.tabgvrasset");
        Bundle0 = UnityEngine.AssetBundle.LoadFromStream(bundle0Stream);
    }

    public static UnityEngine.AssetBundle Bundle0;
}