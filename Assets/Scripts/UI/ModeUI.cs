using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModeUI : MonoBehaviour {
    public MouseController mouse;
    public DrawEntity shaderComposite;

    public GameObject harvestPoint;

    public Button actionButton;
    public Toggle harvestButton;
    public Toggle moveButton;
    public Toggle skipButton;

    TextMeshProUGUI actionText;
    TextMeshProUGUI harvestText;
    TextMeshProUGUI moveText;
    TextMeshProUGUI skipText;
    
    private void Start() {
        actionText = actionButton.GetComponentInChildren<TextMeshProUGUI>();
        harvestText = harvestButton.GetComponentInChildren<TextMeshProUGUI>();
        moveText = moveButton.GetComponentInChildren<TextMeshProUGUI>();
        skipText = skipButton.GetComponentInChildren<TextMeshProUGUI>();

        actionText.text = I18N.GetText("selectAction");
    }

    private void Update() {
        bool activeButton = false;

        harvestPoint.SetActive(false);
        if (harvestButton.isOn) {
            harvestPoint.SetActive(true);
            if (mouse.motherShip.populationPoints.Count == 1) {
                actionText.text = I18N.GetText("harvestEntity");
                activeButton = true;
            }
            else if (mouse.motherShip.populationPoints.Count > 1) {
                actionText.text = I18N.GetText("harvestEntities");
                activeButton = true;
            }
            else
                actionText.text = I18N.GetText("chooseEntities");
        }
        else if (moveButton.isOn) {
            if (mouse.motherShip.targetTile) {
                actionText.text = I18N.GetText("goTo");
                activeButton = true;
            }
            else
                actionText.text = I18N.GetText("chooseDestination");
        }
        else if (skipButton.isOn) {
            actionText.text = I18N.GetText("skipOk");
            activeButton = true;
        }
        else {
            actionText.text = I18N.GetText("selectAction");
        }

        if (TurnManager.Instance.State != TurnManager.TurnState.Player && TurnManager.Instance.State != TurnManager.TurnState.PlayerUpdates) {
            harvestButton.isOn = false;
            moveButton.isOn = false;
            skipButton.isOn = false;

            mouse.motherShip.CancelMove();
            mouse.motherShip.RemoveActiveActionPoints();
        }

        if (TurnManager.Instance.State != TurnManager.TurnState.Player || GameManager.Input.IsBlock) {
            actionText.text = I18N.GetText("pleaseWait");
            activeButton = false;

            harvestButton.interactable = false;
            moveButton.interactable = false;
            skipButton.interactable = false;
        } else {
            harvestButton.interactable = true;
            moveButton.interactable = true;
            skipButton.interactable = true;
        }

        if(activeButton) {
            actionButton.interactable = true;
            actionText.color = new Color(0f, 0f, 0f);
        } else {
            actionButton.interactable = false;
            actionText.color = new Color(1f, 1f, 1f);
        }
    }

    public void DoAction() {
        if (TurnManager.Instance.State != TurnManager.TurnState.Player)
            return;

        if (harvestButton.isOn && mouse.motherShip.populationPoints.Count > 0) {
            TurnManager.Instance.EndTurn();
        }
        else if (moveButton.isOn && mouse.motherShip.targetTile) {
            TurnManager.Instance.EndTurn();
        }
        else if (skipButton.isOn) {
            TurnManager.Instance.EndTurn();
        }
    }

    public void ToogleHarvest(bool state) {
        mouse.harvestMode = state;
        if (state) {
            mouse.motherShip.ShowActiveActionPoints();
            harvestButton.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);
            harvestText.color = new Color(1f, 1f, 1f);
            StartCoroutine(ShowZebra(1f));
            mouse.cameraman.SetTarget(mouse.motherShip.transform.position);
        }
        else {
            mouse.motherShip.ClearActiveActionPoints();
            harvestButton.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
            harvestText.color = new Color(0f, 0f, 0f);
            StopAllCoroutines();
            shaderComposite.effectIntensity = 0.0f;
        }
    }

    public void ToogleMove(bool state) {
        mouse.moveMode = state;
        if (state) {
            moveButton.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);
            moveText.color = new Color(1f, 1f, 1f);

            mouse.motherShip.ClearHarvestOutline();
            mouse.motherShip.RedoMove();
        }
        else {
            moveButton.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
            moveText.color = new Color(0f, 0f, 0f);

            mouse.motherShip.ShowHarvestOutline();
            mouse.motherShip.CancelMove();
        }
    }

    public void ToogleSkip(bool state) {
        if (state) {
            skipButton.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);
            skipText.color = new Color(1f, 1f, 1f);
        }
        else {
            skipButton.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
            skipText.color = new Color(0f, 0f, 0f);
        }
    }

    private IEnumerator ShowZebra(float time) {
        shaderComposite.effectIntensity = 0.0f;
        for (float t = shaderComposite.effectIntensity; t < 1.0f; t += Time.deltaTime / time) {
            shaderComposite.effectIntensity = t;
            yield return null;
        }
    }

}
