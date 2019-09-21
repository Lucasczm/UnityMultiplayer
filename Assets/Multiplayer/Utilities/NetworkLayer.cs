using System;
using UnityEngine;

namespace Multiplayer
{
    public static class NetworkLayer
    {
        public static byte[] newCommand(CMD command, byte[] data)
        {
            byte[] cmd = new byte[sizeof(byte) + data.Length];
            Buffer.BlockCopy(BitConverter.GetBytes((byte)command), 0, cmd, 0, sizeof(byte));
            Buffer.BlockCopy(data, 0, cmd, sizeof(byte), data.Length);
            return cmd;
        }
        public static CMD ReadCommand(byte[] dataSrc, out byte[] outData)
        {
            outData = new byte[dataSrc.Length - sizeof(byte)];
            Buffer.BlockCopy(dataSrc, sizeof(byte), outData, 0, outData.Length);
            //int i = BitConverter.(dataSrc, sizeof(int));
            Debug.Log("III"+ dataSrc[0]);
            return (CMD)dataSrc[0];//   
        }
    }

    public enum CMD: byte
    {
        CONNECT, DISCONNECT, PLAYER_UPDATE, NEW_PLAYER_ENTER, PLAYER_DISCONNECTED, SHOOT, PLAYER_DIE
    }
}