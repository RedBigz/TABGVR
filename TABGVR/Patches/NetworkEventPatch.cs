using System.IO;
using HarmonyLib;
using Landfall.Network;
using TABGVR.Network;
using UnityEngine;

namespace TABGVR.Patches;

[HarmonyPatch(typeof(ServerConnector), nameof(ServerConnector.OnEvent))]
public class NetworkEventPatch
{
    public static bool Interrogated = false;

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

                        store.HmdPosition = new Vector3(
                            (float)reader.ReadDouble(),
                            (float)reader.ReadDouble(),
                            (float)reader.ReadDouble()
                        );

                        store.HmdRotation = new Vector3(
                            (float)reader.ReadDouble(),
                            (float)reader.ReadDouble(),
                            (float)reader.ReadDouble()
                        );

                        store.LeftHandPosition = new Vector3(
                            (float)reader.ReadDouble(),
                            (float)reader.ReadDouble(),
                            (float)reader.ReadDouble()
                        );

                        store.LeftHandRotation = new Vector3(
                            (float)reader.ReadDouble(),
                            (float)reader.ReadDouble(),
                            (float)reader.ReadDouble()
                        );

                        store.RightHandPosition = new Vector3(
                            (float)reader.ReadDouble(),
                            (float)reader.ReadDouble(),
                            (float)reader.ReadDouble()
                        );

                        store.RightHandRotation = new Vector3(
                            (float)reader.ReadDouble(),
                            (float)reader.ReadDouble(),
                            (float)reader.ReadDouble()
                        );
                    }
                }

                return false;
            default:
                return true;
        }
    }
}