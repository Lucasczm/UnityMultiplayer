using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBehaviour : MonoBehaviour
{    
    void OnTriggerEnter(Collider other)
    {
        var playerB = other.GetComponent<PlayerBehaviour>();       
        if(playerB == null) return;
        playerB.Die();
    }
}
