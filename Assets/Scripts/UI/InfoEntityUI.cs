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
    public GameObject infoEntity;
    //public ResourceTemplate ressourcesTemplate;

    public void Initialize(Entity entity, EntitiesHarvestableUI entitiesHarvestable) {
        float population;
        if (entity.population < 0) {
            population = 1;
        }
        else {
            population = Mathf.Floor(entity.population);
        }
        text.text = "" + population + " " + entity.entitySO.name;
        this.entity = entity;
        this.entitiesHarvestable = entitiesHarvestable;

        foreach(Transform trans in ressourcesPreview.transform) {
            Destroy(trans.gameObject);
        }

        foreach(var res in entity.entitySO.resources) {
            /*
            ResourceTemplate temp = Instantiate(ressourcesTemplate, ressourcesPreview.transform);
            temp.icon.sprite = res.Key.icon;
            temp.value.text = "" + res.Value.gain[entity.HarvestedBonus];
            */
        }
    }

}
