using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class LocalisedText : MonoBehaviour {
    TextMeshProUGUI textMesh;
    public string key;

    void Awake () {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable() {
        UpdateText();
    }

    public void UpdateText() {
        textMesh.text = I18N.GetText(key);
    }
}