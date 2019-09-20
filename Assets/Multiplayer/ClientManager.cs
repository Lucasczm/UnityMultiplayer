using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Client;
namespace Multiplayer
{
    public class ClientManager : MonoBehaviour
    {
        #region Singleton 
        public static ClientManager instance;
        void Awake()
        {
            if (instance != null) return;
            instance = this;
        }
        #endregion

        [SerializeField] GameObject playerObject;
        public GameObject bulletObject;
        int clientID;
        List<GameObject> players = new List<GameObject>();
        public Player myPlayer = new Player();
        public bool started;
        private GameObject gm;
        void Start()
        {
            // RegisterPlayer();
        }
        public void RegisterPlayer(int connectionID)
        {
            myPlayer.ID = connectionID;
            GameObject player = Instantiate(playerObject);
            player.AddComponent(typeof(PlayerBehaviour));
            gm = player;
            started = true;
            Debug.Log("New connection ID: " + connectionID);

        }
        public static void UnregisterPlayer()
        {
            if (instance == null) return;
            instance.started = false;
            Destroy(instance.gm);
        }
        public static void OnConnectPlayer(Player player)
        {
            if (instance == null) return;
            GameObject gm = Instantiate(instance.playerObject);
            PlayerClient gmPlayer = (PlayerClient)gm.AddComponent(typeof(PlayerClient));
            gmPlayer.player = player;
            instance.players.Add(gm);
        }
        public static void OnDisconnectPlayer(int playerID)
        {
            if (instance == null) return;
            for (int i = 0; i < instance.players.Count; i++)
            {
                if (instance.players[i].GetComponent<PlayerClient>().player.ID == playerID)
                {
                    var player = instance.players[i];
                    instance.players.RemoveAt(i);
                    Destroy(player);
                    return;
                }
            }
        }
        public static void OnUpdatePlayer(Player player)
        {
            if (instance == null || instance.myPlayer.ID == player.ID) return;
            for (int i = 0; i < instance.players.Count; i++)
            {
                if (instance.players[i].GetComponent<PlayerClient>().player.ID == player.ID)
                {
                    instance.players[i].GetComponent<PlayerClient>().updatePlayer(player);
                    return;
                }
            }
            OnConnectPlayer(player);
        }
        public List<Player> GetPlayers()
        {
            List<Player> players = new List<Player>();
            this.players.ForEach((gm) => players.Add(gm.GetComponent<PlayerClient>().player));
            return players;
        }
    }
}
