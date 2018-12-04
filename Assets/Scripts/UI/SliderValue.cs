using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour {
    TextMeshProUGUI textMesh;
    public Slider slider;

    void Start () {
        textMesh = GetComponent<TextMeshProUGUI> ();
    }

    void Update () {
        textMesh.text = "" + Mathf.Round(slider.value);
    }
}