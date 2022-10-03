using UnityEngine;
using System.Collections;

public class DestroyEffect : MonoBehaviour 
{
    private void Start()
    {
        Destroy(gameObject, 5f);
    }
}
