using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CirclePath : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler 
{
    public TMP_Text text;
    public Sprite jonction;
    public Sprite destination;
    public Sprite remove;
    public Image pointImage;
    public Image removeImage;
    public bool interactable;
    public MotherShip motherShip;

    public void InitCirclePath() {
        text.gameObject.SetActive(true);
        removeImage.gameObject.SetActive(false);
    }

    public void SetJonctionSprite() {
        pointImage.sprite = jonction;
    }
    
    public void SetDestinationSprite() {
        pointImage.sprite = destination;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (interactable && motherShip.targetTile) {
            text.gameObject.SetActive(false);
            pointImage.sprite = remove;
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (interactable && motherShip.targetTile) {
            text.gameObject.SetActive(true);
            pointImage.sprite = destination;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (interactable && motherShip.targetTile) {
            motherShip.CancelMove();
        }
    }


}
