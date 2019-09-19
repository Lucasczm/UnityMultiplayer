using System;
using Unity.Mathematics;
using UnityEngine;

namespace Multiplayer
{
    public static class ByteConverter
    {
        public static byte[] playerToByte(Player player)
        {
            byte[] pos = vector3ToByte(player.position);
            byte[] rotation = quaternionToByte(player.rotation);
            byte[] bytes = new byte[sizeof(int) + pos.Length + rotation.Length];
            Buffer.BlockCopy(BitConverter.GetBytes(player.ID), 0, bytes, 0, sizeof(int));
            Buffer.BlockCopy(pos, 0, bytes, sizeof(int), pos.Length);
            Buffer.BlockCopy(rotation, 0, bytes, sizeof(int) + pos.Length, rotation.Length);
            return bytes;
        }
        public static Player toPlayer(byte[] buff)
        {
            Player player = new Player();
            player.ID = BitConverter.ToInt32(buff, 0);
            player.position = toVector3(buff, sizeof(int));
            player.rotation = toQuaternion(buff, sizeof(int) + sizeof(float) * 3);
            return player;
        }
        public static float3 toFloat3(byte[] buff)
        {
            float3 vect = float3.zero;
            vect.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            vect.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            vect.z = BitConverter.ToSingle(buff, 2 * sizeof(float));
            return vect;
        }
        public static byte[] float3ToByte(float3 vect)
        {
            byte[] bytes = new byte[sizeof(float) * 3];
            Buffer.BlockCopy(BitConverter.GetBytes(vect.x), 0, bytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vect.y), 0, bytes, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vect.z), 0, bytes, 8, 4);
            return bytes;
        }
        public static float3 toVector3(byte[] buff, int offset = 0)
        {
            float3 vect = float3.zero;
            vect.x = BitConverter.ToSingle(buff, offset + 0 * sizeof(float));
            vect.y = BitConverter.ToSingle(buff, offset + 1 * sizeof(float));
            vect.z = BitConverter.ToSingle(buff, offset + 2 * sizeof(float));
            return vect;
        }
        public static byte[] vector3ToByte(Vector3 vect)
        {
            byte[] bytes = new byte[sizeof(float) * 3];
            Buffer.BlockCopy(BitConverter.GetBytes(vect.x), 0, bytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vect.y), 0, bytes, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(vect.z), 0, bytes, 8, 4);
            return bytes;
        }
        public static Quaternion toQuaternion(byte[] buff, int offset = 0)
        {
            Quaternion rotation = Quaternion.identity;
            rotation.x = BitConverter.ToSingle(buff, offset + 0 * sizeof(float));
            rotation.y = BitConverter.ToSingle(buff, offset + 1 * sizeof(float));
            rotation.z = BitConverter.ToSingle(buff, offset + 2 * sizeof(float));
            rotation.w = BitConverter.ToSingle(buff, offset + 3 * sizeof(float));
            return rotation;
        }
        public static byte[] quaternionToByte(Quaternion quaternion)
        {
            byte[] bytes = new byte[sizeof(float) * 4];
            Buffer.BlockCopy(BitConverter.GetBytes(quaternion.x), 0, bytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(quaternion.y), 0, bytes, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(quaternion.z), 0, bytes, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(quaternion.w), 0, bytes, 12, 4);
            return bytes;
        }
    }
}