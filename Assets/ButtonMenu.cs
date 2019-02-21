using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Menu menu;
    public int index;
    public bool locked = false;

    public void OnPointerEnter(PointerEventData eventData) {
        if (!locked) {
            menu.ChangeSprite(index);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!locked) {
            menu.ChangeSprite(0);
        };
    }
}
