using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultComputeShader : MonoBehaviour
{
    public ComputeShader computeShader;
    int csKernel;
    Texture2D resultTexture;
    // Start is called before the first frame update

    public Material targetMaterial;
    void Start()
    {
        resultTexture = new Texture2D(64, 64, TextureFormat.ARGB32, false, true);
        targetMaterial.SetTexture("_MainTex", resultTexture);

        csKernel = computeShader.FindKernel("CSDefault");

        RenderTexture tmpRT = RenderTexture.GetTemporary(64, 64, 16, RenderTextureFormat.ARGB32);
        tmpRT.enableRandomWrite = true;
        tmpRT.Create();
        
        computeShader.SetTexture(csKernel, "Result", tmpRT);
        //Execute
        computeShader.Dispatch(csKernel, 64 / 32, 64 / 32, 1);
        Graphics.CopyTexture(tmpRT, resultTexture);

        tmpRT.Release();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
