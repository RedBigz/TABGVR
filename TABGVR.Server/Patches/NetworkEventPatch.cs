using Epic.OnlineServices.AntiCheatCommon;
using HarmonyLib;
using Landfall.Network;

namespace TABGVR.Server.Patches;

[HarmonyPatch(typeof(ServerClient), nameof(ServerClient.HandleNetorkEvent))]
public class NetworkEventPatch
{
    /// <summary>
    /// Manages VR networking events received.
    /// </summary>
    /// <param name="__instance">The current <see cref="ServerClient"/></param>
    /// <param name="networkEvent">Packet received by the server</param>
    /// <returns></returns>
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

                if (player is null)
                {
#if DEBUG
                    LandLog.Log($"Player {networkEvent.SenderPlayerID} not found");
#endif
                    return false;
                }

                if (networkEvent.Buffer.Length !=
                    sizeof(float) * 3 * 6) // drop malformed packets and kick the responsible player
                {
                    PlayerKickCommand.Run(player,
                        __instance, KickReason.Invalid);
                    return false;
                }

                byte[] message = [player.PlayerIndex, ..networkEvent.Buffer];

                var recipients = from watcher in ServerChunks.Instance.GetWatchers(player.ChunkData)
                    select watcher.PlayerIndex;

                __instance.SendMessageToClients(networkEvent.Code, message, recipients.ToArray(), true);

                return false;
            default:
                return true;
        }
    }
}