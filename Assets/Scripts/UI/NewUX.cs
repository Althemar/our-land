using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewUX : MonoBehaviour {
    public CanvasReference refCanvas;

    public Button takeOffButton;

    public GameObject hourglass;

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

            if (Quaternion.Dot(hourglass.transform.localRotation, Quaternion.Euler(0, 0, 180f)) < 0.999f)
                hourglass.transform.localRotation *= Quaternion.Euler(0, 0, Time.deltaTime * 110f);
            else
                hourglass.transform.localRotation = Quaternion.Euler(0, 0, 180f);
        } else {
            if(!isTakeOff)
                takeOffText.text = I18N.GetText("switchToMovement");
            else
                takeOffText.text = I18N.GetText("switchToHarvest");

            takeOffButton.interactable = true;


            if (Quaternion.Dot(hourglass.transform.localRotation, Quaternion.Euler(0, 0, 0)) < 0.999f)
                hourglass.transform.localRotation *= Quaternion.Euler(0, 0, Time.deltaTime * 110f);
            else
                hourglass.transform.localRotation = Quaternion.Euler(0, 0, 0f);
            //hourglass.transform.localRotation = Quaternion.Slerp(hourglass.transform.localRotation, Quaternion.Euler(0, 0, 0f), Time.deltaTime);
        }
    }

    public void TakeOff() {
        if (TurnManager.Instance.State != TurnManager.TurnState.Player)
            return;

        isTakeOff ^= true;

        if (isTakeOff) {
            AkSoundEngine.PostEvent("Play_SFX_Button_TakeOff", this.gameObject);
            refCanvas.mouse.moveMode = true;
            refCanvas.ship.MovementMode();
            refCanvas.mouse.harvestMode = false;
            
            refCanvas.ship.ClearActivePopulationPoints();
            refCanvas.ship.ClearHarvestOutline();
            refCanvas.ship.RedoMove();
        } else {
            AkSoundEngine.PostEvent("Play_SFX_Button_Landing", this.gameObject);
            refCanvas.mouse.moveMode = false;
            refCanvas.ship.HarvestMode();
            refCanvas.mouse.harvestMode = true;

            refCanvas.ship.ShowActivePopulationPoints();
            refCanvas.ship.ShowHarvestOutline();
            refCanvas.ship.CancelMove();
        }
    }

    public void EndTurn() {
        TurnManager.Instance.EndTurn();
        AkSoundEngine.PostEvent("Play_End_Turn_Pl", this.gameObject);

        refCanvas.mouse.moveMode = false;
        refCanvas.mouse.harvestMode = false;
        refCanvas.ship.ClearActivePopulationPoints();
        refCanvas.ship.ClearHarvestOutline();
    }
}
