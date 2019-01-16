using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeUI : MonoBehaviour {
    public MouseController mouse;
    public DrawEntity shaderComposite;
    public GameObject buttons;
    public GameObject harvestObject;

    public void ToogleToHarvest() {
        mouse.harvestMode = true;
        StartCoroutine(ShowZebra(1f));
        buttons.SetActive(false);
        harvestObject.SetActive(true);
    }

    public void Cancel() {
        mouse.harvestMode = false;
        StopAllCoroutines();
        mouse.cameraman.ChangeZoomCamera(3f);
        shaderComposite.effectIntensity = 0.0f;
        harvestObject.SetActive(false);
        buttons.SetActive(true);
    }
    
    private IEnumerator ShowZebra(float time) {
        shaderComposite.effectIntensity = 0.0f;
        for (float t = shaderComposite.effectIntensity; t < 1.0f; t += Time.deltaTime / time) {
            shaderComposite.effectIntensity = t;
            yield return null;
        }
    }

}
