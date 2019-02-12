using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseUI : MonoBehaviour {
    Image img;
    public Sprite up, down, grab;

    void Start() {
        img = GetComponent<Image>();
    }

    void Update() {
        Cursor.visible = false;
        img.transform.localPosition = Input.mousePosition - new Vector3(Screen.width, Screen.height, 0) / 2;
        Shader.SetGlobalVector("_MousePosition", Input.mousePosition);
        if (Input.GetMouseButton(0))
            img.sprite = down;
        else
            img.sprite = up;


        if (Input.GetMouseButton(1))
            img.sprite = grab;
    }
}
