using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCamera : MonoBehaviour
{
    Camera worldCam, entityCam;
    RenderTexture entity;

    private void Awake() {
        entityCam = GetComponent<Camera>();
        worldCam = transform.parent.GetComponent<Camera>();

        entityCam.enabled = true;
        worldCam.cullingMask ^= entityCam.cullingMask;
    }

    private void Update() {
        entityCam.orthographicSize = worldCam.orthographicSize;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (entity)
            RenderTexture.ReleaseTemporary(entity);
        entity = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 4);
        entityCam.targetTexture = entity;
        
        Graphics.Blit(src, dest);
    }
}
