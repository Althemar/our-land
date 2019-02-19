using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [ReorderableList]
    public List<RuntimeAnimatorController> animators;
   
    [ReorderableList]
    public List<Color> possibleColors;

    public float spawnChance;

    public AnimationCurve alphaDistance;

    private CameraControl cam;
    private Color tmpColor;
    
    void Start() { 
        if (Random.value < spawnChance) {
            animator.runtimeAnimatorController = animators[Random.Range(0, animators.Count)];
        }
        if (possibleColors.Count > 0) {
            spriteRenderer.color = possibleColors[Random.Range(0, possibleColors.Count)];
        }
        cam = Camera.main.transform.parent.GetComponent<CameraControl>();
    }



    private void Update() {
        tmpColor = spriteRenderer.color;
        tmpColor.a = alphaDistance.Evaluate(cam.GetZoomValue());
        spriteRenderer.color = tmpColor;
    }
}
