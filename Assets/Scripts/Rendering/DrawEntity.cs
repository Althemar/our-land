using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawEntity : MonoBehaviour
{
    public Camera harvestCam;
    public Camera shadowCam;
    public Camera entityCam;
    public Camera UICam;

    Material compositeMat, harvestZoneMat;

    public Color zebraColor;
    public float zebraSize = 25;
    [Range(0, 1)]
    public float shadowIntensity = 0.4f;
    [Range(0, 1)]
    public float effectIntensity = 1f;
    
    private void Awake() {
        compositeMat = new Material(Shader.Find("Hidden/Composite"));
        harvestZoneMat = new Material(Shader.Find("Hidden/HarvestZone"));
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        RenderTexture blendHarvest = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 4);
        RenderTexture blendShadow = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 4);
        RenderTexture blendEntity = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default, 4);

        compositeMat.SetTexture("_Composite", shadowCam.activeTexture);
        Graphics.Blit(src, blendShadow, compositeMat);
        compositeMat.SetTexture("_Composite", entityCam.activeTexture);
        Graphics.Blit(blendShadow, blendEntity, compositeMat);
        harvestZoneMat.SetTexture("_Composite", harvestCam.activeTexture);
        harvestZoneMat.SetColor("_Color", zebraColor);
        harvestZoneMat.SetFloat("_ZebraSize", zebraSize);
        harvestZoneMat.SetFloat("_ShadowIntensity", shadowIntensity);
        harvestZoneMat.SetFloat("_EffectIntensity", effectIntensity); 
        Graphics.Blit(blendEntity, blendHarvest, harvestZoneMat);
        compositeMat.SetTexture("_Composite", UICam.activeTexture);
        Graphics.Blit(blendHarvest, dest, compositeMat);

        RenderTexture.ReleaseTemporary(blendShadow);
        RenderTexture.ReleaseTemporary(blendEntity);
        RenderTexture.ReleaseTemporary(blendHarvest);
    }
}
