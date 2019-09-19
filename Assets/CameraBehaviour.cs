using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    GameObject player;
    [SerializeField] Vector3 offset;
    void OnEnable()
    {
        player = FindObjectOfType<PlayerBehaviour>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        transform.position = pos + offset;
    }
}
