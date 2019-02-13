using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConsumptionText : MonoBehaviour {
    public CanvasReference canvasRef;
    TMP_Text conText;

    int pop, con;

    public void Awake() {
        conText = GetComponent<TMP_Text>();

        conText.text = "-0";
    }

    public void Update() {
        if (pop == canvasRef.ship.maxPopulationPoints && con == canvasRef.ship.foodConsumption)
            return;

        conText.text = "-" + (canvasRef.ship.maxPopulationPoints * canvasRef.ship.foodConsumption);
        pop = canvasRef.ship.maxPopulationPoints;
        con = canvasRef.ship.foodConsumption;
    }
}
