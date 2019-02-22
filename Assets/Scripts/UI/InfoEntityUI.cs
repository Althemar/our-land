using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InfoEntityUI : MonoBehaviour
{
    public TMP_Text text;

    [HideInInspector]
    public Entity currentEntity;

    public GameObject ressourcesPreview;
    public GameObject ressourcesTemplate;
    //public ResourceTemplate ressourcesTemplate;

    public static InfoEntityUI Instance;

    private Image background;

    private void Start() {
        Instance = this;
        background = GetComponent<Image>();
    }

    public void Initialize(Entity entity) {
        background.enabled = true;
        currentEntity = entity;

        float population;
        if (entity.population < 0) {
            population = 1;
        }
        else {
            population = Mathf.Floor(entity.population);
        }
        text.text = "" + population + " " + entity.entitySO.name;

        foreach(Transform trans in ressourcesPreview.transform) {
            Destroy(trans.gameObject);
        }

        foreach(var res in entity.entitySO.resources) {
            
            GameObject temp = Instantiate(ressourcesTemplate, ressourcesPreview.transform);
            temp.transform.GetChild(0).GetComponent<TMP_Text>().text = "" + res.Value.gain[entity.HarvestedBonus];
            temp.transform.GetChild(1).GetComponent<Image>().sprite = res.Key.icon;
            
        }
    }

    public void Clear() {
        background.enabled = false;
        currentEntity = null;
        text.text = "";
        foreach (Transform trans in ressourcesPreview.transform) {
            Destroy(trans.gameObject);
        }
    }

}
