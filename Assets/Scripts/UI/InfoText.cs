using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoText : MonoBehaviour {
    public CanvasReference canvasRef;
    public TMP_Text popText;
    public TMP_Text consoText;

    int pop, con;

    public void Awake() {
        popText.text = "0";
        consoText.text = "-0";
    }

    public void Update() {
        if (pop == canvasRef.ship.maxPopulationPoints && con == canvasRef.ship.foodConsumption)
            return;

        popText.text = "" + canvasRef.ship.maxPopulationPoints;
        consoText.text = "-" + (canvasRef.ship.maxPopulationPoints * canvasRef.ship.foodConsumption);
        pop = canvasRef.ship.maxPopulationPoints;
        con = canvasRef.ship.foodConsumption;
    }
}
