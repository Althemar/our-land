using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HarvestEntityUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text text;
    public Button button;

    private Entity entity;
    private EntitiesHarvestableUI entitiesHarvestable;

    private bool pointerIsOnButton = false;

    public bool PointerIsOnButton
    {
        get => pointerIsOnButton;
    }

    public void Initialize(Entity entity, EntitiesHarvestableUI entitiesHarvestable) {
        text.text = "Harvest " + entity.entitySO.name + "\nCost 1PA";
        this.entity = entity;
        this.entitiesHarvestable = entitiesHarvestable;
        if (entitiesHarvestable.motherShip.RemainingActionPoints > 0) {
            button = GetComponent<Button>();
            button.interactable = true;
        }
        else {
            button.interactable = false;
        }
        pointerIsOnButton = false;
    }

    public void HarvestEntity() {
        foreach (KeyValuePair<ResourceType, int> resource in entity.entitySO.resources) {
            entitiesHarvestable.motherShip.Inventory.AddItem(resource.Key, resource.Value);
        }
        entitiesHarvestable.motherShip.RemainingActionPoints--;
        entity.Kill();
        entitiesHarvestable.Clear();
        entitiesHarvestable.DisplayHarvestedResources(entity);
    }



    public void OnPointerEnter(PointerEventData eventData) {
        pointerIsOnButton = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        pointerIsOnButton = false;
    }
}
