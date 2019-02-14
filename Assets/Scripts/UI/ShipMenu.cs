using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipMenu : MonoBehaviour {
    public CanvasReference canvasRef;

    public GameObject shipMenu;
    RectTransform menu;
    Vector3 beginPosition;
    
    bool isOpen;

    private void Start() {
        isOpen = false;
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Ship, false);
        shipMenu.SetActive(false);

        menu = shipMenu.transform.parent.GetComponent<RectTransform>();
        beginPosition = menu.anchoredPosition;
    }

    public void Toogle() {
        isOpen ^= true;

        if (isOpen)
            AkSoundEngine.PostEvent("Play_SFX_Button_IGMenu_Open", this.gameObject);
        else
            AkSoundEngine.PostEvent("Play_SFX_Button_IGMenu_Close", this.gameObject);

        StopAllCoroutines();
        StartCoroutine(isOpen ? Open() : Close());
    }

    IEnumerator Open() {
        shipMenu.SetActive(true);
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Ship, true);

        float progress = 0;
        Vector3 currentPos = menu.anchoredPosition;
        while (progress <= 1) {
            menu.anchoredPosition = Vector3.Lerp(currentPos, new Vector3(0, beginPosition.y, beginPosition.z), progress);
            progress += Time.deltaTime;
            yield return null;
        }
        menu.anchoredPosition =new Vector3(0, beginPosition.y, beginPosition.z);
    }

    IEnumerator Close() {
        float progress = 0;
        Vector3 currentPos = menu.anchoredPosition;
        while (progress <= 1) {
            menu.anchoredPosition = Vector3.Lerp(currentPos, beginPosition, progress);
            progress += Time.deltaTime;
            yield return null;
        }
        menu.anchoredPosition = beginPosition;

        shipMenu.SetActive(false);
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Ship, false);
    }

}
