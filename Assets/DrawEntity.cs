using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawEntity : MonoBehaviour
{
    public Camera shadowCam;
    public Camera entityCam;

    Material compositeMat;

    private void Awake() {
        compositeMat = new Material(Shader.Find("Hidden/Composite"));
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        RenderTexture blend = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

        compositeMat.SetTexture("_Composite", shadowCam.activeTexture);
        Graphics.Blit(src, blend, compositeMat);
        compositeMat.SetTexture("_Composite", entityCam.activeTexture);
        Graphics.Blit(blend, dest, compositeMat);

        RenderTexture.ReleaseTemporary(blend);
    }
}
