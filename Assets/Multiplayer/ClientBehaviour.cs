using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Multiplayer;

public class ClientBehaviour : MonoBehaviour
{
    struct PendingPing
    {
        public int id;
        public float time;
    }

    private UdpNetworkDriver m_ClientDriver;
    private NetworkConnection m_clientToServerConnection;
    // pendingPing is a ping sent to the server which have not yet received a response.
    private PendingPing m_pendingPing;
    // The ping stats are two integers, time for last ping and number of pings
    private int m_lastPingTime;
    private int m_numPingsSent;

    void Start()
    {
        // Create a NetworkDriver for the client. We could bind to a specific address but in this case we rely on the
        // implicit bind since we do not need to bing to anything special
        m_ClientDriver = new UdpNetworkDriver(new INetworkParameter[0]);
    }

    void OnDestroy()
    {
        m_ClientDriver.Dispose();
    }

    void FixedUpdate()
    {
        // Update the ping client UI with the ping statistics computed by teh job scheduled previous frame since that
        // is now guaranteed to have completed
        PingClientUIBehaviour.UpdateStats(m_numPingsSent, m_lastPingTime);

        // Update the NetworkDriver. It schedules a job so we must wait for that job with Complete
        m_ClientDriver.ScheduleUpdate().Complete();

        // If the client ui indicates we should be sending pings but we do not have an active connection we create one
        if (PingClientUIBehaviour.ServerEndPoint.IsValid && !m_clientToServerConnection.IsCreated)
            m_clientToServerConnection = m_ClientDriver.Connect(PingClientUIBehaviour.ServerEndPoint);
        // If the client ui indicates we should not be sending pings but we do have a connection we close that connection
        if (!PingClientUIBehaviour.ServerEndPoint.IsValid && m_clientToServerConnection.IsCreated)
        {
            ClientManager.UnregisterPlayer();
            m_clientToServerConnection.Disconnect(m_ClientDriver);
            m_clientToServerConnection = default(NetworkConnection);
        }
        if (ClientManager.instance.started && m_clientToServerConnection.IsCreated)
        {
            Player player = ClientManager.instance.myPlayer;
            byte[] playerByte = NetworkLayer.newCommand(CMD.PLAYER_UPDATE, ByteConverter.playerToByte(player));
            var playerData = new DataStreamWriter(playerByte.Length, Allocator.Temp);
            playerData.Write(playerByte);
            m_clientToServerConnection.Send(m_ClientDriver, playerData);
        }

        DataStreamReader strm;
        NetworkEvent.Type cmd;
        // Process all events on the connection. If the connection is invalid it will return Empty immediately
        while ((cmd = m_clientToServerConnection.PopEvent(m_ClientDriver, out strm)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                var commandConnect = new DataStreamWriter(1, Allocator.Temp);
                commandConnect.Write((byte)CMD.CONNECT);
                m_clientToServerConnection.Send(m_ClientDriver, commandConnect);

            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtx = default(DataStreamReader.Context);
                byte data = strm.ReadByte(ref readerCtx);
                CMD command = (CMD)data;

                switch (command)
                {
                    case CMD.CONNECT:
                        ClientManager.instance.RegisterPlayer(strm.ReadInt(ref readerCtx));
                        break;
                    case CMD.PLAYER_UPDATE:
                        byte[] playerByte = strm.ReadBytesAsArray(ref readerCtx, 32);
                        var player = ByteConverter.toPlayer(playerByte);
                        Debug.Log("Client R Type :" + command + " id: " + player.ID + " " + player.position + " " + player.rotation);
                        ClientManager.OnUpdatePlayer(player);
                        break;
                    case CMD.PLAYER_DISCONNECTED:
                        var playerID = strm.ReadInt(ref readerCtx);
                        ClientManager.OnDisconnectPlayer(playerID);
                        Debug.Log("Client R Disconneted player: " + playerID);
                        break;
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                // If the server disconnected us we clear out connection
                ClientManager.UnregisterPlayer();
                m_clientToServerConnection = default(NetworkConnection);
            }
        }
    }
}
