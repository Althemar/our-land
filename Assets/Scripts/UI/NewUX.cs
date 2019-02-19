using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewUX : MonoBehaviour {
    public CanvasReference refCanvas;

    public GameObject hourglass;
    Animator panelAnim;

    bool isTakeOff;


    void Start() {
        panelAnim = GetComponent<Animator>();

        refCanvas.mouse.moveMode = false;
        refCanvas.mouse.harvestMode = true;

        refCanvas.ship.OnTurnBegin += ReInit;
    }

    private void ReInit() {
        isTakeOff = false;
        panelAnim.SetBool("IsUp", false);

        refCanvas.mouse.moveMode = false;
        refCanvas.mouse.harvestMode = true;
        refCanvas.ship.CancelMove();
        refCanvas.ship.ShowActivePopulationPoints();
        refCanvas.ship.ShowHarvestOutline();
    }

    private void Update() {
        if (TurnManager.Instance.State != TurnManager.TurnState.Player) {
            if (Quaternion.Angle(hourglass.transform.localRotation, Quaternion.Euler(0, 0, 180f)) > 5f)
                hourglass.transform.localRotation *= Quaternion.Euler(0, 0, Time.deltaTime * 180f);
            else
                hourglass.transform.localRotation = Quaternion.Euler(0, 0, 180f);
        }
        else {
            if (Quaternion.Angle(hourglass.transform.localRotation, Quaternion.Euler(0, 0, 0f)) > 5f)
                hourglass.transform.localRotation *= Quaternion.Euler(0, 0, Time.deltaTime * 180f);
            else
                hourglass.transform.localRotation = Quaternion.Euler(0, 0, 0f);
        }

        if (GameManager.Input.IsBlock) {

        }
    }

    public void TakeOff() {
        if (TurnManager.Instance.State != TurnManager.TurnState.Player || isTakeOff)
            return;

        isTakeOff = true;
        panelAnim.SetBool("IsUp", true);
        
        AkSoundEngine.PostEvent("Play_SFX_Button_TakeOff", this.gameObject);
        refCanvas.mouse.moveMode = true;
        refCanvas.ship.MovementMode();
        refCanvas.mouse.harvestMode = false;

        refCanvas.ship.ClearActivePopulationPoints();
        refCanvas.ship.ClearHarvestOutline();
        refCanvas.ship.RedoMove();

    }

    public void Land() {
        if (TurnManager.Instance.State != TurnManager.TurnState.Player || !isTakeOff)
            return;

        isTakeOff = false;
        panelAnim.SetBool("IsUp", false);

        AkSoundEngine.PostEvent("Play_SFX_Button_Landing", this.gameObject);
        refCanvas.mouse.moveMode = false;
        refCanvas.ship.HarvestMode();
        refCanvas.mouse.harvestMode = true;

        refCanvas.ship.CancelMove();
        refCanvas.ship.ShowActivePopulationPoints();
        refCanvas.ship.ShowHarvestOutline();

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
