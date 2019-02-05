using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasReference : MonoBehaviour {
    public MouseController mouse;
    public MotherShip ship;
    public GameObject mainCamera;

    [HideInInspector]
    public DrawEntity entityDraw;

    void Awake() {
        entityDraw = mainCamera.GetComponent<DrawEntity>();
    }
}
