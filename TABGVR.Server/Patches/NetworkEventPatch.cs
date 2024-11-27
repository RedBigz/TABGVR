using HarmonyLib;
using Landfall.Network;
using TASM.Common.Helpers;

namespace TABGVR.Server;

[HarmonyPatch(typeof(ServerClient), nameof(ServerClient.HandleNetorkEvent))]
public class NetworkEventPatch
{
    public static void Prefix(ServerPackage networkEvent)
    {
        // network events here
    }
}