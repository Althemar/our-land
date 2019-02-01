using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceUI : MonoBehaviour {
    public CanvasReference refCanvas;

    public GameObject panel;
    public GameObject templateResource;

    int sizePool = 10;
    ResourceTemplate[] pool;

    void Awake() {
        pool = new ResourceTemplate[sizePool];
        for (int i = 0; i < sizePool; i++) {
            GameObject item = Instantiate(templateResource, Vector3.zero, Quaternion.identity, panel.transform);
            item.SetActive(false);
            pool[i] = item.GetComponent<ResourceTemplate>();
            pool[i].canShowPanel = true;
        }
    }

    private void Start() {
        refCanvas.ship.Inventory.OnInventoryChange += RefreshView;
        RefreshView();

        I18N.OnLangChange += RefreshView;
    }

    public void RefreshView() {
        for (int i = 0; i < sizePool; i++) {
            pool[i].gameObject.SetActive(false);
        }

        int index = 0;
        foreach (KeyValuePair<ResourceType, float> data in refCanvas.ship.Inventory.resources) {
            ResourceTemplate template = pool[index++];
            template.icon.sprite = data.Key.icon;
            template.value.text = "" + data.Value;
            template.textPanel.text = I18N.GetText(data.Key.infoboxPanel);
            template.textPanel.spriteAsset = data.Key.textIcons;
            template.gameObject.SetActive(true);
        }
    }

}
