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

    public GameObject ressourcesPreview;
    public ResourceTemplate ressourcesTemplate;

    private bool pointerIsOnButton = false;

    public bool PointerIsOnButton
    {
        get => pointerIsOnButton;
    }

    public void Initialize(Entity entity, EntitiesHarvestableUI entitiesHarvestable) {
        text.text = "Harvest " + entity.entitySO.name;

        foreach (Transform t in ressourcesPreview.transform)
            Destroy(t.gameObject);

        foreach (var res in entity.entitySO.resources) {
            ResourceTemplate temp = Instantiate(ressourcesTemplate, ressourcesPreview.transform);
            temp.GetComponent<RectTransform>().sizeDelta = new Vector2(2f, 1);
            temp.icon.sprite = res.Key.icon;
            temp.value.text = "+" + (res.Value.gain[entity.HarvestedBonus]);
            temp.value.color = Color.black;
        }
        ResourceTemplate pa = Instantiate(ressourcesTemplate, ressourcesPreview.transform);
        pa.GetComponent<RectTransform>().sizeDelta = new Vector2(3f, 1);
        pa.icon.sprite = null;
        pa.icon.color = Color.clear;
        pa.value.text = "-1 PA";
        pa.value.color = Color.black;

        this.entity = entity;
        this.entitiesHarvestable = entitiesHarvestable;
        if (entitiesHarvestable.motherShip.remainingPopulationPoints > 0) {
            button = GetComponent<Button>();
            button.interactable = true;
        }
        else {
            button.interactable = false;
        }
        pointerIsOnButton = false;
    }

    public void HarvestEntity() {
        Playtest.TimedLog("Harvest " + Mathf.Floor(entity.population) + " " + entity.entitySO.name);

        if (entitiesHarvestable.motherShip.remainingPopulationPoints > 0) {
            entitiesHarvestable.activePopulationPoints.PlacePopulationPoint(entity);
        }
        entitiesHarvestable.Clear();
    }



    public void OnPointerEnter(PointerEventData eventData) {
        pointerIsOnButton = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        pointerIsOnButton = false;
    }
}
