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
            temp.value.text = "+" + (res.Value * Mathf.Floor(entity.population));
            temp.value.color = Color.black;
        }
        ResourceTemplate pa = Instantiate(ressourcesTemplate, ressourcesPreview.transform);
        pa.GetComponent<RectTransform>().sizeDelta = new Vector2(2f, 1);
        pa.icon.sprite = null;
        pa.icon.color = Color.clear;
        pa.value.text = "-1 PA";
        pa.value.color = Color.black;

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
        Playtest.TimedLog("Harvest " + Mathf.Floor(entity.population) + " " + entity.entitySO.name);
        foreach (KeyValuePair<ResourceType, int> resource in entity.entitySO.resources) {
            entitiesHarvestable.motherShip.Inventory.AddItem(resource.Key, resource.Value * Mathf.Floor(entity.population));
        }
        entitiesHarvestable.motherShip.RemainingActionPoints--;
        entity.Kill();
        if(entity.entitySO)
            AkSoundEngine.PostEvent(entity.entitySO.harvestSound, GameManager.Instance.gameObject);
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
