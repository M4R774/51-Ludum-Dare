using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ReceiveShadows : MonoBehaviour
{

    [ExecuteInEditMode]
    void OnValidate()
    {
        this.FixShadows();
    }

    public void FixShadows()
    {
        this.GetComponent<TilemapRenderer>().receiveShadows = true;
        this.GetComponent<TilemapRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
}
