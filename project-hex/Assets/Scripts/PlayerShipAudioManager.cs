using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerShipAudioManager : MonoBehaviour
{
    public List<AudioClip> movementSounds;

    private AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMovementSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = movementSounds[Random.Range(0, movementSounds.Count)];
            audioSource.Play();
        }
    }
}
