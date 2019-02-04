using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PopulationSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Button button;

    private Entity entity;
    private EntitiesHarvestableUI entitiesHarvestable;

    private bool pointerIsOnButton = false;

    public bool PointerIsOnButton {
        get => pointerIsOnButton;
    }

    public void Initialize(Entity entity, EntitiesHarvestableUI entitiesHarvestable) {
        this.entity = entity;
        this.entitiesHarvestable = entitiesHarvestable;
        button = GetComponent<Button>();
        
        if (entity.populationPoint)
            button.interactable = false;
        else if (entitiesHarvestable.motherShip.remainingPopulationPoints > 0)
            button.interactable = true;
        else
            button.interactable = false;

        pointerIsOnButton = false;
    }

    public void Refresh() {
        if (entity.populationPoint)
            button.interactable = false;
        else if (entitiesHarvestable.motherShip.remainingPopulationPoints > 0)
            button.interactable = true;
        else
            button.interactable = false;

        pointerIsOnButton = false;
    }

    public void HarvestEntity() {
        if (entitiesHarvestable.motherShip.remainingPopulationPoints > 0) {
            entitiesHarvestable.activePopulationPoints.PlacePopulationPoint(entity);
        }
        entitiesHarvestable.UpdateButtons();
    }



    public void OnPointerEnter(PointerEventData eventData) {
        pointerIsOnButton = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        pointerIsOnButton = false;
    }
}
