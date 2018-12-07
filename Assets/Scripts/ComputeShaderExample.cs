using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderExample : MonoBehaviour
{
    public ComputeShader shader;
    public RenderTexture tex;

    // Start is called before the first frame update
    void Start()
    {
        int kernelIndex = shader.FindKernel("CSMain");
        tex = new RenderTexture(512, 512, 24);
        tex.enableRandomWrite = true;
        tex.Create();

        shader.SetTexture(kernelIndex, "Result", tex);
        shader.Dispatch(kernelIndex, 512 / 8, 512 / 8, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
