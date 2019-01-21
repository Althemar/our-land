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
        I18N.OnLangChange += UpdateText;
    }

    void OnDisable() {
        UpdateText();
        I18N.OnLangChange -= UpdateText;
    }

    public void UpdateText() {
        textMesh.text = I18N.GetText(key);
    }
}