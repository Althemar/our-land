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

    public void Initialize(ResourceType resource, float count) {
        image.sprite = resource.icon;
        text.text = "+ " + count;
        Vector3 newPosition = transform.position;
        newPosition.y += offsetY;
        transform.position = newPosition;
    }

    public void Update() {
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
