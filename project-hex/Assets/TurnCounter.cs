using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnCounter : MonoBehaviour
{
    private int turnNumber;
    public TextMeshProUGUI textComponent;

    // Start is called before the first frame update
    void Start()
    {
        textComponent = transform.gameObject.GetComponent<TextMeshProUGUI>();
        UpdateTurnCounterText();
    }

    private void OnEnable()
    {
        EventManager.OnEndTurn += UpdateTurnCounterText;
    }

    private void OnDisable()
    {
        EventManager.OnEndTurn -= UpdateTurnCounterText;
    }

    private void UpdateTurnCounterText()
    {
        turnNumber += 1;
        textComponent.text = "Turn: " + turnNumber;
    }
}
