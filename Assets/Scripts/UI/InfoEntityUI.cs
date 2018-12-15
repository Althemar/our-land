using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InfoEntityUI : MonoBehaviour
{
    public TMP_Text text;

    private Entity entity;
    private EntitiesHarvestableUI entitiesHarvestable;

    public GameObject ressourcesPreview;
    public ResourceTemplate ressourcesTemplate;

    public void Initialize(Entity entity, EntitiesHarvestableUI entitiesHarvestable) {
        text.text = "" + Mathf.Floor(entity.population) + " " + entity.entitySO.name;
        this.entity = entity;
        this.entitiesHarvestable = entitiesHarvestable;

        foreach(Transform trans in ressourcesPreview.transform) {
            Destroy(trans.gameObject);
        }

        foreach(var res in entity.entitySO.resources) {
            ResourceTemplate temp = Instantiate(ressourcesTemplate, ressourcesPreview.transform);
            temp.icon.sprite = res.Key.icon;
            temp.value.text = "" + res.Value * Mathf.Floor(entity.population);
        }
    }

}
