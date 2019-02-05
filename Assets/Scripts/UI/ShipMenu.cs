using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMenu : MonoBehaviour {
    public CanvasReference canvasRef;

    public GameObject shipMenu;
    public GameObject UI;
    bool isOpen;

    private void Start() {
        isOpen = false;
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Ship, false);
        UI.SetActive(true);
        shipMenu.SetActive(false);
    }

    public void Toogle() {
        isOpen ^= true;
        GameManager.Input.SetBlock(GameManager.Input.Blocker.Ship, isOpen);
        UI.SetActive(!isOpen);
        shipMenu.SetActive(isOpen);
    }
}
