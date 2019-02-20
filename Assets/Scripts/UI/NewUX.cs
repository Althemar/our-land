using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewUX : MonoBehaviour {
    public CanvasReference refCanvas;

    public GameObject hourglass;

    public Image slot1;
    public Image slot2;
    public Image slot3;
    public Image slot4;

    public Sprite slotToken;
    public Sprite slotHarvest;
    public Sprite slotResearch;

    Animator panelAnim;

    bool isTakeOff;


    void Start() {
        panelAnim = GetComponent<Animator>();

        refCanvas.mouse.moveMode = false;
        refCanvas.mouse.harvestMode = true;

        refCanvas.ship.OnTurnBegin += ReInit;
        refCanvas.ship.OnRemainingPointsChanged += UpdateActionPointsCount;
        UpdateActionPointsCount();
    }

    void UpdateActionPointsCount() {
        slot1.sprite = slotToken;
        slot2.sprite = slotToken;
        slot3.sprite = slotToken;
        slot4.sprite = slotToken;

        int max = refCanvas.ship.maxPopulationPoints;
        int remaining = refCanvas.ship.remainingPopulationPoints;
        int harvestPoint = refCanvas.ship.populationPoints.Count;
        int researchPoint = max - remaining - harvestPoint;
        
        if (researchPoint >= 1)
            GetSlot(max).sprite = slotResearch;
        if (researchPoint >= 2)
            GetSlot(max - 1).sprite = slotResearch;
        if (researchPoint >= 3)
            GetSlot(max - 2).sprite = slotResearch;
        if (researchPoint >= 4)
            GetSlot(max - 3).sprite = slotResearch;

        int maxHarvest = max - researchPoint;
        if (harvestPoint >= 1)
            GetSlot(maxHarvest).sprite = slotHarvest;
        if (harvestPoint >= 2)
            GetSlot(maxHarvest - 1).sprite = slotHarvest;
        if (harvestPoint >= 3)
            GetSlot(maxHarvest - 2).sprite = slotHarvest;
        if (harvestPoint >= 4)
            GetSlot(maxHarvest - 3).sprite = slotHarvest;


        slot1.color = Color.clear;
        slot2.color = Color.clear;
        slot3.color = Color.clear;
        slot4.color = Color.clear;
        if (max >= 1)
            slot1.color = Color.white;
        if (max >= 2)
            slot2.color = Color.white;
        if (max >= 3)
            slot3.color = Color.white;
        if (max >= 4)
            slot4.color = Color.white;
    }

    Image GetSlot(int id) {
        switch(id) {
            case 1: return slot1;
            case 2: return slot2;
            case 3: return slot3;
            case 4: return slot4;
        }
        return null;
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
