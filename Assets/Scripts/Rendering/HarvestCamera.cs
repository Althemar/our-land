using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestCamera : MonoBehaviour {
    Camera worldCam, HarvestCam;
    RenderTexture entity;

    private void Awake() {
        HarvestCam = GetComponent<Camera>();
        worldCam = transform.parent.GetComponent<Camera>();

        HarvestCam.enabled = true;
        worldCam.cullingMask ^= HarvestCam.cullingMask;
    }

    private void Update() {
        HarvestCam.orthographicSize = worldCam.orthographicSize;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (entity)
            RenderTexture.ReleaseTemporary(entity);
        entity = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 4);
        HarvestCam.targetTexture = entity;

        Graphics.Blit(src, dest);
    }
}
