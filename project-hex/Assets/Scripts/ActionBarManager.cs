using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarManager : MonoBehaviour
{
    public static ActionBarManager instance;
    public List<Image> actionPoints;

    public void Awake()
    {
        CheckThatIamOnlyInstance();
    }

    private void CheckThatIamOnlyInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        int children = gameObject.transform.childCount;
        Debug.Log(children);
        //if (children > 0) {
        //    for (int i = 0; i < children; ++i)
        //    {
        //        actionPoints.Add(gameObject.transform.GetChild(i).GetComponent<Image>());
        //    }
//
        //    for (int i = 0; i < actionPoints.Count - 1; i++)
        //    {
        //        actionPoints[i].color = Color.gray;
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetVisible(int movementPointsLeft)
    {
        //if (movementPointsLeft > 0 && actionPoints.Count > 0) {
        //    for (int i = 0; i < movementPointsLeft; i++)
        //    {
        //        actionPoints[i].color = Color.green;
        //    }
        //}
    }

}
