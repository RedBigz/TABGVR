using Epic.OnlineServices.AntiCheatCommon;
using Landfall.Network;

namespace TABGVR.Server;

/// <summary>
/// An API which detects VR players. Currently broken at the moment.
/// </summary>
/// <param name="playerStore">The player store to send packets to the player</param>
/// <param name="world">The current <see cref="ServerClient"/></param>
public class Identification(PlayerStore playerStore, ServerClient world)
{
    public bool Verified = false;

    public void Interrogate()
    {
        if (Verified || !ConfigManager.Config.VROnly) return;

        world.SendMessageToClients((EventCode)PacketCodes.Interrogate, [], playerStore.playerServer.PlayerIndex,
            true); // send interrogation packet

        world.WaitThenDoAction(10, () =>
        {
            if (Verified)
            {
#if CITRUSLIB
                Plugin.ModLogger.Log(
#elif TASM
                Logging.Log(Logging.LogLevel.Info, "TABGVR",
#endif
                    $"Successfully verified {playerStore.playerServer.PlayerName}.");

                return;
            }

#if CITRUSLIB
            CitrusLib.Citrus.SelfParrot(playerStore.playerServer,
#elif TASM
            TASM.Common.Helpers.Notification.Notify(playerStore.playerServer,
#endif
                "You seem to be running vanilla TABG. This server only supports VR to maintain fairness. You will be kicked in 5 seconds.");

            world.WaitThenDoAction(5,
                () => PlayerKickCommand.Run(playerStore.playerServer, world, KickReason.Invalid));
        });
    }
}