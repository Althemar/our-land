using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCamera : MonoBehaviour {

    Material shadowMat;
    Material blurMat;

    Camera worldCam, entityCam;
    RenderTexture entity;

    private void Awake() {
        shadowMat = new Material(Shader.Find("Hidden/ShadowFill"));
        blurMat = new Material(Shader.Find("Hidden/Blur"));

        entityCam = GetComponent<Camera>();
        worldCam = transform.parent.GetComponent<Camera>();

        entityCam.enabled = true;
        worldCam.cullingMask ^= entityCam.cullingMask;
    }

    private void Update() {
        entityCam.orthographicSize = worldCam.orthographicSize;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if(entity)
            RenderTexture.ReleaseTemporary(entity);
        entity = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        entityCam.targetTexture = entity;

        blurMat.SetVector("_BlurSize", new Vector2(entity.texelSize.x * 3f, entity.texelSize.y * 3f));

        RenderTexture shadow = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        RenderTexture tmp = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        Graphics.Blit(src, shadow, shadowMat);

        for(int i = 0; i < 4; i++) {
            Graphics.Blit(shadow, tmp, blurMat, 0);
            Graphics.Blit(tmp, shadow, blurMat, 1);
        }

        Graphics.Blit(shadow, dest);
        RenderTexture.ReleaseTemporary(tmp);
        RenderTexture.ReleaseTemporary(shadow);
    }

}
