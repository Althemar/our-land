using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    public GameObject panel;
    public GameObject templateResource;
    public Inventory inv;
    public ResourceType debugRes;

    int sizePool = 10;
    ResourceTemplate[] pool;

    void Awake() {
        pool = new ResourceTemplate[sizePool];
        for(int i = 0; i < sizePool; i++) {
            GameObject item = Instantiate(templateResource, Vector3.zero, Quaternion.identity, panel.transform);
            item.SetActive(false);
            pool[i] = item.GetComponent<ResourceTemplate>();
        }

        inv.OnInventoryChange += RefreshView;
        RefreshView();
    }

    public void DebugAdd() {
        inv.AddItem(debugRes, 10);
    }

    public void RefreshView() {
        for(int i = 0; i < sizePool; i++) {
            pool[i].gameObject.SetActive(false);
        }

        int index = 0;
        foreach(KeyValuePair<ResourceType, int> data in inv.resources) {
            ResourceTemplate template = pool[index++];
            template.icon.sprite = data.Key.icon;
            template.value.text = "" + data.Value;
            template.gameObject.SetActive(true);
        }
    }
    
}
