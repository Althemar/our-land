using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionPointsUI : MonoBehaviour {
    public CanvasReference refCanvas;

    TMP_Text text;

    void Start() {
        text = GetComponent<TMP_Text>();
        refCanvas.ship.OnRemainingPointsChanged += UpdateActionPointsCount;
    }

    public void UpdateActionPointsCount() {
        text.text = refCanvas.ship.remainingPopulationPoints.ToString() + " / " + refCanvas.ship.maxPopulationPoints.ToString();
    }
}
