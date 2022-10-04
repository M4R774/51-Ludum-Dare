using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class MapIslandController : MonoBehaviour
{
    public GuideArrow guideArrow;
    public GameObject treasureIsland;
    public GameObject treasuremapMesh;

    private AudioSource audioSource;
    [SerializeField] Collider triggerVolume;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GameObject guideArrowObject = GameObject.Find("GuidingArrow");
        guideArrow = guideArrowObject.GetComponent<GuideArrow>();
        treasureIsland = GameObject.Find("Goal");
        RandomizePosition();
    }

    private void RandomizePosition()
    {
        for (int i = 0; i < 1000; i++)
        {
            WorldTile randomTile = GameTiles.instance.tiles.ElementAt(Random.Range(0, GameTiles.instance.tiles.Count)).Value;
            if (randomTile.IsWalkable() && !randomTile.IsVisible)
            {
                transform.position = randomTile.WorldPosition;
                return;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            audioSource.Play();
            treasuremapMesh.SetActive(false);
            treasureIsland.transform.GetChild(0).gameObject.SetActive(true);
            treasureIsland.GetComponent<Collider>().enabled = true;
            guideArrow.target = treasureIsland;
            triggerVolume.enabled = false;
        }
    }
}
