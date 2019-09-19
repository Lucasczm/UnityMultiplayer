using UnityEngine;
using Game.Client;
using Multiplayer;

public class PlayerBehaviour : MonoBehaviour
{
    new Rigidbody rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Camera.main.gameObject.GetComponent<CameraBehaviour>().enabled = true;
    }
    void FixedUpdate()
    {
        Rotation();
        Move();
    }
    void Move()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var direction = Vector3.zero;

        if (horizontal > 0f)
        {
            direction = horizontal * transform.right;
        }
        else if (horizontal < 0f)
        {
            direction = horizontal * transform.right;
        }
        if (vertical > 0f)
        {
            direction = vertical * transform.forward;
        }
        else if (vertical < 0f)
        {
            direction = vertical * transform.forward;
        }
        var movePostion = rigidbody.position + direction * 3 * Time.deltaTime;
        rigidbody.MovePosition(movePostion);
        ClientManager.instance.myPlayer.position = movePostion;

    }
    void Rotation()
    {
        var mousePos = Input.mousePosition;
        var cameraRay = Camera.main.ScreenPointToRay(mousePos);
        var layerMask = LayerMask.GetMask("Floor");
        if (Physics.Raycast(cameraRay, out var hit, 100, layerMask))
        {
            var forward = hit.point - transform.position;
            var rotation = Quaternion.LookRotation(forward);
            transform.rotation = new Quaternion(0, rotation.y, 0, rotation.w).normalized;
            ClientManager.instance.myPlayer.rotation = transform.rotation;
        }
    }
    void OnDestroy()
    {
        Camera.main.gameObject.GetComponent<CameraBehaviour>().enabled = false;
    }
}