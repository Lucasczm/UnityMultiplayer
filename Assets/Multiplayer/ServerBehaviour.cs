using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;
using Multiplayer;
using System;
using System.Collections.Generic;

public class ServerBehaviour : MonoBehaviour
{
    public UdpNetworkDriver m_ServerDriver;
    private NativeList<NetworkConnection> m_connections;

    private JobHandle m_updateHandle;

    void Start()
    {
        // Create the server driver, bind it to a port and start listening for incoming connections
        m_ServerDriver = new UdpNetworkDriver(new INetworkParameter[0]);
        var addr = NetworkEndPoint.AnyIpv4;
        addr.Port = 9000;
        if (m_ServerDriver.Bind(addr) != 0)
            Debug.Log("Failed to bind to port 9000");
        else
            m_ServerDriver.Listen();

        m_connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    void OnDestroy()
    {
        // All jobs must be completed before we can dispose the data they use
        m_updateHandle.Complete();
        m_ServerDriver.Dispose();
        m_connections.Dispose();
    }

    void FixedUpdate()
    {
        // Update the NetworkDriver. It schedules a job so we must wait for that job with Complete
        m_ServerDriver.ScheduleUpdate().Complete();

        // Accept all new connections
        while (true)
        {
            var con = m_ServerDriver.Accept();
            // "Nothing more to accept" is signaled by returning an invalid connection from accept
            if (!con.IsCreated)
                break;
            m_connections.Add(con);
            // Debug.Log("Connected is " + con.InternalId);
            // var idData = new DataStreamWriter(4, Allocator.Temp);
            //  idData.Write(con.InternalId);
            // DataStreamReader strm;

            // m_ServerDriver.PopEventForConnection(con, out strm);
            //m_ServerDriver.Send(NetworkPipeline.Null, con, idData);
        }
        List<Player> clientList = ServerManager.GetClients();

        for (int i = 0; i < m_connections.Length; ++i)
        {
            DataStreamReader strm;
            NetworkEvent.Type cmd;
            // Pop all events for the connection
            while ((cmd = m_ServerDriver.PopEventForConnection(m_connections[i], out strm)) != NetworkEvent.Type.Empty)
            {

                if (cmd == NetworkEvent.Type.Data)
                {
                    var readerCtx = default(DataStreamReader.Context);
                    byte data = strm.ReadByte(ref readerCtx);
                    CMD command = (CMD)data;
                    //    // var player = ByteConverter.toPlayer(data);
                    //     Debug.Log("Server R Type :"+ command+" id: "+sizeof(byte));// + player.ID + " " + player.position);

                    switch (command)
                    {
                        case CMD.CONNECT:
                            var id = ServerManager.RegisterPlayer(m_connections[i].InternalId);
                            Debug.Log("Connected " + id);
                            byte[] idData = NetworkLayer.newCommand(CMD.CONNECT, BitConverter.GetBytes(id));
                            var newID = new DataStreamWriter(idData.Length, Allocator.Temp);
                            newID.Write(idData);
                            m_ServerDriver.Send(NetworkPipeline.Null, m_connections[i], newID);
                            break;

                        case CMD.PLAYER_UPDATE:
                            byte[] playerByte = strm.ReadBytesAsArray(ref readerCtx, 32);
                            var player = ByteConverter.toPlayer(playerByte);
                            ServerManager.OnUpdatePlayer(player);
                            break;
                        case CMD.SHOOT:
                            var shootBye = NetworkLayer.newCommand(CMD.SHOOT, BitConverter.GetBytes(m_connections[i].InternalId));
                            var shootData = new DataStreamWriter(shootBye.Length, Allocator.Temp);
                            shootData.Write(shootBye);
                            for (int y = 0; y < m_connections.Length; ++y)
                            {
                                if (m_connections[i] != m_connections[y])
                                    m_ServerDriver.Send(NetworkPipeline.Null, m_connections[y], shootData);
                            };
                            break;
                        case CMD.PLAYER_DIE:
                            Debug.LogWarning(" S DIE");
                            var dieByte = NetworkLayer.newCommand(CMD.PLAYER_DIE, BitConverter.GetBytes(m_connections[i].InternalId));
                            var dieData = new DataStreamWriter(dieByte.Length, Allocator.Temp);
                            dieData.Write(dieByte);
                            for (int y = 0; y < m_connections.Length; ++y)
                            {
                                m_ServerDriver.Send(NetworkPipeline.Null, m_connections[y], dieData);
                            };
                            break;
                    }
                    for (int clientIndex = 0; clientIndex < clientList.Count; clientIndex++)
                    {
                        byte[] playerData = NetworkLayer.newCommand(CMD.PLAYER_UPDATE, ByteConverter.playerToByte(clientList[clientIndex]));
                        var player = new DataStreamWriter(playerData.Length, Allocator.Temp);
                        player.Write(playerData);
                        m_ServerDriver.Send(NetworkPipeline.Null, m_connections[i], player);
                    };

                    // Send the pong message with the same id as the ping
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    // This connection no longer exist, remove it from the list
                    // The next iteration will operate on the new connection we swapped in so as long as it exist the
                    // loop can continue
                    Debug.Log("Desconnected " + m_connections[i].InternalId);
                    byte[] disconnectData = NetworkLayer.newCommand(CMD.PLAYER_DISCONNECTED, BitConverter.GetBytes(m_connections[i].InternalId));
                    var player = new DataStreamWriter(disconnectData.Length, Allocator.Temp);
                    player.Write(disconnectData);
                    ServerManager.OnDisconnectPlayer(m_connections[i].InternalId);
                    m_connections.RemoveAtSwapBack(i);
                    for (int y = 0; y < m_connections.Length; ++y)
                    {
                        m_ServerDriver.Send(NetworkPipeline.Null, m_connections[y], player);
                    };
                    if (i >= m_connections.Length)
                        break;
                }
            }
        }
    }
}
