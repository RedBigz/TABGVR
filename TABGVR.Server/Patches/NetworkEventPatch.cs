using HarmonyLib;
using Landfall.Network;

namespace TABGVR.Server;

[HarmonyPatch(typeof(ServerClient), nameof(ServerClient.HandleNetorkEvent))]
public class NetworkEventPatch
{
    public static bool Prefix(ServerClient __instance, ServerPackage networkEvent)
    {
        switch (networkEvent.Code)
        {
            case (EventCode)PacketCodes.Interrogate:
                var store = PlayerStore.GetPlayerStore(__instance.GameRoomReference.FindPlayer(networkEvent.SenderPlayerID));

                if (store?.Identification != null) store.Identification.Verified = true;

                return false;
            default:
                return true;
        }
    }
}