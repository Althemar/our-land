using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PopulationSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Button button;
    public Image icon;

    public Image[] bonus;
    public Sprite bonusOn, bonusOff;

    private Entity entity;
    private EntitiesHarvestableUI entitiesHarvestable;

    private bool pointerIsOnButton = false;

    public bool PointerIsOnButton {
        get => pointerIsOnButton;
    }

    public void Initialize(Entity entity, EntitiesHarvestableUI entitiesHarvestable) {
        this.entity = entity;
        this.entitiesHarvestable = entitiesHarvestable;
        icon.sprite = entity.entitySO.iconEntity;
        button = GetComponent<Button>();

        for(int i = 0; i < 3; i++) {
            bonus[i].sprite = entity.HarvestedBonus > i ? bonusOn : bonusOff;
        }
        
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
            AkSoundEngine.PostEvent("Play_SFX_Button_PPOn", this.gameObject);
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
