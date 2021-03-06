using UnityEngine;
using Game.Client;
using Multiplayer;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour
{
    Rigidbody rigidbody;
    Transform hand, anchor;
    public float playerSpeed = 2, jumpForce = 6, groundChecker = 1.05f;
    float bulletsCount = 150, reloadingTime;
    bool reloading;
    void Start()
    {
        transform.tag = "MainPlayer";
        rigidbody = GetComponent<Rigidbody>();
        hand = transform.GetChild(0).GetComponent<Transform>();
        anchor = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Transform>();
        //        Camera.main.gameObject.GetComponent<CameraBehaviour>().enabled = true;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
    }
    void FixedUpdate()
    {
        Rotation();
        Move();
        Shoot();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            rigidbody.isKinematic = true;
            transform.position = ClientManager.GetSpawn();
            rigidbody.isKinematic = false;
        }
    }
    void Shoot()
    {
        if (Input.GetMouseButton(0) && bulletsCount > 0 && !reloading)
        {
            ClientBehaviour.SendShoot();
            Instantiate(ClientManager.instance.bulletObject, anchor.position, anchor.rotation);
            bulletsCount--;
            UIManager.UpdateBar(bulletsCount, 150f);
        }
        if (bulletsCount <= 0 && !reloading)
        {
            reloading = true;
            StartCoroutine(ReloadAmmo());

        }
    }
    void Move()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var direction = Vector3.zero;

        if (horizontal > 0f)
        {
            direction = playerSpeed * horizontal * transform.forward;
        }
        else if (horizontal < 0f)
        {
            direction = playerSpeed * horizontal * transform.forward;
        }
        if (isGround())
        {
            if (vertical > 0f)
            {
                rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
        }
        var movePostion = rigidbody.position + direction * 3 * Time.deltaTime;
        rigidbody.MovePosition(movePostion);
        ClientManager.instance.myPlayer.position = rigidbody.position;

    }
    public float distance = 5.23f;
    void Rotation()
    {
        var mouse_pos = Input.mousePosition;
        mouse_pos.z = distance; //The distance between the camera and object
        var object_pos = Camera.main.WorldToScreenPoint(hand.transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = object_pos.y - mouse_pos.y;
        var angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
        hand.transform.rotation = Quaternion.Euler(new Vector3(angle, 0, 0));
        ClientManager.instance.myPlayer.rotation = hand.transform.rotation;
    }
    bool isGround()
    {
        return Physics.Raycast(transform.position, -Vector3.up, groundChecker);
    }
    void OnDestroy()
    {
        // Camera.main.gameObject.GetComponent<CameraBehaviour>().enabled = false;
    }
    public void Die()
    {
        ClientBehaviour.SendDie();
        RestorePosition();
    }
    public void RestorePosition()
    {
        rigidbody.isKinematic = true;
        transform.position = ClientManager.GetSpawn();
        rigidbody.isKinematic = false;
    }
    IEnumerator ReloadAmmo()
    {
        reloadingTime = 0f;
        while (reloadingTime < 3f)
        {
            reloadingTime += Time.deltaTime;
            UIManager.UpdateBar(reloadingTime, 3f);
            yield return null;
        }
        bulletsCount = 150;
        reloading = false;
    }
}