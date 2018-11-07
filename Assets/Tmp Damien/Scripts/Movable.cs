using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{
    public float speed;

    Vector2 beginPos;
    Vector2 targetPos;
    bool moving;
    float progress;

    void Start() {

    }

    private void Update() {
        if (moving) {
            progress += speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(beginPos, targetPos, progress);
            if (progress >= 1) {
                moving = true;
                progress = 0;
            }
        }
    }

    public void MoveTo(Vector2 targetPos) {
        if (!moving) {
            beginPos = transform.position;
            this.targetPos = targetPos;
            moving = true;
            progress = 0;
        }
    }

}

    
