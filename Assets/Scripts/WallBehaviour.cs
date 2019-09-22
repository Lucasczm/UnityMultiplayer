using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehaviour : MonoBehaviour
{    
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("MainPlayer"))
            other.GetComponent<PlayerBehaviour>().Die();       
    }
}
