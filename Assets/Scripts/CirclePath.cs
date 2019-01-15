using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CirclePath : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler 
{
    public TMP_Text text;
    public Image removeImage;
    public bool interactable;
    public MotherShip motherShip;

    public void Start() {
        removeImage.gameObject.SetActive(false);
    }

    public void InitCirclePath() {
        text.gameObject.SetActive(true);
        removeImage.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (interactable && motherShip.targetTile) {
            text.gameObject.SetActive(false);
            removeImage.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (interactable && motherShip.targetTile) {
            text.gameObject.SetActive(true);
            removeImage.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (interactable && motherShip.targetTile) {
            motherShip.CancelMove();
        }
    }


}
