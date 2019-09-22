using Multiplayer;
using UnityEngine;
namespace Game.Client
{
    public class PlayerClient : MonoBehaviour
    {
        public Player player;
        Rigidbody rigidbody;
        Transform hand, anchor;
        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            hand = transform.GetChild(0).GetComponent<Transform>();
            anchor = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Transform>();
        }
        public void setPosition(Vector3 position)
        {
            rigidbody.MovePosition(position);
        }
        public void setRotation(Quaternion rotation)
        {
            transform.rotation = new Quaternion(0, rotation.y, 0, rotation.w).normalized;
        }
        public void updatePlayer(Player player)
        {
            rigidbody.MovePosition(player.position);
            hand.rotation = player.rotation;
        }
        public void Shoot()
        {
            Instantiate(ClientManager.instance.bulletObject, anchor.position, anchor.rotation);
        }
    }
}