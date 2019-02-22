using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceHarvestedUI : MonoBehaviour
{
    public Image image;
    public TMP_Text text;

    public float speed;
    public float fadeSpeed;
    public float offsetY;

    bool canUpdate = false;

    public void Initialize(ResourceType resource, float count, float delay) {
        if(delay > 0) {
            image.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            StartCoroutine(DelayedInit(resource, count, delay));
            return;
        }
        image.sprite = resource.icon;
        text.text = "+ " + count;
        Vector3 newPosition = transform.position;
        newPosition.y += offsetY;
        transform.position = newPosition;
        canUpdate = true;
    }

    IEnumerator DelayedInit(ResourceType resource, float count, float delay) {
        yield return new WaitForSeconds(delay);
        image.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
        Initialize(resource, count, 0);
    }

    public void Update() {
        if(!canUpdate)
            return;

        Vector3 newPosition = transform.position;
        newPosition.y += speed * Time.deltaTime;
        transform.position = newPosition;

        Color color = image.color;
        color.a -= fadeSpeed * Time.deltaTime;
        image.color = color;
        text.color = color;

        if (color.a <= 0) {
            Destroy(gameObject);
        }
    }
}
