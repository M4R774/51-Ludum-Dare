using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Hold info on count of oranges it has to give for the player
// If there's oranges to give, gives it when player collides with trigger volume
//

public class DesertIsland : MonoBehaviour
{
    ActionBarManager actionBarManager;
    GameObject islandModel;

    private AudioSource audioSource;
    private bool isVisited;
    [SerializeField] List<GameObject> objectsToDisable = new List<GameObject>();
    [SerializeField] Collider triggerVolume;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        isVisited = false;
        islandModel = this.gameObject.transform.GetChild(0).gameObject;
        islandModel.transform.rotation = Random.rotation;
        islandModel.transform.localEulerAngles = new Vector3(0, islandModel.transform.localEulerAngles.y, 0);

        actionBarManager = GameObject.Find("ActionBar").GetComponent<ActionBarManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isVisited)
        {
            // let's check that the player needs oranges
            if(other.gameObject.GetComponent<CubeController>().movementSpeed < 10)
            {
                isVisited = true;
                audioSource.Play();
                actionBarManager.IncreaseMaxMovementPoints();
                actionBarManager.IncreaseMaxMovementPoints();
                foreach (var item in objectsToDisable)
                {
                    item.SetActive(false);
                }
            }
        }
    }

    // Hack to make the player appear entered into a trigger volume even if they already were there
    // to circumvent problem with the player staying in the collider, losing action points but not gaining any
    void FlickCollider()
    {
        triggerVolume.enabled = false;
        triggerVolume.enabled = true;
    }

    private void OnEnable()
    {
        EventManager.OnTenSecondTimerEnded += FlickCollider;
    }

    private void OnDisable()
    {
        EventManager.OnTenSecondTimerEnded -= FlickCollider;
    }
}
