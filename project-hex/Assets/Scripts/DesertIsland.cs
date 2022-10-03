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

    bool isVisited;
    [SerializeField] List<GameObject> objectsToDisable = new List<GameObject>();

    void Start()
    {
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
            isVisited = true;
            actionBarManager.IncreaseMaxMovementPoints();
            actionBarManager.IncreaseMaxMovementPoints();
            foreach (var item in objectsToDisable)
            {
                item.SetActive(false);
            }
        }
    }
}
