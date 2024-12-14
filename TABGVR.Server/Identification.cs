using Epic.OnlineServices.AntiCheatCommon;
using Landfall.Network;

namespace TABGVR.Server;

public class Identification(PlayerStore playerStore, ServerClient world)
{
    public bool Verified = false;

    public void Interrogate()
    {
        world.SendMessageToClients((EventCode)PacketCodes.Interrogate, [], playerStore.playerServer.PlayerIndex,
            true); // send interrogation packet

        world.WaitThenDoAction(10, () =>
        {
            if (Verified || !ConfigManager.Config.VROnly) return;

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