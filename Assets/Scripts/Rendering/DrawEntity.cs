using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawEntity : MonoBehaviour
{
    public Camera shadowCam;
    public Camera entityCam;
    public Camera UICam;

    Material compositeMat;

    private void Awake() {
        compositeMat = new Material(Shader.Find("Hidden/Composite"));
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        RenderTexture blendShadow = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
        RenderTexture blendEntity = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

        compositeMat.SetTexture("_Composite", shadowCam.activeTexture);
        Graphics.Blit(src, blendShadow, compositeMat);
        compositeMat.SetTexture("_Composite", entityCam.activeTexture);
        Graphics.Blit(blendShadow, blendEntity, compositeMat);
        compositeMat.SetTexture("_Composite", UICam.activeTexture);
        Graphics.Blit(blendEntity, dest, compositeMat);

        RenderTexture.ReleaseTemporary(blendShadow);
        RenderTexture.ReleaseTemporary(blendEntity);
    }
}
