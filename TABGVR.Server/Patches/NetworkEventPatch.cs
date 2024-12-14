using Epic.OnlineServices.AntiCheatCommon;
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
                var store = PlayerStore.GetPlayerStore(
                    __instance.GameRoomReference.FindPlayer(networkEvent.SenderPlayerID));

                if (store?.Identification != null) store.Identification.Verified = true;

                return false;
            case (EventCode)PacketCodes.ControllerMotion:
                var player = __instance.GameRoomReference.FindPlayer(networkEvent.SenderPlayerID);
                
                if (networkEvent.Buffer.Length != 8 * 18) // drop malformed packets and kick the responsible player
                {
                    PlayerKickCommand.Run(player,
                        __instance, KickReason.Invalid);
                    return false;
                }

                byte[] message = [player.PlayerIndex, ..networkEvent.Buffer];
                
                __instance.SendMessageToClients(networkEvent.Code, message, byte.MaxValue, false);

                return false;
            default:
                return true;
        }
    }
}