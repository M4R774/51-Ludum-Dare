using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Class for player ship
//
public class PlayerShip : Ship
{
    [Header ("Debug")]
    [SerializeField] bool godMode = false;
    new public void SinkShip()
    {
        if(!godMode)
        {
            animator.SetTrigger("sink");
            audioSource.PlayOneShot(sinkingSound);
            Destroy(gameObject, 5f);
        }
    }
}