using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public float fadeSpeed = 0.7f;
    public Image image;
    public bool fadeAtStart;
    public Fade logo;

    private Color tmpColor;

    private bool displayLogo;

    public void Awake() {
        if (fadeAtStart) {
            StartFade(true);
        }
    }

    public void StartFade(bool fadeOut) {
        image.gameObject.SetActive(true);
        tmpColor = image.color;
        if (fadeOut) {
            tmpColor.a = 1;
        }
        else {
            tmpColor.a = 0;
        }
        image.color = tmpColor;
        StartCoroutine(FadeImage(fadeOut));
    }

    private void Update() {
        if (logo && Input.GetKeyDown(KeyCode.F)) {
            displayLogo = true;
            StartCoroutine(FadeImage(false));
        }
    }

    public IEnumerator FadeImage(bool fadeOut) {
        bool alpha = (fadeOut) ? (image.color.a > 0) : (image.color.a < 1);
        while (alpha) {
            tmpColor = image.color;
            if (fadeOut) {
                tmpColor.a -= fadeSpeed * Time.deltaTime;
            }
            else {
                tmpColor.a += fadeSpeed * Time.deltaTime;
            }
            image.color = tmpColor;
            alpha = (fadeOut) ? (image.color.a > 0) : (image.color.a < 1);
            yield return null;
        }

        if (displayLogo && logo) {
            logo.StartFade(false);
            AkSoundEngine.PostEvent("END", gameObject);
        }
        
    }
}
