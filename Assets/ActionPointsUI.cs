﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionPointsUI : MonoBehaviour
{
    public MotherShip motherShip;

    TMP_Text text;
    
    void Start()
    {
        text = GetComponent<TMP_Text>();
        motherShip.OnTurnBegin += UpdateActionPointsCount;
        motherShip.OnEndMoving += UpdateActionPointsCount;
        motherShip.OnRemainingPointsChanged += UpdateActionPointsCount;
    }

    public void UpdateActionPointsCount() {
        text.text = motherShip.RemainingActionPoints + " / " + motherShip.ActionPoints;
    }
}
