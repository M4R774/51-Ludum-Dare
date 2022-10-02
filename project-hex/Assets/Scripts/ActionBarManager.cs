using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarManager : MonoBehaviour
{
    public static ActionBarManager instance;
    public List<Image> actionPoints;

    public int savedState;
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
        for (int i = 0; i < children; ++i)
        {
            actionPoints.Add(gameObject.transform.GetChild(i).GetComponent<Image>());
        }

        for (int i = 0; i < actionPoints.Count; i++)
        {
            actionPoints[i].color = Color.gray;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetVisible(int movementPointsLeft)
    {
        for (int i = 0; i < actionPoints.Count; i++)
        {
            if(i < movementPointsLeft) {
                actionPoints[i].color = Color.white;
            } else {
                actionPoints[i].color = new Color (0, 0, 0, 0.1f);
            }
        }
        savedState = movementPointsLeft;
    }

    public void SetPlan(int plannedMoveCount) {
        // Plan next move BUT
        // first return from previous plan:
        SetVisible(savedState);

        var greens = new List<int>();
        for (int i = 0; i < actionPoints.Count; i++) {
            if(
                actionPoints[i].color == Color.white
            ) {
                greens.Add(i);
            }
        }

        for (int i = 0; i < plannedMoveCount; i++)
        {
            int lastGreen = greens.Count - i - 1;
            if (lastGreen < greens.Count)
            {
                actionPoints[greens[lastGreen]].color = Color.gray;
            }
        }
    }
}
