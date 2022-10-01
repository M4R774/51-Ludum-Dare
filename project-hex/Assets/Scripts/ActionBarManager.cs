using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarManager : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(255, 0, 0 ); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
