using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FastTurns : MonoBehaviour
{
    public InputField nbTurnsInput;
    public float speedMultiplicator = 2;

    [HideInInspector]
    public bool isFastTurn;

    private int remainingTurns;

    public static FastTurns Instance;

    private void Awake() {
        if (!Instance) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public void StartFastTurns() {
        remainingTurns = Int32.Parse(nbTurnsInput.text);
        TurnManager.Instance.OnEndTurn += UpdateFastTurn;
        isFastTurn = true;
        UpdateFastTurn();
    }
   
    
    void UpdateFastTurn()
    {
        if (remainingTurns > 0) {
            remainingTurns--;
            TurnManager.Instance.EndTurn();
        }
        else {
            isFastTurn = false;
            TurnManager.Instance.OnEndTurn -= UpdateFastTurn;
        }
    }
}
