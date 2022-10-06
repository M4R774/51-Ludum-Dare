using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Class for player ship
//

public class PlayerShip : Ship
{
    [SerializeField] CubeController controller;
    [Header ("Debug")]
    [SerializeField] bool godMode = false;
    [SerializeField] bool unlimitedMoves = false;
    new public void SinkShip()
    {
        if(!godMode)
        {
            animator.SetTrigger("sink");
            audioSource.PlayOneShot(sinkingSound);
            controller.enabled = false;
            Destroy(gameObject, 5f);
        }
    }

    void Update()
    {
        if(unlimitedMoves)
        {
            controller.movementSpeed = 10;
            controller.movementPointsLeft = 10;
        }
    }
}