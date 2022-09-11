using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSnap : MonoBehaviour
{
    public int x;
    public int z;
    public int y;

    private GridLayout gridLayout;

    // Start is called before the first frame update
    void Start()
    {
        // Snap the GameObject to parent GridLayout
        gridLayout = transform.parent.GetComponentInParent<GridLayout>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = gridLayout.CellToWorld(new Vector3Int(x, y, z));
    }
}
