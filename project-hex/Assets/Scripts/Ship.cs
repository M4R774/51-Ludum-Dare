using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Class for ships, player and enemy alike
//
public class Ship : MonoBehaviour
{

    [SerializeField] protected Animator animator;

    [Header ("Audio")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip sinkingSound;
    public void SinkShip()
    {
        animator.SetTrigger("sink");
        audioSource.PlayOneShot(sinkingSound);
        Destroy(gameObject, 5f);
    }
}
