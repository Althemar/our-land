using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverHighlight : MonoBehaviour {
    private void OnMouseEnter() {
        GetComponent<SpriteRenderer>().material.SetFloat("_Intensity", 0.5f);
    }

    private void OnMouseExit() {
        GetComponent<SpriteRenderer>().material.SetFloat("_Intensity", 0f);
    }
}
