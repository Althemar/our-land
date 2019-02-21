using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class SnapButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject snap;

    public float offset = 5;
    Vector3 initPos;

    void Start() {
        initPos = snap.GetComponent<RectTransform>().anchoredPosition;
    }

    //Do this when the mouse is clicked over the selectable object this script is attached to.
    public void OnPointerDown(PointerEventData eventData) {
        snap.GetComponent<RectTransform>().anchoredPosition = initPos + Vector3.down * offset;
    }

    //Do this when the mouse is clicked over the selectable object this script is attached to.
    public void OnPointerUp(PointerEventData eventData) {
        snap.GetComponent<RectTransform>().anchoredPosition = initPos;
    }
}