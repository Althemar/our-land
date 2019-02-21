using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public float fadeSpeed = 0.7f;
    public Image image;

    private Color tmpColor;

    public void Awake() {
        image.gameObject.SetActive(true);
        tmpColor = image.color;
        tmpColor.a = 1;
        image.color = tmpColor;
        StartCoroutine(FadeImage());
    }

    public IEnumerator FadeImage() {
        while (image.color.a > 0) {
            tmpColor = image.color;
            tmpColor.a -= fadeSpeed * Time.deltaTime;
            image.color = tmpColor;
            yield return null;
        }
        image.gameObject.SetActive(false);
    }
}
