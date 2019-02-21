using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class Infobox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject panel;

    public void OnPointerEnter(PointerEventData eventData) {
        panel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        panel.SetActive(false);
    }
}