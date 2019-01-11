using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour {
    Camera worldCam, UICam;
    RenderTexture entity;

    private void Awake() {
        UICam = GetComponent<Camera>();
        worldCam = transform.parent.GetComponent<Camera>();

        UICam.enabled = true;
        worldCam.cullingMask ^= UICam.cullingMask;
    }

    private void Update() {
        UICam.orthographicSize = worldCam.orthographicSize;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (entity)
            RenderTexture.ReleaseTemporary(entity);
        entity = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        UICam.targetTexture = entity;

        Graphics.Blit(src, dest);
    }
}
