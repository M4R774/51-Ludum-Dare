using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Class for ships, player and enemy alike
//
public class EnemyShip : Ship
{
  [SerializeField] EnemyAI aiComponent;

  new public void SinkShip()
  {
    animator.SetTrigger("sink");
    audioSource.PlayOneShot(sinkingSound);
    aiComponent.isAlive = false;
    Destroy(gameObject, 5f);
  }

}