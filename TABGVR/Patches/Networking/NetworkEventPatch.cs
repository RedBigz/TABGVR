using System;
using System.IO;
using HarmonyLib;
using Landfall.Network;
using TABGVR.Network;
using UnityEngine;

namespace TABGVR.Patches.Networking;

[HarmonyPatch(typeof(ServerConnector), nameof(ServerConnector.OnEvent))]
public class NetworkEventPatch
{
    public static bool Interrogated = false;

    /// <summary>
    /// Manages VR networking events received.
    /// </summary>
    /// <param name="clientPackage">Packet received by the client</param>
    /// <returns></returns>
    public static bool Prefix(ClientPackage clientPackage)
    {
        // Plugin.Logger.LogInfo(clientPackage.Code);

        switch (clientPackage.Code)
        {
            case (EventCode)PacketCodes.Interrogate: // Interrogate
                ServerConnector.m_ServerHandler.SendMessageToServer((EventCode)PacketCodes.Interrogate, [], true);
                Interrogated = true;
                return false;
            case (EventCode)PacketCodes.ControllerMotion:
                using (MemoryStream stream = new(clientPackage.Buffer))
                {
                    using (BinaryReader reader = new(stream))
                    {
                        var playerIndex = reader.ReadByte();

                        if (!NetworkStoreList.NetworkStores.ContainsKey(playerIndex))
                            NetworkStoreList.NetworkStores.Add(playerIndex, new NetworkStore());

                        var store = NetworkStoreList.NetworkStores[playerIndex];

                        Vector3 ReadVector() => new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                        store.HmdPosition = ReadVector();
                        store.HmdRotation = ReadVector();
                        store.LeftHandPosition = ReadVector();
                        store.LeftHandRotation = ReadVector();
                        store.RightHandPosition = ReadVector();
                        store.RightHandRotation = ReadVector();
                    }
                }

                return false;
            default:
                return true;
        }
    }
}