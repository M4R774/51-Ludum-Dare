using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Manages spawning a random 3D object onto hex
//

public class HexObjectManager : MonoBehaviour
{
    [SerializeField] List<GameObject> objects = new List<GameObject>();
    void Start()
    {
        int index = Random.Range(0, objects.Count);
        int rotation = Random.Range(0, 360);
        GameObject obj = Instantiate(objects[index], transform.position, Quaternion.Euler(0, rotation, 0), this.transform);
    }

    void Update()
    {
        
    }
}
