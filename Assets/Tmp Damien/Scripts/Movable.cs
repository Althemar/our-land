using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{
    public float speed;

    Vector3 beginPos;
    Vector3 targetPos;
    bool moving;
    float progress;

    void Start() {

    }

    private void Update() {
        if (moving) {
            progress += speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(beginPos, targetPos, progress);
            if (transform.position == targetPos) {
                moving = false;
                progress = 0;
            }
        }
    }

    public void MoveTo(Vector3 targetPos) {
        if (!moving) {
            beginPos = transform.position;
            this.targetPos = targetPos;
            moving = true;
            progress = 0;
        }
    }

}

    
