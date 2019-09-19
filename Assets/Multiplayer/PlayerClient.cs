using UnityEngine;
namespace Game.Client
{
    public class PlayerClient : MonoBehaviour
    {
        public Player player;
        new Rigidbody rigidbody;
        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }
        public void setPosition(Vector3 position)
        {
            rigidbody.MovePosition(position);
        }
        public void setRotation(Quaternion rotation)
        {
            transform.rotation = new Quaternion(0, rotation.y, 0, rotation.w).normalized;
        }
        public void updatePlayer(Player player){
            rigidbody.MovePosition(player.position);
            transform.rotation = player.rotation;
        }
    }
}