using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{

    public AnimationCurve alphaDistance;
    public SpriteRenderer spriteRenderer;

    private CameraControl cam;
    private Color tmpColor;

    public void Initialize(CameraControl cam) {
        this.cam = cam;
    }
    
    void Update()
    {
        tmpColor = spriteRenderer.color;
        tmpColor.a = alphaDistance.Evaluate(cam.GetZoomValue());
        spriteRenderer.color = tmpColor;    
    }
}
