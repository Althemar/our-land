using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ResourceTemplate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Image icon;
    public TextMeshProUGUI value;
    public GameObject panel;
    public TextMeshProUGUI textPanel;
    public bool canShowPanel = false;

    public void OnPointerEnter(PointerEventData eventData) {
        panel.SetActive(canShowPanel);
    }

    public void OnPointerExit(PointerEventData eventData) {
        panel.SetActive(false);
    }
}
