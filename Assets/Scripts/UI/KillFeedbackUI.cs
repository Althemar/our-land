using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KillFeedbackUI : MonoBehaviour
{
    public Image sprite;

    public float speed;
    public float fadeSpeed;
    public float offsetY;

    public void Initialize() {
        Vector3 newPosition = transform.position;
        newPosition.y += offsetY;
        transform.position = newPosition;
    }

    public void Update() {
        Vector3 newPosition = transform.position;
        newPosition.y += speed * Time.deltaTime;
        transform.position = newPosition;

        Color color = sprite.color;
        color.a -= fadeSpeed * Time.deltaTime;
        sprite.color = color;

        if (color.a <= 0) {
            Destroy(gameObject);
        }
    }
}
