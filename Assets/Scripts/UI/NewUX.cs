using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewUX : MonoBehaviour {
    public CanvasReference refCanvas;

    public Button takeOffButton;

    TextMeshProUGUI takeOffText;

    bool isTakeOff;

    void Awake() {
        takeOffText = takeOffButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start() {
        takeOffText.text = I18N.GetText("takeOf");

        refCanvas.mouse.moveMode = false;
        refCanvas.mouse.harvestMode = true;

        refCanvas.ship.OnTurnBegin += ReInit;
    }

    private void ReInit() {
        isTakeOff = false;

        refCanvas.mouse.moveMode = false;
        refCanvas.mouse.harvestMode = true;
        refCanvas.ship.CancelMove();
        refCanvas.ship.ShowActivePopulationPoints();
        refCanvas.ship.ShowHarvestOutline();
    }

    private void Update() {
        if (TurnManager.Instance.State != TurnManager.TurnState.Player || GameManager.Input.IsBlock) {
            takeOffText.text = I18N.GetText("pleaseWait");

            takeOffButton.interactable = false;
        } else {
            if(!isTakeOff)
                takeOffText.text = I18N.GetText("switchToMovement");
            else
                takeOffText.text = I18N.GetText("switchToHarvest");

            takeOffButton.interactable = true;
        }
    }

    public void TakeOff() {
        if (TurnManager.Instance.State != TurnManager.TurnState.Player)
            return;

        isTakeOff ^= true;

        if (isTakeOff) {
            refCanvas.mouse.moveMode = true;
            refCanvas.mouse.harvestMode = false;
            
            refCanvas.ship.ClearActivePopulationPoints();
            refCanvas.ship.ClearHarvestOutline();
            refCanvas.ship.RedoMove();
        } else {
            refCanvas.mouse.moveMode = false;
            refCanvas.mouse.harvestMode = true;

            refCanvas.ship.ShowActivePopulationPoints();
            refCanvas.ship.ShowHarvestOutline();
            refCanvas.ship.CancelMove();
        }
    }

    public void EndTurn() {
        TurnManager.Instance.EndTurn();

        refCanvas.mouse.moveMode = false;
        refCanvas.mouse.harvestMode = false;
        refCanvas.ship.ClearActivePopulationPoints();
        refCanvas.ship.ClearHarvestOutline();
    }
}
