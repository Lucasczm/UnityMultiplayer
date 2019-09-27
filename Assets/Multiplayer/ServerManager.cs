using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Multiplayer
{
    public class ServerManager : MonoBehaviour
    {
        #region Singleton 
        public static ServerManager instance;

        List<Player> clients = new List<Player>();
        void Awake()
        {
            if (instance != null) return;
            instance = this;
        }
        #endregion
        public static List<Player> GetClients()
        {
            if (instance == null) return null;
            return instance.clients;
        }
        public static int RegisterPlayer(int connectionID)
        {
            if (instance == null) return 0;
            // int newID = 0;
            // if (instance.clients.Count > 0)
            // {
            //     newID = instance.clients[instance.clients.Count - 1].ID + 1;
            // }
            instance.clients.Add(new Player { ID = connectionID });
            return connectionID;
        }
        public static void OnDisconnectPlayer(int playerID)
        {
            if (instance == null) return;
            for (int i = 0; i < instance.clients.Count; i++)
            {
                if (instance.clients[i].ID == playerID)
                {
                    instance.clients.RemoveAt(i);
                }
            }
        }
        public static void OnUpdatePlayer(Player player)
        {
            if(instance == null) return;
            for (int i = 0; i < instance.clients.Count; i++)
            {
                if (instance.clients[i].ID == player.ID)
                {
                    instance.clients[i] = player;
                }
            }
        }
    }
}